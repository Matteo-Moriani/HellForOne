using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    private bool isAnimating = false;
    private Animator animator;
    public bool IsAnimating { get => isAnimating; set => isAnimating = value; }
    public Animator Animator { get => animator; set => animator = value; }

    private CombatEventsManager combatEventsManager;

    public enum Animations
    {
        Death,
        Attack,
        Run,
        Idle
    }

    private void OnEnable()
    {
        // Exemple to explain how to use events for animations
        if(combatEventsManager != null) { 
            combatEventsManager.onStartSingleAttack += PlayAttackAnimation;
            combatEventsManager.onStartGroupAttack += PlayAttackAnimation;
            combatEventsManager.onStartGlobalAttack += PlayAttackAnimation;
            combatEventsManager.onStartIdle += PlayIdleAnimation;
            combatEventsManager.onStartRunning += PlayRunAnimation;
            combatEventsManager.onDeath += PlayDeathAnimation;
            combatEventsManager.onStopSingleAttack += StopAnimations;
            combatEventsManager.onStopAnimation += StopAnimations;
        }    
    }

    private void OnDisable()
    {
        // Exemple to explain how to use events for animations
        if (combatEventsManager != null)
        {
            combatEventsManager.onStartSingleAttack -= PlayAttackAnimation;
            combatEventsManager.onStartGroupAttack -= PlayAttackAnimation;
            combatEventsManager.onStartGlobalAttack -= PlayAttackAnimation;
            combatEventsManager.onStartIdle -= PlayIdleAnimation;
            combatEventsManager.onStartRunning -= PlayRunAnimation;
            combatEventsManager.onDeath -= PlayDeathAnimation;
            combatEventsManager.onStopSingleAttack -= StopAnimations;
            combatEventsManager.onStopAnimation -= StopAnimations;
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
    private void Awake()
    {
        Animator = GetComponent<Animator>();

        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
    }

    // Example method to explain how to use events for animations
    public void PlayAttackAnimation() { 
        animator.SetBool("isAttacking",true);    
    }

    public void PlayRunAnimation() {
        animator.SetBool("isRunning", true);
    }

    public void PlayIdleAnimation() {
        animator.SetBool("isIdle", true);
    }

    public void PlayDeathAnimation() {
        animator.SetBool("isDying", true);
    }

    public void StopAnimations() {
        Animator.SetBool("isDying", false);
        Animator.SetBool("isAttacking", false);
        Animator.SetBool("isRunning", false);
        Animator.SetBool("isIdle", false);

        IsAnimating = false;
    }
}
