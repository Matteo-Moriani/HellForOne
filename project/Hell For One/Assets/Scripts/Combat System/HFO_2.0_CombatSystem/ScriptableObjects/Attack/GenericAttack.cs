using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public virtual ObjectsPooler GetPooler()
    {
        return null;
    }
    
    public virtual bool IsLegitAttack(GenericIdle targetIdleValues)
    {
        return false;
    }

    public virtual bool CanRiseAggro(Stats.Type unitType)
    {
        return false;
    }

    #endregion
}
