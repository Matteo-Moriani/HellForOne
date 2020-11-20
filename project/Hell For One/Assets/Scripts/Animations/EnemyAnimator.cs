using UnityEngine;

public abstract class EnemyAnimator : MonoBehaviour
{
    #region Fields

    private protected Animator animator;
    private AbstractBoss myBehaviour;
    private Stats stats;
    private CombatEventsManager combatEventsManager;
    private NormalCombat normalCombat;
    private StunReceiver stunReceiver;
    private bool moving = false;
    private bool stunned = false;

    #endregion

    #region Unity methods

    private void Awake()
    {
        animator = GetComponent<Animator>();
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        myBehaviour = gameObject.GetComponent<AbstractBoss>();
        stats = GetComponent<Stats>();
        stunReceiver = GetComponentInChildren<StunReceiver>();
        normalCombat = GetComponentInChildren<NormalCombat>();
    }

    private void OnEnable()
    {
        // TODO - registrare OnPain all'evento appropriato e creare da qualche parte il metodo di fine Pain che verrà richiamato come animation event a fine animazione

        if(stats != null)
        {
            stats.onDeath += OnDeath;
        }

        if(normalCombat != null)
        {
            normalCombat.onStartAttack += OnStartAttack;
        }

        if(myBehaviour != null)
        {
            myBehaviour.onStartMoving += OnStartMoving;
            myBehaviour.onStopMoving += OnStopMoving;
        }

        if(stunReceiver != null)
        {
            stunReceiver.onStartStun += OnStartStun;
            stunReceiver.onStopStun += OnStopStun;
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

        if(myBehaviour != null)
        {
            myBehaviour.onStartMoving -= OnStartMoving;
            myBehaviour.onStopMoving -= OnStopMoving;
        }

        if(stunReceiver != null)
        {
            stunReceiver.onStartStun -= OnStartStun;
            stunReceiver.onStopStun -= OnStopStun;
        }
    }

    private void Update()
    {
        if(moving)
            PlayMoveAnimation();
        else if(stunned)
            PlayStunAnimation();
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

    public void PlayDeathAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetTrigger("death");
    }

    public void PlayPainAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetTrigger("pain");
    }

    public void PlayMoveAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetBool("isMoving", true);
    }

    public void PlayStunAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetBool("isStunned", true);
    }

    public void SetAllBoolsToFalse()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isStunned", false);
    }

    #endregion

    #region Event handlers

    private void OnDeath(Stats sender)
    {
        PlayDeathAnimation();
    }

    private void OnPain()
    {
        PlayPainAnimation();
    }

    private void OnStartMoving()
    {
        moving = true;
    }

    private void OnStopMoving()
    {
        moving = false;
    }

    private void OnStartStun()
    {
        stunned = true;
    }

    private void OnStopStun()
    {
        stunned = false;
    }

    private protected abstract void OnStartAttack(NormalCombat sender, GenericAttack attack);

    #endregion
}
