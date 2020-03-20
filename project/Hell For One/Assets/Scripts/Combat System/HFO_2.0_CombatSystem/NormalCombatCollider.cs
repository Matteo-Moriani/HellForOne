using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class NormalCombatCollider : MonoBehaviour
{
    #region fields

    private Stats.Type type;
    private bool isAttacking = false;
    
    private NormalCombatManager normalCombatManager;

    public delegate void OnNormalAttackHit(NormalCombatCollider sender);
    public event OnNormalAttackHit onNormalAttackHit;

    #endregion

    #region Unity methods

    private void Awake()
    {
        Stats stats = transform.root.gameObject.GetComponent<Stats>();
        
        if(stats != null)
            type = transform.root.gameObject.GetComponent<Stats>().ThisUnitType;
        
        normalCombatManager = this.transform.parent.gameObject.GetComponent<NormalCombatManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking)
        {
            return;
        }

        if (!other.CompareTag(IdleCollider.NameAndTag))
        {
            return;
        }

        var targetType = other.transform.root.gameObject.GetComponent<Stats>().ThisUnitType;
        
        if (!IsLegitAttack(targetType))
        {
            return;
        }

        var targetIdleCollider = other.GetComponent<IdleCollider>();
        
        if (targetIdleCollider == null)
        {
            return;
        }
        
        targetIdleCollider.NotifyOnNormalAttackBeingHit(normalCombatManager.NormalCombat, normalCombatManager.CurrentNormalAttack);
        
        RaiseOnNormalAttackHit();
    }

    #endregion

    #region Methods

    private bool IsLegitAttack(Stats.Type targetType)
    {
        switch (type)
        {
            case Stats.Type.Player:
                if (targetType == Stats.Type.Boss)
                {
                    return true;
                }
                break;
            case Stats.Type.Ally:
                if (targetType == Stats.Type.Boss)
                {
                    return true;
                }
                break;
            case Stats.Type.Boss:
                if (targetType == Stats.Type.Ally || targetType == Stats.Type.Player)
                {
                    return true;
                }
                break;
        }
        return false;
    }

    public void StartAttack()
    {
        isAttacking = true;
    }

    public void StopAttack()
    {
        isAttacking = false;
        
        transform.localScale = Vector3.zero;
    }

    public void SetStatsType(Stats.Type newType)
    {
        type = newType;
    }

    public void SetNormalCombatManager(NormalCombatManager newNormalCombatManager)
    {
        normalCombatManager = newNormalCombatManager;
    }
    
    #endregion

    #region Events

    public void ResetOnNormalAttackHit()
    {
        onNormalAttackHit = null;
    }
    
    private void RaiseOnNormalAttackHit()
    {
        onNormalAttackHit?.Invoke(this);
    }

    #endregion
}
