using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobAnimator : MonoBehaviour {

    #region Fields

    private AnimationsManager animationsManager;
    private Animator animator;
    private Stats stats;
    
    private bool isAnimating = false;
    
    private float meleeSpeedMultiplier = 2.5f;

    private CombatEventsManager combatEventsManager;
    private NavMeshAgent agent;
    
    #endregion

    #region Unity methods

    private void Awake() {
        animator = GetComponent<Animator>();
        animationsManager = GetComponent<AnimationsManager>();
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<Stats>();
    }
    
    private void OnEnable() {
        if(combatEventsManager != null) {
            combatEventsManager.onStartIdle += PlayIdleAnimation;
            combatEventsManager.onStartMoving += PlayMoveAnimation;
            stats.onDeath += OnDeath;
            combatEventsManager.onStartSingleAttack += PlaySingleAttackAnimation;
        }
    }

    private void OnDisable() {
        if(combatEventsManager != null) {
            combatEventsManager.onStartIdle -= PlayIdleAnimation;
            combatEventsManager.onStartMoving -= PlayMoveAnimation;
            stats.onDeath -= OnDeath;
            combatEventsManager.onStartSingleAttack -= PlaySingleAttackAnimation;
        }
    }
    
    #endregion

    #region Methods

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
    
    public void StopAnimations() {
        animator.SetBool("isDying", false);
        animator.SetBool("isMeleeAttacking", false);
        animator.SetBool("isMoving", false);
        animator.SetBool("isIdle", false);
    }

    #endregion

    #region Event handlers

    private void OnDeath(Stats sender)
    {
        PlayDeathAnimation();
    }

    #endregion

    #region Coroutines

    public IEnumerator WaitAnimation(float time) {
        if(GetComponent<Stats>().ThisUnitType == Stats.Type.Enemy) {
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

    #endregion
}
