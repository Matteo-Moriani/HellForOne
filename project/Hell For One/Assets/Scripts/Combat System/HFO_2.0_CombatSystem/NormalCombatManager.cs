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
    private GenericAttack currentAttack;

    private GameObject attackGameObject;

    private Vector3 attackGameObjectStartPosition;

    private static readonly string attackGameObjectNameAndTag = "Attack";

    private bool isIdle = true;
    
    private Coroutine attackCr = null;

    #region properties

    public NormalCombat NormalCombat { get => normalCombat; private set => normalCombat = value; }

    public GenericAttack CurrentAttack
    {
        get => currentAttack;
        private set => currentAttack = value;
    }

    #endregion

    #endregion

    #region Delegates and events

    public delegate void OnAttackHit(GenericAttack attack, GenericIdle targetGenericIdle);
    public event OnAttackHit onAttackHit;

    public delegate void OnStopAttack(NormalCombatManager sender, GenericAttack attack);
    public event OnStopAttack onStopAttack;

    #region Methods

    private void RaiseOnStopAttack(GenericAttack attack)
    {
        onStopAttack?.Invoke(this,attack);
    }

    private void RaiseOnAttackHit(GenericAttack attack, GenericIdle targetGenericIdle)
    {
        onAttackHit?.Invoke(attack, targetGenericIdle);
    }

    #endregion 
    
    #endregion

    #region Unity methods

    private void Awake()
    {
        CreateAttackGameObject();
        
        normalCombat = GetComponentInParent<NormalCombat>();
        
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
        
        // Reset collider's object scale
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
    public void StartAttack(GenericAttack attack)
    {
        if (attackCr == null && isIdle)
        {
            currentAttack = attack;
            attackCr = StartCoroutine(AttackCoroutine(attack));
        }
    }

    // Used for ranged attacks
    public void StartAttackRanged(GenericAttack attack, GameObject target)
    {
        if (attackCr != null || !isIdle || target == null) return;
        
        currentAttack = attack;
        attackCr = StartCoroutine(AttackRangedCoroutine(attack, target));
    }

    public void StopAttack(GenericAttack attack)
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

    #endregion

    #region Events handler

    private void AttackColliderOnAttackHitHandler(GenericIdle targetGenericIdle)
    {
        if (!currentAttack.CanHitMultipleTargets || targetGenericIdle.BlockMultipleTargetAttacks)
        { 
            StopAttack(currentAttack);
        }
        
        RaiseOnAttackHit(currentAttack, targetGenericIdle);
    }
    
    #endregion

    #region Coroutines

    private IEnumerator AttackCoroutine(GenericAttack attack)
    {
        isIdle = false;

        var targetPosition =
            attackGameObject.transform.localPosition + new Vector3(0.0f, 0.0f, attack.Range);

        yield return new WaitForSeconds(attack.DelayInSeconds);

        attackGameObject.transform.localPosition = targetPosition;

        attackCollider.StartAttack();
        
        attackGameObject.transform.localScale = Vector3.one * attack.Size;

        yield return new WaitForSeconds(attack.DurationInSeconds);
        
        StopAttack(attack);
    }
    
    private IEnumerator AttackRangedCoroutine(GenericAttack attack, GameObject target)
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

            // TODO - Circular dependency, try to remove this.
            projectileAttackCollider.SetNormalCombatManager(this);
            
            projectileAttackCollider.StartAttack();
            
            projectileAttackCollider.onAttackHit += AttackColliderOnAttackHitHandler;
            
            yield return new WaitForSeconds(5.0f);
            
            StopAttack(attack);    
        }
    }

    #endregion
}
