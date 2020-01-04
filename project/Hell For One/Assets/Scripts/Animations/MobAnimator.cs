using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobAnimator : MonoBehaviour {

    private AnimationsManager animationsManager;
    private Animator animator;
    private Animator Animator { get => animator; set => animator = value; }
    private bool isAnimating = false;
    public bool IsAnimating { get => isAnimating; set => isAnimating = value; }

    private float meleeSpeedMultiplier = 2.5f;

    private CombatEventsManager combatEventsManager;
    private NavMeshAgent agent;

    private void OnEnable() {
        if(combatEventsManager != null) {
            combatEventsManager.onStartIdle += PlayIdleAnimation;
            combatEventsManager.onStartMoving += PlayMoveAnimation;
            combatEventsManager.onDeath += PlayDeathAnimation;
            combatEventsManager.onStartSingleAttack += PlaySingleAttackAnimation;
        }
    }

    private void OnDisable() {
        if(combatEventsManager != null) {
            combatEventsManager.onStartIdle -= PlayIdleAnimation;
            combatEventsManager.onStartMoving -= PlayMoveAnimation;
            combatEventsManager.onDeath -= PlayDeathAnimation;
            combatEventsManager.onStartSingleAttack -= PlaySingleAttackAnimation;
        }
    }

    private void Awake() {
        Animator = GetComponent<Animator>();
        animationsManager = GetComponent<AnimationsManager>();
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void PlaySingleAttackAnimation() {
        StopAnimations();
        animator.SetBool("isMeleeAttacking", true);
        StartCoroutine(WaitAnimation(animationsManager.GetAnimation("Standing Torch Melee Attack Stab").length / meleeSpeedMultiplier));
    }

    public void PlayMoveAnimation() {
        if(!animator.GetBool("isBlocking")) {
            StopAnimations();
            animator.SetBool("isMoving", true);
        }
    }

    public void PlayIdleAnimation() {
        if(!animator.GetBool("isBlocking")) {
            StopAnimations();
            animator.SetBool("isIdle", true);
        }
    }

    public void PlayDeathAnimation() {
        StopAnimations();
        animator.SetBool("isDying", true);
        StartCoroutine(WaitAnimation(animationsManager.GetAnimation("Death").length));
    }

    public IEnumerator WaitAnimation(float time) {
        if(GetComponent<Stats>().type == Stats.Type.Enemy) {
            isAnimating = true;
            yield return new WaitForSeconds(time);
            isAnimating = false;
            combatEventsManager.RaiseOnStartIdle();
        }
        else {
            isAnimating = true;
            yield return new WaitForSeconds(time);
            isAnimating = false;
            if(!animator.GetBool("isDying")) {
                if(agent.speed >= 0.1f)
                    combatEventsManager.RaiseOnStartMoving();
                else
                    combatEventsManager.RaiseOnStartIdle();
            }
        }

    }


    public void StopAnimations() {
        Animator.SetBool("isDying", false);
        Animator.SetBool("isMeleeAttacking", false);
        Animator.SetBool("isMoving", false);
        Animator.SetBool("isIdle", false);
    }
}
