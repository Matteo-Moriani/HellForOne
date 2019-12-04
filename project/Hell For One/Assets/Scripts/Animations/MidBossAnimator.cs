using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBossAnimator : MonoBehaviour {
    private Animator animator;
    public Animator Animator { get => animator; set => animator = value; }

    private CombatEventsManager combatEventsManager;

    private void OnEnable() {
        // Exemple to explain how to use events for animations
        if(combatEventsManager != null) {
            combatEventsManager.onStartSingleAttack += PlaySingleAttackAnimation;
            combatEventsManager.onStartGroupAttack += PlayGroupAttackAnimation;
            combatEventsManager.onStartIdle += PlayIdleAnimation;
            combatEventsManager.onStartRunning += PlayRunAnimation;
            combatEventsManager.onDeath += PlayDeathAnimation;
            combatEventsManager.onStopSingleAttack += StopAnimations;
            combatEventsManager.onStopGroupAttack += StopAnimations;

            //combatEventsManager.onStopAnimation += StopAnimations;
        }
    }

    private void OnDisable() {
        // Exemple to explain how to use events for animations
        if(combatEventsManager != null) {
            combatEventsManager.onStartSingleAttack -= PlaySingleAttackAnimation;
            combatEventsManager.onStartGroupAttack -= PlayGroupAttackAnimation;
            combatEventsManager.onStartIdle -= PlayIdleAnimation;
            combatEventsManager.onStartRunning -= PlayRunAnimation;
            combatEventsManager.onDeath -= PlayDeathAnimation;
            combatEventsManager.onStopSingleAttack -= StopAnimations;
            combatEventsManager.onStopGroupAttack -= StopAnimations;
            // End mods

            //combatEventsManager.onStopAnimation -= StopAnimations;
        }
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
    private void Awake() {
        Animator = GetComponent<Animator>();

        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
    }

    // Example method to explain how to use events for animations
    public void PlaySingleAttackAnimation() {
        StopAnimations();
        animator.SetBool("isSingleAttacking", true);
    }

    public void PlayGroupAttackAnimation() {
        StopAnimations();
        animator.SetBool("isGroupAttacking", true);
    }

    public void PlayRunAnimation() {
        StopAnimations();
        animator.SetBool("isRunning", true);
    }

    public void PlayIdleAnimation() {
        StopAnimations();
        animator.SetBool("isIdle", true);
    }

    public void PlayDeathAnimation() {
        StopAnimations();
        animator.SetBool("isDying", true);
        //        AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(0); // get next state on layer 0

        //        if(nextState.IsName("Death"))
        //{
        //            animator.SetBool("isDying", true);
        //        }
    }


    public void StopAnimations() {
        Animator.SetBool("isDying", false);
        Animator.SetBool("isSingleAttacking", false);
        Animator.SetBool("isGroupAttacking", false);
        Animator.SetBool("isRunning", false);
        Animator.SetBool("isIdle", false);
    }
}
