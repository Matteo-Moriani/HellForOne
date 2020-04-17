using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GenericIdle : ScriptableObject
{
    [SerializeField]
    private bool causeStunWhenHit = false;

    [SerializeField]
    private float attackerStunDuration = 0f;
    
    [SerializeField]
    private bool blockMultipleTargetAttacks = false;
    
    public bool CauseStunWhenHit
    {
        get => causeStunWhenHit;
        private set => causeStunWhenHit = value;
    }

    public bool BlockMultipleTargetAttacks
    {
        get => blockMultipleTargetAttacks;
        private set => blockMultipleTargetAttacks = value;
    }

    public float AttackerStunDuration
    {
        get
        {
            if(causeStunWhenHit)
                return attackerStunDuration;
            else
            {
                Debug.LogError("Access to IdleValues.AttackStunDuration but CauseStunWhenHit = false");
                return 0f;
            }
        }
        
        private set => attackerStunDuration = value;
    }
}
