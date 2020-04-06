using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class IdleCombatManager : MonoBehaviour
{
    #region Fields

    private static string nameAndTag = "IdleCollider";

    private GameObject idleColliderGameObject;
    private IdleCollider idleCollider;
    private CombatSystemManager combatSystemManager;

    #endregion
    
    #region Delegates and events

    public delegate void OnAttackTry(IdleCombatManager sender, Attack attack, NormalCombat attackerNormalCombat);
    public event OnAttackTry onAttackTry;

    #region Methods

    private void RaiseOnAttackTry(Attack attack, NormalCombat attackerNormalCombat)
    {
        onAttackTry?.Invoke(this,attack, attackerNormalCombat);
    }

    #endregion
    
    #endregion

    #region Properties

    public static string NameAndTag
    {
        get => nameAndTag;
        private set => nameAndTag = value;
    }

    #endregion

    #region Unity methods

    private void Awake()
    {
        combatSystemManager = transform.parent.gameObject.GetComponent<CombatSystemManager>();
        
        idleColliderGameObject = combatSystemManager.CreateCombatSystemCollider_GO(transform, nameAndTag);
        
        // TODO - Remove this after testing
        idleColliderGameObject.GetComponent<Renderer>().material = combatSystemManager.idleMaterial;

        idleCollider = idleColliderGameObject.AddComponent<IdleCollider>();
    }

    private void OnEnable()
    {
        idleCollider.onAttackBeingHit += OnAttackBeingHit;
    }

    private void OnDisable()
    {
        idleCollider.onAttackBeingHit -= OnAttackBeingHit;
    }

    #endregion

    #region External events handlers

    private void OnAttackBeingHit(IdleCollider sender, NormalCombat attackerNormalCombat, Attack attack)
    {
        RaiseOnAttackTry(attack, attackerNormalCombat);
    }

    #endregion
}
