using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ImpAnimator : MonoBehaviour
{
    #region Fields

    private bool isAnimating = false;
    
    private float meleeSpeedMultiplier = 2.5f;
    private float rangedSpeedMultiplier = 2.8f;
    private float dashSpeedMultiplier = 2f;

    private CombatEventsManager combatEventsManager;
    private Reincarnation reincarnation;
    private PlayerController playerController;
    private DemonMovement demonMovement;
    private AnimationsManager animationsManager;
    private ChildrenObjectsManager childrenObjectsManager;
    private Animator animator;
    private NormalCombat normalCombat;
    private Block block;
    private Stats stats;
    private Support support;
    private Recruit recruit;
    private Dash dash;

    #endregion

    #region Properties

    public Animator Animator { get => animator; private set => animator = value; }
    public bool IsAnimating { get => isAnimating; private set => isAnimating = value; }

    #endregion
        
    #region Unity methods

    private void Awake() {
        Animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        animationsManager = GetComponent<AnimationsManager>();
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        demonMovement = gameObject.GetComponent<DemonMovement>();
        childrenObjectsManager = gameObject.GetComponent<ChildrenObjectsManager>();
        reincarnation = this.gameObject.GetComponent<Reincarnation>();
        normalCombat = gameObject.GetComponentInChildren<NormalCombat>();
        block = gameObject.GetComponentInChildren<Block>();
        stats = GetComponent<Stats>();
        support = GetComponent<Support>();
        recruit = GetComponent<Recruit>();
        dash = GetComponent<Dash>();
    }
    
    private void OnEnable() {
        dash.onDashStart += OnDashStart;
        
        if(recruit != null)
            recruit.onStartRecruit += OnStartRecruit;
        
        if(support != null)
            support.onStartSupport += OnStartSupport;
        
        demonMovement.onStartMoving += OnStartMoving;
        demonMovement.onStartIdle += OnStartIdle;

        playerController.onPlayerStartMoving += OnStartMoving;
        playerController.onPlayerEndMoving += OnStartIdle;
        
        reincarnation.onReincarnation += OnReincarnation;
        
        stats.onDeath += OnDeath;    
        
        normalCombat.onStartAttack += OnStartAttack;    
        
        block.onStartBlock += OnStartBlock;
        block.onStopBlock += OnStopBlock;
        block.onBlockSuccess += OnBlockSuccess;

        BattleEventsManager.onBattleExit += PlayIdleAnimation;
        BattleEventsManager.onBossBattleExit += PlayIdleAnimation;
    }

    private void OnDisable() {
        dash.onDashStart -= OnDashStart;
        
        if(recruit != null)
            recruit.onStartRecruit -= OnStartRecruit;
        
        if(support != null)
            support.onStartSupport -= OnStartSupport;
        
        demonMovement.onStartMoving -= OnStartMoving;
        demonMovement.onStartIdle -= OnStartIdle;

        playerController.onPlayerStartMoving -= OnStartMoving;
        playerController.onPlayerEndMoving -= OnStartIdle;
        
        reincarnation.onReincarnation -= OnReincarnation;
        
        stats.onDeath -= OnDeath;    
        
        normalCombat.onStartAttack -= OnStartAttack;    
        
        block.onStartBlock -= OnStartBlock;
        block.onStopBlock -= OnStopBlock;
        block.onBlockSuccess -= OnBlockSuccess;    
        
        BattleEventsManager.onBattleExit -= PlayIdleAnimation;
        BattleEventsManager.onBossBattleExit -= PlayIdleAnimation;
    }

    #endregion

    #region Methods

    private void PlaySingleAttackAnimation() {
        StopAnimations();
        animator.SetBool("isMeleeAttacking", true);
        StartCoroutine(WaitAnimation(animationsManager.GetAnimation("Standing Torch Melee Attack Stab").length / meleeSpeedMultiplier));
    }

    private void PlayRangedAttackAnimation() {
        StopAnimations();
        animator.SetBool("isRangedAttacking", true);
        StartCoroutine(WaitRangedAnimation(animationsManager.GetAnimation("Goalie Throw").length / rangedSpeedMultiplier));
    }

    private void PlayMoveAnimation() {
        if(!animator.GetBool("isBlocking")) {
            StopAnimations();
            animator.SetBool("isMoving", true);
        }
    }

    private void PlayIdleAnimation() {
        if(!animator.GetBool("isBlocking")) {
            StopAnimations();
            animator.SetBool("isIdle", true);
        }
    }

    private void PlayDeathAnimation() {
        StopAnimations();
        animator.SetBool("isDying", true);
    }

    private void PlayBlockAnimation() {
        StopAnimations();
        animator.SetBool("isBlocking", true);
        
    }

    private void PlaySupportAnimation() {
        StopAnimations();
        HideWeapons();
        animator.SetBool("isSupporting", true);
    }

    private void PlayRecruitAnimation() {
        StopAnimations();
        HideWeapons();
        animator.SetBool("isRecruiting", true);
    }

    private void PlayDashAnimation() {
        StopAnimations();
        animator.SetBool("isDashing", true);
        StartCoroutine(WaitAnimation(animationsManager.GetAnimation("Jump").length / dashSpeedMultiplier));
    }

    private void StopBlockAnimation() {
        // TODO - fix this, it gives wrong behaviour when dying
        if(playerController.ZMovement != 0 || playerController.XMovement != 0) {
            StopAnimations();
            //combatEventsManager.RaiseOnStartMoving();
            PlayMoveAnimation();
        }
        else {
            StopAnimations();
            //combatEventsManager.RaiseOnStartIdle();
            PlayIdleAnimation();
        }
    }
    
    private void StopAnimations() {
        ShowWeapons();
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

    // TODO - legare questi due metodi alla scelta dell'ordine e non alle loro animazioni
    private void HideWeapons() {
        childrenObjectsManager.spear.SetActive(false);
        childrenObjectsManager.shield.SetActive(false);
    }

    private void ShowWeapons() {
        childrenObjectsManager.spear.SetActive(true);
        childrenObjectsManager.shield.SetActive(true);
    }

    #endregion

    #region Events handlers

    private void OnStartIdle()
    {
        PlayIdleAnimation();
    }

    private void OnStartMoving()
    {
        PlayMoveAnimation();
    }

    private void OnStartSupport(Support sender)
    {
        PlaySupportAnimation();
    }
    
    private void OnStartRecruit(Recruit sender)
    {
        PlayRecruitAnimation();
    }

    private void OnDashStart()
    {
        PlayDashAnimation();
    }
    
    private void OnDeath(Stats stats)
    {
        PlayDeathAnimation();
    }

    private void OnReincarnation(GameObject player) { 
        StopAnimations();    
    }
    
    private void OnStartAttack(NormalCombat sender, GenericAttack attack)
    {
        if (attack.IsRanged)
        {
            PlayRangedAttackAnimation();
        }
        else
        {
            PlaySingleAttackAnimation();
        }
    }

    private void OnStartBlock(Block sender)
    {
        PlayBlockAnimation();    
    }

    private void OnStopBlock(Block sender)
    {
        StopBlockAnimation();
    }
    
    private void OnBlockSuccess(Block sender, GenericAttack genericAttack, NormalCombat attackernormalcombat)
    {
        PlayBlockAnimation();
    }

    #endregion
    
    #region Coroutines

    private IEnumerator WaitAnimation(float time) {
        if(GetComponent<Stats>().ThisUnitType == Stats.Type.Enemy) {
            IsAnimating = true;
            yield return new WaitForSeconds(time);
            IsAnimating = false;
            //combatEventsManager.RaiseOnStartIdle();
            PlayIdleAnimation();
        } else {
            IsAnimating = true;
            yield return new WaitForSeconds(time);
            IsAnimating = false;
            if(playerController.ZMovement != 0 || playerController.XMovement != 0)
                PlayMoveAnimation();
            else
                PlayIdleAnimation();
        }
        
    }

    private IEnumerator WaitRangedAnimation(float time) {
        IsAnimating = true;
        GetComponent<ChildrenObjectsManager>().spear.SetActive(false);
        yield return new WaitForSeconds(time);
        IsAnimating = false;
        GetComponent<ChildrenObjectsManager>().spear.SetActive(true);
        if(playerController.ZMovement != 0 || playerController.XMovement != 0)
            PlayMoveAnimation();
        else
            PlayIdleAnimation();
    }

    #endregion
}
