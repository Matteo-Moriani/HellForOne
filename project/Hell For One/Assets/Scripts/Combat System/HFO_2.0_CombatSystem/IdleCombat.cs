using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates objects for collider and manage collider scaling
/// (Rigidbody GameObject scale must be 1,1,1 so we need to scale it's parent)
/// </summary>
public class IdleCombat : MonoBehaviour
{
    #region Fields

    private GameObject idleCombatManagerGameObject;
    private IdleCombatManager idleCombatManager;
    private CombatSystemManager combatSystemManager;
    
    private static string IdleCombatManagerGameObjectNameAndTag  = "IdleCombatManager";

    #endregion

    #region Delegates and events

    public delegate void OnNormalAttackTry(IdleCombat sender, NormalAttack normalAttack, NormalCombat attackerNormalCombat);
    public event OnNormalAttackTry onNormalAttackTry;

    public delegate void OnNormalAttackBeingHit(IdleCombat sender, NormalAttack normalAttack, NormalCombat attackerNormalCombat);
    public event OnNormalAttackBeingHit onNormalAttackBeingHit;
    
    #region Methods

    private void RaiseOnNormalAttackTry(NormalAttack normalAttack, NormalCombat attackerNormalCombat)
    {
        onNormalAttackTry?.Invoke(this,normalAttack, attackerNormalCombat);
    }

    private void RaiseOnNormalAttackBeingHit(NormalAttack normalAttack, NormalCombat attackerNormalCombat)
    {
        onNormalAttackBeingHit?.Invoke(this,normalAttack, attackerNormalCombat);
    }

    #endregion
    
    #endregion
    
    #region Unity methods

    private void Awake()
    {
        combatSystemManager = GetComponent<CombatSystemManager>();
        
        idleCombatManagerGameObject = combatSystemManager.CreateCombatSystem_GO(transform,IdleCombatManagerGameObjectNameAndTag);
        idleCombatManager = idleCombatManagerGameObject.AddComponent<IdleCombatManager>();
    }

    private void OnEnable()
    {
        idleCombatManager.onNormalAttackTry += OnNormalAttackTryHandler;
    }

    private void OnDisable()
    {
        idleCombatManager.onNormalAttackTry -= OnNormalAttackTryHandler;
    }

    #endregion

    #region Event handlers

    private void OnNormalAttackTryHandler(IdleCombatManager sender,NormalAttack normalAttack, NormalCombat attackerNormalCombat)
    {
        if (!normalAttack.CanBeBlocked)
        {
            RaiseOnNormalAttackBeingHit(normalAttack, attackerNormalCombat);
            return;
        }

        var block = gameObject.GetComponent<Block>();

        if (block == null)
        {
            return;
            Debug.LogError(this.name + " " + this.transform.root.gameObject.name + " is receiving a blockable NormalAttack but his CombatSystem does not have Block attached");
        }
        
        RaiseOnNormalAttackTry(normalAttack, attackerNormalCombat);
    }
    
    #endregion
}
