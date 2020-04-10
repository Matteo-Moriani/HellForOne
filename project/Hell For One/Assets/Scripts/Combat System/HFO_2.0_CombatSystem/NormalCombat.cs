using UnityEngine;

// TODO - Rename this after refactor
public class NormalCombat : MonoBehaviour
{
    #region Fields

    // TODO - Rename this after refactor
    private static string normalCombatManagerGameObjectNameAndTag = "CombatManager";

    // TODO - Rename this after refactor
    private CombatSystemManager combatSystemManager;
    
    // TODO - Rename this after refactor
    private NormalCombatManager normalCombatManager;
    private GameObject normalCombatManagerGameObject;

    private Attack currentAttack;

    private Reincarnation reincarnation;

    #endregion

    #region Delegates and events
    
    public delegate void OnAttackHit(NormalCombat sender, Attack attack);
    public event OnAttackHit onAttackHit;
    
    public delegate void OnStartAttack(NormalCombat sender, Attack attack);
    public event OnStartAttack onStartAttack;
    
    public delegate void OnStopAttack(NormalCombat sender, Attack attack);
    public event OnStopAttack onStopAttack;

    #region Methods
    
    private void RaiseOnAttackHit(Attack attack)
    {
        onAttackHit?.Invoke(this,attack);
    }
    
    private void RaiseOnStartAttack(Attack attack)
    {
        onStartAttack?.Invoke(this,attack);
    }
    
    private void RaiseOnStopAttack(Attack attack)
    {
        onStopAttack?.Invoke(this,attack);
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
        normalCombatManager.onAttackHit += CombatManagerOnAttackHitHandler;
        normalCombatManager.onStopAttack += CombatManagerOnStopAttackHandler;

        if (reincarnation != null)
            reincarnation.onReincarnation += OnReincarnation;
    }

    private void OnDisable()
    {
        normalCombatManager.onAttackHit -= CombatManagerOnAttackHitHandler;
        normalCombatManager.onStopAttack += CombatManagerOnStopAttackHandler;

        if (reincarnation != null)
            reincarnation.onReincarnation -= OnReincarnation;
    }

    #endregion
    
    #region Methods
    
    public void StartAttack(Attack attack)
    {
        if(attack == null)
            return;

        currentAttack = attack;

        normalCombatManager.StartAttack(attack);
        
        RaiseOnStartAttack(attack);
    }
    
    public void StartAttackRanged(Attack attack, GameObject target)
    {
        if (attack.IsRanged)
        {
            if(attack == null || target == null)
                return;

            currentAttack = attack;

            normalCombatManager.StartAttackRanged(attack,target);
        
            RaiseOnStartAttack(attack);   
        }
    }
    
    public void StopAttack(Attack attack)
    {
        if(attack == null)
            return;

        normalCombatManager.StopAttack(attack);

        RaiseOnStopAttack(attack);
        
        currentAttack = null;
    }

    public void SetStatsType(Stats.Type newType)
    {
        normalCombatManager.SetStatsType(newType);   
    }

    #endregion

    #region Events handler

    private void OnReincarnation(GameObject gameObject)
    {
        if (currentAttack != null)
        {
            StopAttack(currentAttack);
        }
    }
    
    private void CombatManagerOnStopAttackHandler(NormalCombatManager sender, Attack attack)
    {
        StopAttack(attack);
    }

    private void CombatManagerOnAttackHitHandler(NormalCombatManager sender, Attack attack)
    {   
        if(!attack.CanHitMultipleTargets)
            StopAttack(attack);
        
        RaiseOnAttackHit(attack);
    }
    
    #endregion
}
