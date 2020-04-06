using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBossAnimator : MonoBehaviour {

    #region Fields

    private Animator animator;
    private AnimationsManager animationsManager;
    private MidBossBehavior myBehaviour;
    private Stats stats;
    
    private CombatEventsManager combatEventsManager;
    private float groupAttackSpeedMultiplier = 1f;

    private NormalCombat normalCombat;
    private MidBossBehavior midBossBehaviour;
    
    #endregion

    #region Unity methods

    private void Awake() {
        animator = GetComponent<Animator>();
        animationsManager = GetComponent<AnimationsManager>();
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        myBehaviour = gameObject.GetComponent<MidBossBehavior>();
        stats = GetComponent<Stats>();
        
        normalCombat = GetComponentInChildren<NormalCombat>();
        midBossBehaviour = GetComponent<MidBossBehavior>();
    }
    
    private void OnEnable() {
        if (stats != null)
        {
            stats.onDeath += OnDeath;
        }
        
        if (normalCombat != null)
        {
            normalCombat.onStartAttack += OnStartAttack;
            normalCombat.onStopAttack += OnStopAttack;
        }

        if (midBossBehaviour != null)
        {
            midBossBehaviour.onStartIdle += OnStartIdle;
            midBossBehaviour.onStartMoving += OnStartMoving;
        }
    }

    private void OnDisable() {
        if (stats != null)
        {
            stats.onDeath -= OnDeath;
        }
        
        if (normalCombat != null)
        {
            normalCombat.onStartAttack -= OnStartAttack;
            normalCombat.onStopAttack -= OnStopAttack;
        }

        if (midBossBehaviour != null)
        {
            midBossBehaviour.onStartIdle -= OnStartIdle;
            midBossBehaviour.onStartMoving -= OnStartMoving;
        }
    }
    
    #endregion

    #region Methods

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
        animator.SetBool("isDying", false);
        animator.SetBool("isSingleAttacking", false);
        animator.SetBool("isGroupAttacking", false);
        animator.SetBool("isMoving", false);
        animator.SetBool("isIdle", false);
    }

    #endregion

    #region Event handlers

    private void OnDeath(Stats sender)
    {
        PlayDeathAnimation();
    }
    
    private void OnStartMoving()
    {
        PlayMoveAnimation();
    }

    private void OnStartIdle()
    {
        PlayIdleAnimation();
    }

    private void OnStopAttack(NormalCombat sender, Attack attack)
    {
        StopAnimations();
    }

    private void OnStartAttack(NormalCombat sender, Attack attack)
    {
        if (attack.CanHitMultipleTargets)
        {
            PlayGroupAttackAnimation();
        }
    }

    #endregion

    #region Corotuines

    private IEnumerator WaitAttackAnimation(float totalTime, float moveTimePercentage) {
        
        yield return new WaitForSeconds(totalTime * moveTimePercentage);
        // a metà attacco smetto di muovermi verso il player
        myBehaviour.CanWalk = false;

        // da quando inizia effettivamente l'attacco smetto con l'auto-tracking
        myBehaviour.FaceCRisActive = false;
        myBehaviour.CanFace = false;

        yield return new WaitForSeconds(totalTime * (1 - moveTimePercentage));
        myBehaviour.FaceCRisActive = true;
        //combatEventsManager.RaiseOnStartIdle();
        PlayIdleAnimation();
    }

    #endregion
}
