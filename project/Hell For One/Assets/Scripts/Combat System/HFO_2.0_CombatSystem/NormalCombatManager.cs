using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NormalCombatManager : MonoBehaviour
{
    #region Fields

    private CombatSystemManager combatSystemManager;
    private NormalCombat normalCombat;
    private NormalCombatCollider normalCombatCollider;
    private GameObject currentLance;
    private Lancer lancer;
    private NormalAttack currentNormalAttack;

    private GameObject normalAttackGameObject;

    private Vector3 normalAttackStartPosition;

    private static readonly string normalAttackNameAndTag = "NormalAttack";

    private bool isIdle = true;
    
    private Coroutine normalAttackCr = null;

    #region properties

    public NormalCombat NormalCombat { get => normalCombat; private set => normalCombat = value; }

    public NormalAttack CurrentNormalAttack
    {
        get => currentNormalAttack;
        private set => currentNormalAttack = value;
    }

    #endregion

    #endregion

    #region Delegates and events

    public delegate void OnNormalAttackHit(NormalCombatManager sender, NormalAttack normalAttack);
    public event OnNormalAttackHit onNormalAttackHit;
    
    #endregion

    #region Unity methods

    private void Awake()
    {
        combatSystemManager = transform.parent.GetComponent<CombatSystemManager>();
        
        // Creating objects for collider
        normalAttackGameObject = combatSystemManager.CreateCombatSystemCollider_GO(transform, normalAttackNameAndTag);
        
        // Rest collider's object scale
        // Correct scale is managed by attacks
        normalAttackGameObject.transform.localScale = Vector3.zero;
        
        // TODO - Remove Materials after testing
        normalAttackGameObject.GetComponent<Renderer>().material = combatSystemManager.normalAttackMaterial;
        
        // Get collider's objects start position
        normalAttackStartPosition = normalAttackGameObject.transform.localPosition;

        // Assign collider logic to objects
        normalCombatCollider = normalAttackGameObject.AddComponent<NormalCombatCollider>();

        normalCombat = GetComponentInParent<NormalCombat>();
        lancer = transform.root.gameObject.GetComponent<Lancer>();
    }

    private void OnEnable()
    {
        normalCombatCollider.onNormalAttackHit += NormalCombatColliderOnNormalCombatHitHandler;
    }

    private void OnDisable()
    {
        normalCombatCollider.onNormalAttackHit -= NormalCombatColliderOnNormalCombatHitHandler;
    }

    #endregion

    #region Methods

    // Used for melee attacks
    public void StartNormalAttack(NormalAttack normalAttack)
    {
        if (normalAttackCr == null && isIdle)
        {
            currentNormalAttack = normalAttack;
            normalAttackCr = StartCoroutine(NormalAttackCoroutine(normalAttack));
        }
    }

    // Used for ranged attacks
    public void StartNormalAttack(NormalAttack normalAttack, GameObject target)
    {
        if (normalAttackCr != null || !isIdle || target == null) return;
        
        currentNormalAttack = normalAttack;
        normalAttackCr = StartCoroutine(NormalAttackCoroutine(normalAttack, target));
    }

    public void StopNormalAttack(NormalAttack normalAttack)
    {
        if (normalAttackCr != null && !isIdle)
        {
            StopCoroutine(normalAttackCr);
            normalAttackCr = null;

            normalAttackGameObject.transform.localPosition = normalAttackStartPosition;
            
            normalCombatCollider.StopAttack();

            isIdle = true;
            //currentNormalAttack = null;
        }
    }

    #endregion

    #region Internal events

    private void RaiseOnNormalAttackHit(NormalAttack normalAttack)
    {
        onNormalAttackHit?.Invoke(this,normalAttack);
    }
    
    #endregion

    #region Events handler

    private void NormalCombatColliderOnNormalCombatHitHandler(NormalCombatCollider sender)
    {
        if (!currentNormalAttack.CanHitMultipleTargets)
        { 
            StopNormalAttack(currentNormalAttack);
        }
        RaiseOnNormalAttackHit(currentNormalAttack);
    }
    
    #endregion

    #region Coroutines

    private IEnumerator NormalAttackCoroutine(NormalAttack normalAttack)
    {
        isIdle = false;

        var targetPosition =
            normalAttackGameObject.transform.localPosition + new Vector3(0.0f, 0.0f, normalAttack.Range);
        
        normalAttackGameObject.transform.localScale = new Vector3( normalAttack.SizeX, normalAttack.SizeY, normalAttack.SizeZ);

        yield return new WaitForSeconds(normalAttack.DelayInSeconds);

        normalAttackGameObject.transform.localPosition = targetPosition;
        
        normalCombatCollider.StartAttack();

        yield return new WaitForSeconds(normalAttack.DurationInSeconds);
        
        StopNormalAttack(normalAttack);
    }
    
    private IEnumerator NormalAttackCoroutine(NormalAttack normalAttack, GameObject target)
    {
        if (target != null)
        {
            isIdle = false;

            yield return new WaitForSeconds(normalAttack.DelayInSeconds);

            currentLance = lancer.LaunchNewCombatSystem(target);

            NormalCombatCollider lanceNormalCombatCollider = currentLance.GetComponentInChildren<NormalCombatCollider>();

            if (lanceNormalCombatCollider != null)
            {
                lanceNormalCombatCollider.ResetOnNormalAttackHit();
                Destroy(lanceNormalCombatCollider);    
            }
            
            lanceNormalCombatCollider = currentLance.transform.GetChild(0).gameObject.AddComponent<NormalCombatCollider>();
            lanceNormalCombatCollider.SetStatsType(transform.root.gameObject.GetComponent<Stats>().ThisUnitType);
            lanceNormalCombatCollider.SetNormalCombatManager(this);
            lanceNormalCombatCollider.StartAttack();
            
            lanceNormalCombatCollider.onNormalAttackHit += NormalCombatColliderOnNormalCombatHitHandler;
            
            yield return new WaitForSeconds(5.0f);
            
            StopNormalAttack(normalAttack);    
        }
    }

    #endregion
}
