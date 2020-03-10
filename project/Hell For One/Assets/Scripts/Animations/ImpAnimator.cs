using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ImpAnimator : MonoBehaviour
{
    private AnimationsManager animationsManager;

    private Animator animator;
    private Animator Animator { get => animator; set => animator = value; }
    //private GroupBehaviour groupBehaviour;

    private Controller controller;
    private DemonMovement demonMovement;
    private bool isAnimating = false;
    public bool IsAnimating { get => isAnimating; set => isAnimating = value; }

    private float meleeSpeedMultiplier = 2.5f;
    private float rangedSpeedMultiplier = 2.8f;
    private float dashSpeedMultiplier = 2f;

    private CombatEventsManager combatEventsManager;

    private void OnEnable() {
        if(combatEventsManager != null) {
            combatEventsManager.onStartIdle += PlayIdleAnimation;
            combatEventsManager.onStartMoving += PlayMoveAnimation;
            combatEventsManager.onDeath += PlayDeathAnimation;
            combatEventsManager.onStartSingleAttack += PlaySingleAttackAnimation;
            combatEventsManager.onStartRangedAttack += PlayRangedAttackAnimation;
            combatEventsManager.onStartBlock += PlayBlockAnimation;
            combatEventsManager.onStopBlock += StopBlockAnimation;
            combatEventsManager.onStartSupport += PlaySupportAnimation;
            combatEventsManager.onStartRecruit += PlayRecruitAnimation;
            combatEventsManager.onStartDash += PlayDashAnimation;
            BattleEventsManager.onBattleExit += PlayIdleAnimation;
            BattleEventsManager.onBossBattleExit += PlayIdleAnimation;
            combatEventsManager.onReincarnation += StopBlockAnimation;
        }
    }

    private void OnDisable() {
        if(combatEventsManager != null) {
            combatEventsManager.onStartIdle -= PlayIdleAnimation;
            combatEventsManager.onStartMoving -= PlayMoveAnimation;
            combatEventsManager.onDeath -= PlayDeathAnimation;
            combatEventsManager.onStartSingleAttack -= PlaySingleAttackAnimation;
            combatEventsManager.onStartRangedAttack -= PlayRangedAttackAnimation;
            combatEventsManager.onStartBlock -= PlayBlockAnimation;
            combatEventsManager.onStopBlock -= StopBlockAnimation;
            combatEventsManager.onStartSupport -= PlaySupportAnimation;
            combatEventsManager.onStartRecruit -= PlayRecruitAnimation;
            combatEventsManager.onStartDash -= PlayDashAnimation;
            BattleEventsManager.onBattleExit -= PlayIdleAnimation;
            BattleEventsManager.onBossBattleExit -= PlayIdleAnimation;
            combatEventsManager.onReincarnation -= StopBlockAnimation;
        }
    }

    private void Awake() {
        Animator = GetComponent<Animator>();
        controller = GetComponent<Controller>();
        animationsManager = GetComponent<AnimationsManager>();
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        demonMovement = gameObject.GetComponent<DemonMovement>();
        //groupBehaviour = gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupBehaviour>();
    }

    public void PlaySingleAttackAnimation() {
        StopAnimations();
        animator.SetBool("isMeleeAttacking", true);
        StartCoroutine(WaitAnimation(animationsManager.GetAnimation("Standing Torch Melee Attack Stab").length / meleeSpeedMultiplier));
    }

    public void PlayRangedAttackAnimation() {
        StopAnimations();
        animator.SetBool("isRangedAttacking", true);
        StartCoroutine(WaitRangedAnimation(animationsManager.GetAnimation("Goalie Throw").length / rangedSpeedMultiplier));
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
    }

    public void PlayBlockAnimation() {
        StopAnimations();
        animator.SetBool("isBlocking", true);
        
    }

    public void PlaySupportAnimation() {
        StopAnimations();
        animator.SetBool("isSupporting", true);
    }

    public void PlayRecruitAnimation() {
        StopAnimations();
        animator.SetBool("isRecruiting", true);
    }

    public void PlayDashAnimation() {
        StopAnimations();
        animator.SetBool("isDashing", true);
        StartCoroutine(WaitAnimation(animationsManager.GetAnimation("Jump").length / dashSpeedMultiplier));
    }

    public void StopBlockAnimation() {
        if(controller.ZMovement != 0 || controller.XMovement != 0) {
            StopAnimations();
            combatEventsManager.RaiseOnStartMoving();
        }
        else {
            StopAnimations();
            combatEventsManager.RaiseOnStartIdle();
        }
    }

    public IEnumerator WaitAnimation(float time) {
        if(GetComponent<Stats>().ThisUnitType == Stats.Type.Enemy) {
            IsAnimating = true;
            yield return new WaitForSeconds(time);
            IsAnimating = false;
            combatEventsManager.RaiseOnStartIdle();
        } else {
            IsAnimating = true;
            yield return new WaitForSeconds(time);
            IsAnimating = false;
            if(controller.ZMovement != 0 || controller.XMovement != 0)
                combatEventsManager.RaiseOnStartMoving();
            else
                combatEventsManager.RaiseOnStartIdle();
        }
        
    }

    public IEnumerator WaitRangedAnimation(float time) {
        IsAnimating = true;
        GetComponent<ChildrenObjectsManager>().spear.SetActive(false);
        yield return new WaitForSeconds(time);
        IsAnimating = false;
        GetComponent<ChildrenObjectsManager>().spear.SetActive(true);
        if(controller.ZMovement != 0 || controller.XMovement != 0)
            combatEventsManager.RaiseOnStartMoving();
        else
            combatEventsManager.RaiseOnStartIdle();
    }


    public void StopAnimations() {
        Animator.SetBool("isDying", false);
        Animator.SetBool("isMeleeAttacking", false);
        Animator.SetBool("isRangedAttacking", false);
        Animator.SetBool("isMoving", false);
        Animator.SetBool("isIdle", false);
        Animator.SetBool("isBlocking", false);
        Animator.SetBool("isSupporting", false);
        Animator.SetBool("isDashing", false);
        Animator.SetBool("isRecruiting", false);
    }
}
