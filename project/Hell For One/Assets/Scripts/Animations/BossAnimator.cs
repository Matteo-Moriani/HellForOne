using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    #region Fields

    private Stats stats;
    private Animator animator;
    private CombatEventsManager combatEventsManager;
    private bool moving = false;

    #endregion

    #region Unity methods

    private void Awake()
    {
        animator = GetComponent<Animator>();
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        stats = GetComponent<Stats>();
    }
    
    private void OnEnable()
    {
        if(combatEventsManager != null) { 
            combatEventsManager.onStartSingleAttack += PlaySingleAttackAnimation;
            combatEventsManager.onStartGroupAttack += PlayGroupAttackAnimation;
            combatEventsManager.onStartGlobalAttack += PlayGlobalAttackAnimation;
            combatEventsManager.onStopMoving += OnStopMoving;
            combatEventsManager.onStartMoving += OnStartMoving;
            stats.onDeath += OnDeath;
            combatEventsManager.onStopSingleAttack += SetAllBoolsToFalse;
            combatEventsManager.onStopGroupAttack += SetAllBoolsToFalse;
            combatEventsManager.onStopGlobalAttack += SetAllBoolsToFalse;
        }
    }

    private void OnDisable()
    {
        if (combatEventsManager != null)
        {
            combatEventsManager.onStartSingleAttack -= PlaySingleAttackAnimation;
            combatEventsManager.onStartGroupAttack -= PlayGroupAttackAnimation;
            combatEventsManager.onStartGlobalAttack -= PlayGlobalAttackAnimation;
            combatEventsManager.onStopMoving -= OnStopMoving;
            combatEventsManager.onStartMoving -= OnStartMoving;
            stats.onDeath -= OnDeath;
            combatEventsManager.onStopSingleAttack -= SetAllBoolsToFalse;
            combatEventsManager.onStopGroupAttack -= SetAllBoolsToFalse;
            combatEventsManager.onStopGlobalAttack -= SetAllBoolsToFalse;
        }
    }

    private void Update()
    {
        if(moving)
            PlayMoveAnimation();
        else
            SetAllBoolsToFalse();
    }

    #endregion

    #region Methods

    public void PlaySingleAttackAnimation() {
        SetAllBoolsToFalse();
        animator.SetTrigger("singleAttack");    
    }

    public void PlayGroupAttackAnimation() {
        SetAllBoolsToFalse();
        animator.SetTrigger("groupAttack");
    }
    
    public void PlayGlobalAttackAnimation() {
        SetAllBoolsToFalse();
        animator.SetTrigger("globalAttack");
    }

    public void PlayMoveAnimation() {
        SetAllBoolsToFalse();
        animator.SetBool("isMoving", true);
    }

    public void PlayDeathAnimation() {
        SetAllBoolsToFalse();
        animator.SetTrigger("death");
    }

    public void SetAllBoolsToFalse() {
        animator.SetBool("isMoving", false);
    }

    #endregion

    #region Event handlers

    private void OnDeath(Stats sender)
    {
        PlayDeathAnimation();
    }

    private void OnStartMoving()
    {
        moving = true;
    }

    private void OnStopMoving()
    {
        moving = false;
    }

    #endregion

}
