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

    public delegate void OnNormalAttackBeingHit(IdleCollider sender, NormalCombat attackerNormalCombat, NormalAttack normalAttack);
    public event OnNormalAttackBeingHit onNormalAttackBeingHit;

    #region Methods
    
    private void RaiseOnNormalAttackBeingHit(NormalCombat attackerNormalCombat, NormalAttack normalAttack) {
        onNormalAttackBeingHit?.Invoke(this,attackerNormalCombat,normalAttack);
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
    
    public void NotifyOnNormalAttackBeingHit(NormalCombat attackerNormalCombat, NormalAttack normalAttack) { 
        if(canBeDamaged)
            RaiseOnNormalAttackBeingHit(attackerNormalCombat,normalAttack);    
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
