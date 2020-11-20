using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    #region Fields

    private Animator animator;
    private BossBehavior myBehaviour;
    private Stats stats;
    private CombatEventsManager combatEventsManager;
    private NormalCombat normalCombat;
    private BossBehavior bossBehaviour;
    private bool moving = false;

    #endregion

    #region Unity methods

    private void Awake()
    {
        animator = GetComponent<Animator>();
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        myBehaviour = gameObject.GetComponent<BossBehavior>();
        stats = GetComponent<Stats>();

        normalCombat = GetComponentInChildren<NormalCombat>();
        bossBehaviour = GetComponent<BossBehavior>();
    }

    private void OnEnable()
    {
        if(stats != null)
        {
            stats.onDeath += OnDeath;
        }

        if(normalCombat != null)
        {
            normalCombat.onStartAttack += OnStartAttack;
        }

        if(bossBehaviour != null)
        {
            bossBehaviour.onStartMoving += OnStartMoving;
            bossBehaviour.onStopMoving += OnStopMoving;
        }
    }

    private void OnDisable()
    {
        if(stats != null)
        {
            stats.onDeath -= OnDeath;
        }

        if(normalCombat != null)
        {
            normalCombat.onStartAttack -= OnStartAttack;
        }

        if(bossBehaviour != null)
        {
            bossBehaviour.onStartMoving -= OnStartMoving;
            bossBehaviour.onStopMoving -= OnStopMoving;
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

    public void PlayGlobalAttackAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetTrigger("globalAttack");
    }

    public void PlayDeathAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetTrigger("death");
    }

    public void PlayMoveAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetBool("isMoving", true);
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
        switch(attack.name)
        {
            case "BossNormalAttackSwipe":
                PlaySingleAttackAnimation();
                break;
            case "FlameCircle":
                PlayGroupAttackAnimation();
                break;
            case "FlameExplosion":
                PlayGlobalAttackAnimation();
                break;
            default:
                break;
        }        
    }

    #endregion
}
