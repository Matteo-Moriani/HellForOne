using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    #region Fields

    private Stats stats;
    private Animator animator;
    private CombatEventsManager combatEventsManager;

    #endregion

    #region Properties

    public Animator Animator { get => animator; private set => animator = value; }

    #endregion

    #region Unity methods

    private void Awake()
    {
        Animator = GetComponent<Animator>();

        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();

        stats = GetComponent<Stats>();
    }
    
    private void OnEnable()
    {
        if(combatEventsManager != null) { 
            combatEventsManager.onStartSingleAttack += PlaySingleAttackAnimation;
            combatEventsManager.onStartGroupAttack += PlayGroupAttackAnimation;
            combatEventsManager.onStartGlobalAttack += PlayGlobalAttackAnimation;
            combatEventsManager.onStartIdle += PlayIdleAnimation;
            combatEventsManager.onStartMoving += PlayMoveAnimation;
            stats.onDeath += OnDeath;
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
            stats.onDeath -= OnDeath;
            combatEventsManager.onStopSingleAttack -= StopAnimations;
            combatEventsManager.onStopGroupAttack -= StopAnimations;
            combatEventsManager.onStopGlobalAttack -= StopAnimations;
        }
    }
    
    #endregion

    #region Methods

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

    #endregion

    #region Event handlers

    private void OnDeath(Stats sender)
    {
        PlayDeathAnimation();
    }

    #endregion
    
}
