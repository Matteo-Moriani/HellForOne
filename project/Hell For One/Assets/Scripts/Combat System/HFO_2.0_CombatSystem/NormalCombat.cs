using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

public class NormalCombat : MonoBehaviour
{
    #region Fields

    private static string normalCombatManagerGameObjectNameAndTag = "NormalCombatManager";

    private CombatSystemManager combatSystemManager;
    
    private NormalCombatManager normalCombatManager;
    private GameObject normalCombatManagerGameObject;

    private NormalAttack currentNormalAttack;

    private Reincarnation reincarnation;

    #endregion

    #region Properties
    
    public static string NormalCombatManagerGameObjectNameAndTag { get => normalCombatManagerGameObjectNameAndTag; private set => normalCombatManagerGameObjectNameAndTag = value; }
    
    public NormalCombatManager NormalCombatManager { get => normalCombatManager; private set => normalCombatManager = value; }

    #endregion

    #region Delegates and events

    // TODO - Implement aggro
    public delegate void OnNormalAttackHit(NormalCombat sender, NormalAttack normalAttack);
    public event OnNormalAttackHit onNormalAttackHit;

    // TODO - Implement Audio, animation
    public delegate void OnStartNormalAttack(NormalCombat sender, NormalAttack normalAttack);
    public event OnStartNormalAttack onStartNormalAttack;

    // TODO - Implement Audio, animation
    public delegate void OnStopNormalAttack(NormalCombat sender, NormalAttack normalAttack);
    public event OnStopNormalAttack onStopNormalAttack;

    // Used to apply bonus etc etc
    public delegate void OnAwakeNormalAttack(NormalCombat sender, NormalAttack normalAttack);
    public event OnAwakeNormalAttack onAwakeNormalAttack;

    #region Methods

    private void RaiseOnAwakeNormalAttack(NormalAttack normalAttack)
    {
        onAwakeNormalAttack?.Invoke(this,normalAttack);
    }

    private void RaiseOnNormalAttackHit(NormalAttack normalAttack)
    {
        onNormalAttackHit?.Invoke(this,normalAttack);
    }
    
    private void RaiseOnStartNormalAttack(NormalAttack normalAttack)
    {
        onStartNormalAttack?.Invoke(this,normalAttack);
    }
    
    private void RaiseOnStopNormalAttack(NormalAttack normalAttack)
    {
        onStopNormalAttack?.Invoke(this,normalAttack);
    }

    #endregion
    
    #endregion

    #region Unity methods

    private void Awake()
    {
        combatSystemManager = GetComponent<CombatSystemManager>();
        
        normalCombatManagerGameObject = combatSystemManager.CreateCombatSystem_GO(transform, normalCombatManagerGameObjectNameAndTag);
        normalCombatManager = normalCombatManagerGameObject.AddComponent<NormalCombatManager>();

        reincarnation = transform.root.GetComponent<Reincarnation>();
    }

    private void OnEnable()
    {
        normalCombatManager.onNormalAttackHit += NormalCombatManagerOnNormalAttackHitHandler;

        if (reincarnation != null)
            reincarnation.onReincarnation += OnReincarnation;
    }

    private void OnDisable()
    {
        normalCombatManager.onNormalAttackHit -= NormalCombatManagerOnNormalAttackHitHandler;

        if (reincarnation != null)
            reincarnation.onReincarnation -= OnReincarnation;
    }

    #endregion
    
    #region Methods
    
    public void StartNormalAttack(NormalAttack normalAttack)
    {
        if(normalAttack == null)
            return;

        currentNormalAttack = normalAttack;
        
        RaiseOnAwakeNormalAttack(normalAttack);
        
        normalCombatManager.StartNormalAttack(normalAttack);
        
        RaiseOnStartNormalAttack(normalAttack);
    }

    // TODO - implement [Imps]: melee, ranged
    // TODO - implement [MidBoss]: Swipe
    // TODO - Implement [Boss]: Swipe
    public void StartNormalAttack(NormalAttack normalAttack, GameObject target)
    {
        if(normalAttack == null || target == null)
            return;

        currentNormalAttack = normalAttack;

        normalCombatManager.StartNormalAttack(normalAttack,target);
        
        RaiseOnStartNormalAttack(normalAttack);    
    }

    // TODO - implement [Imps]: melee, ranged
    // TODO - implement [MidBoss]: Swipe
    // TODO - Implement [Boss]: Swipe
    public void StopNormalAttack(NormalAttack normalAttack)
    {
        if(normalAttack == null)
            return;

        normalCombatManager.StopNormalAttack(normalAttack);

        RaiseOnStopNormalAttack(normalAttack);
        
        currentNormalAttack = null;
    }
    
    #endregion

    #region Events handler

    private void OnReincarnation(GameObject gameObject)
    {
        if (currentNormalAttack != null)
        {
            StopNormalAttack(currentNormalAttack);
        }
    }

    private void NormalCombatManagerOnNormalAttackHitHandler(NormalCombatManager sender, NormalAttack normalAttack)
    {   
        if(!normalAttack.CanHitMultipleTargets)
            StopNormalAttack(normalAttack);
        
        RaiseOnNormalAttackHit(normalAttack);
    }
    
    #endregion
}
