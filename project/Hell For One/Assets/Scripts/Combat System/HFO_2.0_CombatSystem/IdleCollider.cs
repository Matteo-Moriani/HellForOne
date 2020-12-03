using System;
using System.Collections;
using System.Collections.Generic;
using FactoryBasedCombatSystem;
using UnityEngine;

public class IdleCollider : MonoBehaviour
{
    #region Fields

    private static string nameAndTag = IdleCombatManager.NameAndTag;
    private bool canBeDamaged = true;
    
    // TODO - Bad design
    private Dash dash;
    private IdleCombatManager parentIdleCombatManager;
    
    #endregion

    #region Properties

    public static string NameAndTag
    {
        get => nameAndTag;
        private set => nameAndTag = value;
    }

    public IdleCombatManager ParentIdleCombatManager
    {
        get => parentIdleCombatManager;
        private set => parentIdleCombatManager = value;
    }

    #endregion
    
    #region Delegates and events

    public delegate void OnAttackBeingHit(IdleCollider sender, NormalCombat attackerNormalCombat, GenericAttack attack);
    public event OnAttackBeingHit onAttackBeingHit;

    #region Methods
    
    private void RaiseOnAttackBeingHit(NormalCombat attackerNormalCombat, GenericAttack attack) {
        onAttackBeingHit?.Invoke(this,attackerNormalCombat,attack);
    }

    #endregion
    
    #endregion

    #region Unity methods

    private void Awake()
    {
        dash = transform.root.gameObject.GetComponent<Dash>();
        parentIdleCombatManager = transform.parent.GetComponent<IdleCombatManager>();
    }

    private void OnEnable()
    {
        if (dash != null)
        {
            dash.OnStartDash += OnDashStart;
            dash.OnStopDash += OnDashStop;    
        }
    }

    private void OnDisable()
    {
        if (dash != null)
        {
            dash.OnStartDash -= OnDashStart;
            dash.OnStopDash -= OnDashStop;    
        }
    }

    #endregion
    
    #region Methods
    
    public void NotifyOnNormalAttackBeingHit(NormalCombat attackerNormalCombat, GenericAttack attack) { 
        if(canBeDamaged)
            RaiseOnAttackBeingHit(attackerNormalCombat,attack);    
    }

    #endregion

    #region Event handlers
    
    // TODO - Bad design
    private void OnDashStart()
    {
        canBeDamaged = false;
    }

    // TODO - Bad design
    private void OnDashStop()
    {
        canBeDamaged = true;
    }

    #endregion
}
