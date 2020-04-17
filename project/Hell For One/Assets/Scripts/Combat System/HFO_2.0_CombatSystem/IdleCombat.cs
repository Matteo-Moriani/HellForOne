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

    [SerializeField] private GenericIdle genericIdle;
    
    private GameObject idleCombatManagerGameObject;
    private IdleCombatManager idleCombatManager;
    private CombatSystemManager combatSystemManager;
    
    private static string IdleCombatManagerGameObjectNameAndTag  = "IdleCombatManager";

    #endregion

    #region Properties

    public GenericIdle GenericIdle
    {
        get => genericIdle;
        private set => genericIdle = value;
    }

    #endregion
    
    #region Delegates and events

    public delegate void OnAttackTry(IdleCombat sender, GenericAttack attack, NormalCombat attackerNormalCombat);
    public event OnAttackTry onAttackTry;

    public delegate void OnAttackBeingHit(IdleCombat sender, GenericAttack attack, NormalCombat attackerNormalCombat);
    public event OnAttackBeingHit onAttackBeingHit;

    #region Methods

    private void RaiseOnAttackTry(GenericAttack attack, NormalCombat attackerNormalCombat)
    {
        onAttackTry?.Invoke(this,attack, attackerNormalCombat);
    }

    private void RaiseOnAttackBeingHit(GenericAttack attack, NormalCombat attackerNormalCombat)
    {
        onAttackBeingHit?.Invoke(this,attack, attackerNormalCombat);
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
        idleCombatManager.onAttackTry += OnAttackTryHandler;
    }

    private void OnDisable()
    {
        idleCombatManager.onAttackTry -= OnAttackTryHandler;
    }

    #endregion

    #region Event handlers
    
    private void OnAttackTryHandler(IdleCombatManager sender,GenericAttack attack, NormalCombat attackerNormalCombat)
    {
        if (!attack.CanBeBlocked)
        {
            RaiseOnAttackBeingHit(attack, attackerNormalCombat);
            return;
        }

        var block = gameObject.GetComponent<Block>();

        if (block == null)
        {
            return;
            Debug.LogError(this.name + " " + this.transform.root.gameObject.name + " is receiving a blockable NormalAttack but his CombatSystem does not have Block attached");
        }
        
        RaiseOnAttackTry(attack, attackerNormalCombat);
    }

    #endregion
}
