using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpAnimator : MonoBehaviour
{
    private Animator animator;
    public Animator Animator { get => animator; set => animator = value; }

    private CombatEventsManager combatEventsManager;

    private void OnEnable() {
        if(combatEventsManager != null) {
            combatEventsManager.onStartIdle += PlayIdleAnimation;
            combatEventsManager.onStartMoving += PlayMoveAnimation;
            combatEventsManager.onDeath += PlayDeathAnimation;
            combatEventsManager.onStartSingleAttack += PlaySingleAttackAnimation;
            combatEventsManager.onStopSingleAttack += StopAnimations;
            combatEventsManager.onStartRangedAttack += PlayRangedAttackAnimation;
            combatEventsManager.onStartBlock += PlayBlockAnimation;
            combatEventsManager.onStartSupport += PlaySupportAnimation;
            combatEventsManager.onStartDash += PlayDashAnimation;
        }
    }

    private void OnDisable() {
        if(combatEventsManager != null) {
            combatEventsManager.onStartIdle -= PlayIdleAnimation;
            combatEventsManager.onStartMoving -= PlayMoveAnimation;
            combatEventsManager.onDeath -= PlayDeathAnimation;
            combatEventsManager.onStopSingleAttack -= StopAnimations;
            combatEventsManager.onStartSingleAttack -= PlaySingleAttackAnimation;
            combatEventsManager.onStartRangedAttack -= PlayRangedAttackAnimation;
            combatEventsManager.onStartBlock -= PlayBlockAnimation;
            combatEventsManager.onStartSupport -= PlaySupportAnimation;
            combatEventsManager.onStartDash -= PlayDashAnimation;
        }
    }

    private void Awake() {
        Animator = GetComponent<Animator>();

        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
    }

    public void PlaySingleAttackAnimation() {
        StopAnimations();
        animator.SetBool("isMeleeAttacking", true);
    }

    public void PlayRangedAttackAnimation() {
        StopAnimations();
        animator.SetBool("isRangedAttacking", true);
    }

    public void PlayMoveAnimation() {
        StopAnimations();
        animator.SetBool("isMoving", true);
    }

    public void PlayIdleAnimation() {
        StopAnimations();
        animator.SetBool("isIdle", true);
    }

    public void PlayDeathAnimation() {
        StopAnimations();
        animator.SetBool("isDying", true);
    }

    public void PlayBlockAnimation() {
        StopAnimations();
        animator.SetBool("isBlocking", true);
    }

    public void PlaySupportAnimation() {
        StopAnimations();
        animator.SetBool("isSupporting", true);
    }

    public void PlayDashAnimation() {
        StopAnimations();
        animator.SetBool("isDashing", true);
    }


    public void StopAnimations() {
        Animator.SetBool("isDying", false);
        Animator.SetBool("isMeleeAttacking", false);
        Animator.SetBool("isRangedAttacking", false);
        Animator.SetBool("isMoving", false);
        Animator.SetBool("isIdle", false);
        Animator.SetBool("isBlocking", false);
        Animator.SetBool("isSupporting", false);
    }
}
