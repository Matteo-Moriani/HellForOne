using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    private bool isAnimating = false;
    private Animator animator;

    public bool IsAnimating { get => isAnimating; set => isAnimating = value; }
    public Animator Animator { get => animator; set => animator = value; }

    private CombatEventsManager combatEventsManager;

    public enum Animations
    {
        Death,
        Attack,
        Run,
        Idle
    }

    private void OnEnable()
    {
        // Exemple to explain how to use events for animations
        if(combatEventsManager != null) { 
            combatEventsManager.onMeleeAttack += PlayAttackAnimation;
            combatEventsManager.onStartSweep += PlayAttackAnimation;
            combatEventsManager.onStartGlobalAttack += PlayAttackAnimation;
        }    
    }

    private void OnDisable()
    {
        // Exemple to explain how to use events for animations
        if (combatEventsManager != null)
        {
            combatEventsManager.onMeleeAttack -= PlayAttackAnimation;
            combatEventsManager.onStartSweep -= PlayAttackAnimation;
            combatEventsManager.onStartGlobalAttack -= PlayAttackAnimation;
        }
    }

    public void PlayAnimation( Animations animation )
    {
        switch ( animation )
        {
            case Animations.Death:
                Animator.SetBool( "isDying", true );
                break;
            case Animations.Attack:
                //Animator.SetBool( "isAttacking", true );
                break;
            case Animations.Run:
                Animator.SetBool( "isRunning", true );
                break;
            case Animations.Idle:
                Animator.SetBool( "isIdle", true );
                break;
        }

        IsAnimating = true;
    }

    public void StopAnimations()
    {
        Animator.SetBool( "isDying", false );
        Animator.SetBool( "isAttacking", false );
        Animator.SetBool( "isRunning", false );
        Animator.SetBool( "isIdle", false );

        IsAnimating = false;
    }

    /*
    void Start()
    {
        Animator = GetComponent<Animator>();

        combatEventsManager = this.gameObject.GetComponent<CombatEventsManager>();
    }
    */

    // Is better to use awake in this case
    // Start is called after OnEnable
    // Awake is called before OnEnable
    // And we need to register for events in OnEnable
    private void Awake()
    {
        Animator = GetComponent<Animator>();

        combatEventsManager = this.gameObject.GetComponent<CombatEventsManager>();
    }

    // Example method to explain how to use events for animations
    public void PlayAttackAnimation() { 
        animator.SetBool("isAttacking",true);    
    }
}
