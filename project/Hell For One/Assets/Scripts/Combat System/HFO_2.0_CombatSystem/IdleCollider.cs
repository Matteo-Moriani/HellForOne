using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleCollider : MonoBehaviour
{
    #region Fields

    private static string nameAndTag = IdleCombatManager.NameAndTag;
    private bool canBeDamaged = true;

    private Dash dash;
    
    #endregion

    #region Properties

    public static string NameAndTag
    {
        get => nameAndTag;
        private set => nameAndTag = value;
    }

    #endregion
    
    #region Delegates and events

    public delegate void OnAttackBeingHit(IdleCollider sender, NormalCombat attackerNormalCombat, Attack attack);
    public event OnAttackBeingHit onAttackBeingHit;

    #region Methods
    
    private void RaiseOnAttackBeingHit(NormalCombat attackerNormalCombat, Attack attack) {
        onAttackBeingHit?.Invoke(this,attackerNormalCombat,attack);
    }

    #endregion
    
    #endregion

    #region Unity methods

    private void Awake()
    {
        dash = transform.root.gameObject.GetComponent<Dash>();
    }

    private void OnEnable()
    {
        if (dash != null)
        {
            dash.onDashStart += OnDashStart;
            dash.onDashStop += OnDashStop;    
        }
    }

    private void OnDisable()
    {
        if (dash != null)
        {
            dash.onDashStart -= OnDashStart;
            dash.onDashStop -= OnDashStop;    
        }
    }

    #endregion
    
    #region Methods
    
    public void NotifyOnNormalAttackBeingHit(NormalCombat attackerNormalCombat, Attack attack) { 
        if(canBeDamaged)
            RaiseOnAttackBeingHit(attackerNormalCombat,attack);    
    }

    #endregion

    #region Event handlers

    private void OnDashStart()
    {
        canBeDamaged = false;
    }

    private void OnDashStop()
    {
        canBeDamaged = true;
    }

    #endregion
}
