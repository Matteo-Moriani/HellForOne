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

    public delegate void OnNormalAttackTry(IdleCombatManager sender, NormalAttack normalAttack, NormalCombat attackerNormalCombat);
    public event OnNormalAttackTry onNormalAttackTry;

    #region Methods

    private void RaiseOnNormalAttackTry(NormalAttack normalAttack, NormalCombat attackerNormalCombat)
    {
        onNormalAttackTry?.Invoke(this,normalAttack, attackerNormalCombat);
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
        idleCollider.onNormalAttackBeingHit += OnNormalAttackBeingHit;
    }

    private void OnDisable()
    {
        idleCollider.onNormalAttackBeingHit -= OnNormalAttackBeingHit;
    }

    #endregion

    #region External events handlers

    private void OnNormalAttackBeingHit(IdleCollider sender, NormalCombat attackerNormalCombat, NormalAttack normalAttack)
    {
        RaiseOnNormalAttackTry(normalAttack, attackerNormalCombat);
    }

    #endregion
}
