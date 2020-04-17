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

    private GenericAttack currentAttack;

    private Reincarnation reincarnation;

    #endregion

    #region Delegates and events
    
    public delegate void OnAttackHit(GenericAttack attack, GenericIdle targetGenericIdle);
    public event OnAttackHit onAttackHit;
    
    public delegate void OnStartAttack(NormalCombat sender, GenericAttack attack);
    public event OnStartAttack onStartAttack;
    
    public delegate void OnStopAttack(NormalCombat sender, GenericAttack attack);
    public event OnStopAttack onStopAttack;

    #region Methods
    
    private void RaiseOnAttackHit(GenericAttack attack, GenericIdle targetGenericIdle)
    {
        onAttackHit?.Invoke(attack, targetGenericIdle);
    }
    
    private void RaiseOnStartAttack(GenericAttack attack)
    {
        onStartAttack?.Invoke(this,attack);
    }
    
    private void RaiseOnStopAttack(GenericAttack attack)
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
    
    public void StartAttack(GenericAttack attack)
    {
        if(attack == null)
            return;

        currentAttack = attack;

        normalCombatManager.StartAttack(attack);
        
        RaiseOnStartAttack(attack);
    }
    
    public void StartAttackRanged(GenericAttack attack, GameObject target)
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
    
    public void StopAttack(GenericAttack attack)
    {
        if(attack == null)
            return;

        normalCombatManager.StopAttack(attack);

        RaiseOnStopAttack(attack);
        
        currentAttack = null;
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
    
    private void CombatManagerOnStopAttackHandler(NormalCombatManager sender, GenericAttack attack)
    {
        StopAttack(attack);
    }

    private void CombatManagerOnAttackHitHandler(GenericAttack attack, GenericIdle targetGenericIdle)
    {   
        if(!attack.CanHitMultipleTargets)
            StopAttack(attack);
        
        RaiseOnAttackHit(attack, targetGenericIdle);
    }
    
    #endregion
}
