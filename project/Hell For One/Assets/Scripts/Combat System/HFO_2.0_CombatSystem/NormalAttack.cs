using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalAttack", menuName = "CombatSystem/NormalAttack", order = 1)]
public class NormalAttack : ScriptableObject
{
    #region Fields

    [SerializeField] private bool hasDamageSupportBonus = false;
    
    [SerializeField] private bool isRanged = false;
    
    [SerializeField] private bool canHitMultipleTargets = false;
    
    [SerializeField] private bool canBeBlocked = false;
    
    [SerializeField] private bool causeStun = false;
    [SerializeField] private bool causeStunWhenBlocked = false;

    [SerializeField] private bool causeKnockback = false;
    [SerializeField] private bool causeKnockbackWhenBlocked = false;
    
    [SerializeField] private float damage = 0f;
    [SerializeField] private float range = 0f;
    [SerializeField] private float aggroModifier = 0f;
    
    [SerializeField] private float sizeX = 0f;
    [SerializeField] private float sizeY = 0f;
    [SerializeField] private float sizeZ = 0f;

    [SerializeField] private float delayInSeconds = 0f;
    [SerializeField] private float durationInSeconds = 0f;
    
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

    public float SizeX
    {
        get => sizeX;
        private set => sizeX = value;
    }
    
    public float SizeY
    {
        get => sizeY;
        private set => sizeY = value;
    }

    public float SizeZ
    {
        get => sizeZ;
        private set => sizeZ = value;
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

    #region methods
    

    #endregion
}
