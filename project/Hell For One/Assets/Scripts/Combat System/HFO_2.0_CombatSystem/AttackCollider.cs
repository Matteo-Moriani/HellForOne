using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    #region fields
    
    private bool isAttacking = false;
    
    private NormalCombatManager normalCombatManager;

    public delegate void OnAttackHit(GenericIdle targetGenericIdle);
    public event OnAttackHit onAttackHit;

    #region Methods

    private void RaiseOnAttackHit(GenericIdle targetGenericIdle)
    {
        onAttackHit?.Invoke(targetGenericIdle);
    }

    #endregion
    
    #endregion

    #region Unity methods

    private void Awake()
    {
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
        
        IdleCollider targetIdleCollider = other.GetComponent<IdleCollider>();
        
        if (targetIdleCollider == null)
        {
            return;
        }
        
        GenericIdle targetGenericIdle =
            targetIdleCollider.ParentIdleCombatManager.ParentIdleCombat.GenericIdle;
        
        if(!normalCombatManager.CurrentAttack.IsLegitAttack(targetGenericIdle))
            return;
        
        targetIdleCollider.NotifyOnNormalAttackBeingHit(normalCombatManager.NormalCombat, normalCombatManager.CurrentAttack);
        
        RaiseOnAttackHit(targetGenericIdle);
    }

    #endregion

    #region Methods
    
    public void StartAttack()
    {
        isAttacking = true;
    }

    public void StopAttack()
    {
        isAttacking = false;
        
        transform.localScale = Vector3.zero;
    }

    public void SetNormalCombatManager(NormalCombatManager newNormalCombatManager)
    {
        normalCombatManager = newNormalCombatManager;
    }
    
    public void ResetOnAttackHit()
    {
        onAttackHit = null;
    }
    
    #endregion
}
