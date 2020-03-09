using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBossAnimator : MonoBehaviour {
    private Animator animator;
    private AnimationsManager animationsManager;
    private MidBossBehavior myBehaviour;

    public Animator Animator { get => animator; set => animator = value; }
    int i = 0;
    

    private CombatEventsManager combatEventsManager;
    private float groupAttackSpeedMultiplier = 1f;

    private void OnEnable() {
        if(combatEventsManager != null) {
            combatEventsManager.onStartSingleAttack += PlaySingleAttackAnimation;
            combatEventsManager.onStartGroupAttack += PlayGroupAttackAnimation;
            combatEventsManager.onStartIdle += PlayIdleAnimation;
            combatEventsManager.onStartMoving += PlayMoveAnimation;
            combatEventsManager.onDeath += PlayDeathAnimation;
            combatEventsManager.onStopSingleAttack += StopAnimations;
            combatEventsManager.onStopGroupAttack += StopAnimations;
        }
    }

    private void OnDisable() {
        if(combatEventsManager != null) {
            combatEventsManager.onStartSingleAttack -= PlaySingleAttackAnimation;
            combatEventsManager.onStartGroupAttack -= PlayGroupAttackAnimation;
            combatEventsManager.onStartIdle -= PlayIdleAnimation;
            combatEventsManager.onStartMoving -= PlayMoveAnimation;
            combatEventsManager.onDeath -= PlayDeathAnimation;
            combatEventsManager.onStopSingleAttack -= StopAnimations;
            combatEventsManager.onStopGroupAttack -= StopAnimations;
        }
    }
    
    private void Awake() {
        Animator = GetComponent<Animator>();
        animationsManager = GetComponent<AnimationsManager>();
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        myBehaviour = gameObject.GetComponent<MidBossBehavior>();
    }
    
    public void PlaySingleAttackAnimation() {
        StopAnimations();
        animator.SetBool("isSingleAttacking", true);
    }

    public void PlayGroupAttackAnimation() {
        StopAnimations();
        animator.SetBool("isGroupAttacking", true);
        StartCoroutine(WaitAttackAnimation(animationsManager.GetAnimation("GroupAttack").length / groupAttackSpeedMultiplier, 0.4f));
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
        Animator.SetBool("isMoving", false);
        Animator.SetBool("isIdle", false);
    }

    public IEnumerator WaitAttackAnimation(float totalTime, float moveTimePercentage) {
        
        yield return new WaitForSeconds(totalTime * moveTimePercentage);
        // a metà attacco smetto di muovermi verso il player
        myBehaviour.CanWalk = false;

        // da quando inizia effettivamente l'attacco smetto con l'auto-tracking
        myBehaviour.FaceCRisActive = false;
        myBehaviour.CanFace = false;

        yield return new WaitForSeconds(totalTime * (1 - moveTimePercentage));
        myBehaviour.FaceCRisActive = true;
        combatEventsManager.RaiseOnStartIdle();
    }
}
