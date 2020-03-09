using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    private Animator animator;
    public Animator Animator { get => animator; set => animator = value; }

    private CombatEventsManager combatEventsManager;

    private void OnEnable()
    {
        if(combatEventsManager != null) { 
            combatEventsManager.onStartSingleAttack += PlaySingleAttackAnimation;
            combatEventsManager.onStartGroupAttack += PlayGroupAttackAnimation;
            combatEventsManager.onStartGlobalAttack += PlayGlobalAttackAnimation;
            combatEventsManager.onStartIdle += PlayIdleAnimation;
            combatEventsManager.onStartMoving += PlayMoveAnimation;
            combatEventsManager.onDeath += PlayDeathAnimation;
            combatEventsManager.onStopSingleAttack += StopAnimations;
            combatEventsManager.onStopGroupAttack += StopAnimations;
            combatEventsManager.onStopGlobalAttack += StopAnimations;
        }
    }

    private void OnDisable()
    {
        if (combatEventsManager != null)
        {
            combatEventsManager.onStartSingleAttack -= PlaySingleAttackAnimation;
            combatEventsManager.onStartGroupAttack -= PlayGroupAttackAnimation;
            combatEventsManager.onStartGlobalAttack -= PlayGlobalAttackAnimation;
            combatEventsManager.onStartIdle -= PlayIdleAnimation;
            combatEventsManager.onStartMoving -= PlayMoveAnimation;
            combatEventsManager.onDeath -= PlayDeathAnimation;
            combatEventsManager.onStopSingleAttack -= StopAnimations;
            combatEventsManager.onStopGroupAttack -= StopAnimations;
            combatEventsManager.onStopGlobalAttack -= StopAnimations;
        }
    }
    
    private void Awake()
    {
        Animator = GetComponent<Animator>();

        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
    }
    
    public void PlaySingleAttackAnimation() {
        StopAnimations();
        animator.SetBool("isSingleAttacking",true);    
    }

    public void PlayGroupAttackAnimation() {
        StopAnimations();
        animator.SetBool("isGroupAttacking", true);
    }
    
    public void PlayGlobalAttackAnimation() {
        StopAnimations();
        animator.SetBool("isGlobalAttacking", true);
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


    public void StopAnimations() {
        Animator.SetBool("isDying", false);
        Animator.SetBool("isSingleAttacking", false);
        Animator.SetBool("isGroupAttacking", false);
        Animator.SetBool("isGlobalAttacking", false);
        Animator.SetBool("isMoving", false);
        Animator.SetBool("isIdle", false);
    }
}
