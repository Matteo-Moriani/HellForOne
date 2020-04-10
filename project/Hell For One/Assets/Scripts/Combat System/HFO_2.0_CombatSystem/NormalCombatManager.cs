using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NormalCombatManager : MonoBehaviour
{
    #region Fields

    private CombatSystemManager combatSystemManager;
    private NormalCombat normalCombat;
    private AttackCollider attackCollider;
    private GameObject currentProjectile;
    private ProjectileCaster projectileCaster;
    private Attack currentAttack;

    private GameObject attackGameObject;

    private Vector3 attackGameObjectStartPosition;

    private static readonly string attackGameObjectNameAndTag = "Attack";

    private bool isIdle = true;
    
    private Coroutine attackCr = null;

    #region properties

    public NormalCombat NormalCombat { get => normalCombat; private set => normalCombat = value; }

    public Attack CurrentAttack
    {
        get => currentAttack;
        private set => currentAttack = value;
    }

    #endregion

    #endregion

    #region Delegates and events

    public delegate void OnAttackHit(NormalCombatManager sender, Attack attack);
    public event OnAttackHit onAttackHit;

    public delegate void OnStopAttack(NormalCombatManager sender, Attack attack);
    public event OnStopAttack onStopAttack;

    #region Methods

    private void RaiseOnStopAttack(Attack attack)
    {
        onStopAttack?.Invoke(this,attack);
    }

    private void RaiseOnAttackHit(Attack attack)
    {
        onAttackHit?.Invoke(this,attack);
    }

    #endregion 
    
    #endregion

    #region Unity methods

    private void Awake()
    {
        CreateAttackGameObject();
        
        normalCombat = GetComponentInParent<NormalCombat>();
        //projectileCaster = transform.root.gameObject.GetComponent<ProjectileCaster>();
        projectileCaster = GetComponentInParent<ProjectileCaster>();
    }

    private void OnEnable()
    {
        attackCollider.onAttackHit += AttackColliderOnAttackHitHandler;
    }

    private void OnDisable()
    {
        attackCollider.onAttackHit -= AttackColliderOnAttackHitHandler;
    }

    #endregion

    #region Methods

    private void CreateAttackGameObject()
    {
        combatSystemManager = transform.parent.GetComponent<CombatSystemManager>();
        
        // Creating objects for collider
        attackGameObject = combatSystemManager.CreateCombatSystemCollider_GO(transform, attackGameObjectNameAndTag);
        
        // Rest collider's object scale
        // Correct scale is managed by attacks
        attackGameObject.transform.localScale = Vector3.zero;
        
        // TODO - Remove Materials after testing
        attackGameObject.GetComponent<Renderer>().material = combatSystemManager.normalAttackMaterial;
        
        // Get collider's objects start position
        attackGameObjectStartPosition = attackGameObject.transform.localPosition;

        // Assign collider logic to objects
        attackCollider = attackGameObject.AddComponent<AttackCollider>();
    }

    // Used for melee attacks
    public void StartAttack(Attack attack)
    {
        if (attackCr == null && isIdle)
        {
            currentAttack = attack;
            attackCr = StartCoroutine(AttackCoroutine(attack));
        }
    }

    // Used for ranged attacks
    public void StartAttackRanged(Attack attack, GameObject target)
    {
        if (attackCr != null || !isIdle || target == null) return;
        
        currentAttack = attack;
        attackCr = StartCoroutine(AttackRangedCoroutine(attack, target));
    }

    public void StopAttack(Attack attack)
    {
        if (attackCr != null && !isIdle)
        {
            StopCoroutine(attackCr);
            attackCr = null;

            attackGameObject.transform.localPosition = attackGameObjectStartPosition;
            
            attackCollider.StopAttack();

            isIdle = true;
            
            RaiseOnStopAttack(attack);
        }
    }
    
    public void SetStatsType(Stats.Type newType)
    {
        attackCollider.SetStatsType(newType);     
    }

    #endregion

    #region Events handler

    private void AttackColliderOnAttackHitHandler(AttackCollider sender)
    {
        if (!currentAttack.CanHitMultipleTargets)
        { 
            StopAttack(currentAttack);
        }
        RaiseOnAttackHit(currentAttack);
    }
    
    #endregion

    #region Coroutines

    private IEnumerator AttackCoroutine(Attack attack)
    {
        isIdle = false;

        var targetPosition =
            attackGameObject.transform.localPosition + new Vector3(0.0f, 0.0f, attack.Range);
        
        //attackGameObject.transform.localScale = Vector3.one * attack.Size;

        yield return new WaitForSeconds(attack.DelayInSeconds);

        attackGameObject.transform.localPosition = targetPosition;

        attackCollider.StartAttack();
        
        attackGameObject.transform.localScale = Vector3.one * attack.Size;

        yield return new WaitForSeconds(attack.DurationInSeconds);
        
        StopAttack(attack);
    }
    
    private IEnumerator AttackRangedCoroutine(Attack attack, GameObject target)
    {
        if (target != null)
        {
            isIdle = false;

            yield return new WaitForSeconds(attack.DelayInSeconds);

            currentProjectile = projectileCaster.LaunchNewCombatSystem(target,attack.GetPooler());

            AttackCollider projectileAttackCollider = currentProjectile.GetComponentInChildren<AttackCollider>();

            if (projectileAttackCollider != null)
            {
                projectileAttackCollider.ResetOnAttackHit();
                Destroy(projectileAttackCollider);    
            }
            
            projectileAttackCollider = currentProjectile.transform.GetChild(0).gameObject.AddComponent<AttackCollider>();

            Stats stats = transform.root.gameObject.GetComponent<Stats>();
            if(stats != null)
                projectileAttackCollider.SetStatsType(transform.root.gameObject.GetComponent<Stats>().ThisUnitType);
            else
                projectileAttackCollider.SetStatsType(Stats.Type.Ally);
            
            projectileAttackCollider.SetNormalCombatManager(this);
            projectileAttackCollider.StartAttack();
            
            projectileAttackCollider.onAttackHit += AttackColliderOnAttackHitHandler;
            
            yield return new WaitForSeconds(5.0f);
            
            StopAttack(attack);    
        }
    }

    #endregion
}
