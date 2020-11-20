using UnityEngine;

public class MidBossAnimator : MonoBehaviour {

    #region Fields

    private Animator animator;
    private MidBossBehavior myBehaviour;
    private Stats stats;
    private CombatEventsManager combatEventsManager;
    private NormalCombat normalCombat;
    private MidBossBehavior midBossBehaviour;
    private bool moving = false;
    
    #endregion

    #region Unity methods

    private void Awake() {
        animator = GetComponent<Animator>();
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        myBehaviour = gameObject.GetComponent<MidBossBehavior>();
        stats = GetComponent<Stats>();
        
        normalCombat = GetComponentInChildren<NormalCombat>();
        midBossBehaviour = GetComponent<MidBossBehavior>();
    }
    
    private void OnEnable() {
        if (stats != null)
        {
            stats.onDeath += OnDeath;
        }
        
        if (normalCombat != null)
        {
            normalCombat.onStartAttack += OnStartAttack;
        }

        if (midBossBehaviour != null)
        {
            midBossBehaviour.onStartMoving += OnStartMoving;
            midBossBehaviour.onStopMoving += OnStopMoving;
        }
    }

    private void OnDisable() {
        if (stats != null)
        {
            stats.onDeath -= OnDeath;
        }
        
        if (normalCombat != null)
        {
            normalCombat.onStartAttack -= OnStartAttack;
        }

        if (midBossBehaviour != null)
        {
            midBossBehaviour.onStartMoving -= OnStartMoving;
            midBossBehaviour.onStopMoving -= OnStopMoving;
        }
    }

    private void Update()
    {
        if(moving)
            PlayMoveAnimation();
        else
            SetAllBoolsToFalse();
    }

    #endregion

    #region Methods

    public void PlaySingleAttackAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetTrigger("singleAttack");
    }

    public void PlayGroupAttackAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetTrigger("groupAttack");
    }

    public void PlayMoveAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetBool("isMoving", true);
    }

    public void PlayDeathAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetTrigger("death");
    }

    public void SetAllBoolsToFalse()
    {
        animator.SetBool("isMoving", false);
    }

    #endregion

    #region Event handlers

    private void OnDeath(Stats sender)
    {
        PlayDeathAnimation();
    }
    
    private void OnStartMoving()
    {
        moving = true;
    }

    private void OnStopMoving()
    {
        moving = false;
    }

    private void OnStartAttack(NormalCombat sender, GenericAttack attack)
    {
        if (attack.CanHitMultipleTargets)
        {
            PlayGroupAttackAnimation();
        }
    }

    #endregion
}
