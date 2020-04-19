using System.Collections;
using UnityEngine;
using System;

// TODO - Implement a mechanism of initialization
public class GenericAttack : ScriptableObject
{
    #region Fields

    [SerializeField] protected bool hasDamageSupportBonus = false;
    
    [SerializeField] protected bool isRanged = false;
    
    [SerializeField] protected bool canHitMultipleTargets = false;
    
    [SerializeField] protected bool canBeBlocked = false;
    
    [SerializeField] protected bool causeStun = false;
    [SerializeField] protected bool causeStunWhenBlocked = false;

    [SerializeField] protected bool causeKnockback = false;
    [SerializeField] protected bool causeKnockbackWhenBlocked = false;
    
    [SerializeField] protected float damage = 0f;
    [SerializeField] protected float range = 0f;
    [SerializeField] protected float aggroModifier = 0f;
    
    [SerializeField] protected float size = 0f;
    
    [SerializeField] protected float delayInSeconds = 0f;
    [SerializeField] protected float durationInSeconds = 0f;

    protected ObjectsPooler projectilePooler;
    
    #endregion

    #region Properties

    public bool IsRanged
    {
        get => isRanged;
        private set => isRanged = value;
    }
    
    public bool CanHitMultipleTargets
    {
        get => canHitMultipleTargets;
        private set => canHitMultipleTargets = value;
    }

    public bool CanBeBlocked
    {
        get => canBeBlocked;
        private set => canBeBlocked = value;
    }

    public bool CauseStun
    {
        get => causeStun;
        private set => causeStun = value;
    }

    public bool CauseStunWhenBlocked
    {
        get => causeStunWhenBlocked;
        private set => causeStunWhenBlocked = value;
    }

    public float Damage
    {
        get => damage;
        private set => damage = value;
    }

    public float Range
    {
        get => range;
        private set => range = value;
    }

    public float Size
    {
        get => size;
        private set => size = value;
    }

    public float DelayInSeconds
    {
        get => delayInSeconds;
        private set => delayInSeconds = value;
    }

    public float DurationInSeconds
    {
        get => durationInSeconds;
        private set => durationInSeconds = value;
    }

    public bool CauseKnockback
    {
        get => causeKnockback;
        private set => causeKnockback = value;
    }

    public bool CauseKnockbackWhenBlocked
    {
        get => causeKnockbackWhenBlocked;
        private set => causeKnockbackWhenBlocked = value;
    }

    public float AggroModifier
    {
        get => aggroModifier;
        private set => aggroModifier = value;
    }

    public bool HasDamageSupportBonus
    {
        get => hasDamageSupportBonus;
        private set => hasDamageSupportBonus = value;
    }

    #endregion

    #region Methods

    protected virtual ObjectsPooler GetPooler()
    {
        return null;
    }
    
    protected virtual bool IsLegitAttack(GenericIdle targetIdleValues)
    {
        return false;
    }

    public virtual bool CanRiseAggro(Stats.Type unitType)
    {
        return false;
    }

    // Override this to change attack logic in subclasses
    public virtual IEnumerator PerformAttack(GameObject attackColliderGo, Action<GenericAttack> stopAction )
    {
        var targetPosition =
            attackColliderGo.transform.localPosition + new Vector3(0.0f, 0.0f, this.range);

        yield return new WaitForSeconds(this.DelayInSeconds);

        attackColliderGo.transform.localPosition = targetPosition;

        attackColliderGo.GetComponent<AttackCollider>().StartAttack();
        
        attackColliderGo.transform.localScale = Vector3.one * this.Size;

        yield return new WaitForSeconds(this.DurationInSeconds);

        stopAction(this);
    }

    // Override this to change ranged attack logic in subclasses
    public virtual IEnumerator PerformAttackRanged(GameObject target, ProjectileCaster projectileCaster, NormalCombatManager normalCombatManager, AttackCollider.OnAttackHit hitAction, Action<GenericAttack> stopAction)
    {
        if (target != null)
        {
            yield return new WaitForSeconds(this.DelayInSeconds);

            GameObject currentProjectile = projectileCaster.LaunchNewCombatSystem(target, this.GetPooler());

            AttackCollider projectileAttackCollider = currentProjectile.GetComponentInChildren<AttackCollider>();

            if (projectileAttackCollider != null)
            {
                projectileAttackCollider.ResetOnAttackHit();
                Destroy(projectileAttackCollider);    
            }
            
            projectileAttackCollider = currentProjectile.transform.GetChild(0).gameObject.AddComponent<AttackCollider>();

            // TODO - Circular dependency, try to remove this.
            projectileAttackCollider.SetNormalCombatManager(normalCombatManager);
            
            projectileAttackCollider.StartAttack();
            
            projectileAttackCollider.onAttackHit += hitAction;
            
            yield return new WaitForSeconds(5.0f);
            
            stopAction(this);    
        }
    }

    public virtual IEnumerator ManageHit(GenericIdle targetGenericIdle, IdleCollider targetIdleCollider, NormalCombat attackerNormalCombat, Action<GenericIdle> hitAction)
    {
        if (this.IsLegitAttack(targetGenericIdle))
        {
            targetIdleCollider.NotifyOnNormalAttackBeingHit(attackerNormalCombat,this);

            hitAction(targetGenericIdle);
        }

        yield break;
    }

    #endregion
}
