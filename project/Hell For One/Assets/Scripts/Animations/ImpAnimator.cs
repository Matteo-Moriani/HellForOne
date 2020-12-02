using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ImpAnimator : MonoBehaviour
{
    #region Fields

    private bool isAnimating = false;

    private CombatEventsManager combatEventsManager;
    private Reincarnation reincarnation;
    private PlayerController playerController;
    private AllyImpMovement allyImpMovement;
    private ChildrenObjectsManager childrenObjectsManager;
    private Animator animator;
    private NormalCombat normalCombat;
    private Block block;
    private Stats stats;
    private Support support;
    private Recruit recruit;
    private Dash dash;
    private PlayerScriptedMovements playerScriptedMovements;
    private bool isBlocking = false;
    private bool isRecruiting = false;
    private bool playerScriptedMovement = false;
    private bool allyIsMoving = false;

    #endregion

    #region Properties

    public Animator Animator { get => animator; private set => animator = value; }
    public bool IsAnimating { get => isAnimating; private set => isAnimating = value; }

    #endregion
        
    #region Unity methods

    private void Awake() {
        Animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        allyImpMovement = gameObject.GetComponent<AllyImpMovement>();
        childrenObjectsManager = gameObject.GetComponent<ChildrenObjectsManager>();
        reincarnation = gameObject.GetComponent<Reincarnation>();
        normalCombat = gameObject.GetComponentInChildren<NormalCombat>();
        block = gameObject.GetComponentInChildren<Block>();
        stats = GetComponent<Stats>();
        support = GetComponent<Support>();
        recruit = GetComponent<Recruit>();
        dash = GetComponent<Dash>();
        playerScriptedMovements = GetComponent<PlayerScriptedMovements>();
    }
    
    private void OnEnable() {
        
        if(recruit != null)
        {
            recruit.onStartRecruit += OnStartRecruit;
            recruit.onStopRecruit += OnStopRecruit;
        } 

        dash.onDashStart += OnDashStart;
        allyImpMovement.onStartMoving += OnAllyMovementStart;
        allyImpMovement.onStopMoving += OnAllyMovementEnd;
        reincarnation.onReincarnation += OnReincarnation;
        stats.onDeath += OnDeath;    
        normalCombat.onStartAttack += OnStartAttack;  
        block.onStartBlock += OnStartBlock;
        block.onStopBlock += OnStopBlock;
        block.onBlockSuccess += OnBlockSuccess;
        BattleEventsManager.onBattleExit += SetAllBoolsToFalse;
        playerScriptedMovements.OnScriptedMovementStart += OnScriptedMovementStart;
        playerScriptedMovements.OnScriptedMovementEnd += OnScriptedMovementEnd;
    }

    private void OnDisable() {

        if(recruit != null)
        {
            recruit.onStartRecruit -= OnStartRecruit;
            recruit.onStopRecruit -= OnStopRecruit;
        }

        dash.onDashStart -= OnDashStart;
        allyImpMovement.onStartMoving -= OnAllyMovementStart;
        allyImpMovement.onStopMoving -= OnAllyMovementEnd;
        reincarnation.onReincarnation -= OnReincarnation;
        stats.onDeath -= OnDeath;    
        normalCombat.onStartAttack -= OnStartAttack;    
        block.onStartBlock -= OnStartBlock;
        block.onStopBlock -= OnStopBlock;
        block.onBlockSuccess -= OnBlockSuccess;    
        BattleEventsManager.onBattleExit -= SetAllBoolsToFalse;
        playerScriptedMovements.OnScriptedMovementStart -= OnScriptedMovementStart;
        playerScriptedMovements.OnScriptedMovementEnd -= OnScriptedMovementEnd;
    }

    private void Update()
    {
        // ordered by priority
        if(isBlocking)
            PlayBlockAnimation();
        else if(playerController.ZMovement != 0f || playerController.XMovement != 0f || playerScriptedMovement || (allyIsMoving && stats.ThisUnitType == Stats.Type.Ally))
            PlayMoveAnimation();
        else if(isRecruiting)
            PlayRecruitAnimation();
        else
            SetAllBoolsToFalse();
    }

    #endregion

    #region Methods

    // loops: they are booleans
    private void PlayMoveAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetBool("isMoving", true);
    }

    private void PlayBlockAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetBool("isBlocking", true);
    }

    private void PlayRecruitAnimation()
    {
        SetAllBoolsToFalse();
        HideWeapons();
        animator.SetBool("isRecruiting", true);
    }

    // single actions: they are triggers and they all start from idle and end to idle
    private void PlaySingleAttackAnimation() {
        SetAllBoolsToFalse();
        animator.SetTrigger("meleeAttack");
    }

    private void PlayRangedAttackAnimation() {
        SetAllBoolsToFalse();
        animator.SetTrigger("rangedAttack");
    }

    private void PlayDashAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetTrigger("dash");
    }

    private void PlayDeathAnimation() {
        SetAllBoolsToFalse();
        animator.SetTrigger("death");
    }

    private void PlayParryAnimation()
    {
        SetAllBoolsToFalse();
        animator.SetTrigger("parry");
    }

    private void StopBlockAnimation() {
        // TODO - fix this, it gives wrong behaviour when dying
        if(playerController.ZMovement != 0 || playerController.XMovement != 0) {
            SetAllBoolsToFalse();
            //combatEventsManager.RaiseOnStartMoving();
            PlayMoveAnimation();
        }
        else {
            SetAllBoolsToFalse();
        }
    }
    
    private void SetAllBoolsToFalse() {
        ShowWeapons();
        Animator.SetBool("isMoving", false);
        Animator.SetBool("isBlocking", false);
        Animator.SetBool("isRecruiting", false);
    }

    // TODO - legare questi due metodi alla scelta dell'ordine e non alle loro animazioni
    private void HideWeapons() {
        if(gameObject.tag != "Player")
        {
            childrenObjectsManager.spear.SetActive(false);
            childrenObjectsManager.shield.SetActive(false);
        }
    }

    private void ShowWeapons() {
        if(gameObject.tag != "Player")
        {
            childrenObjectsManager.spear.SetActive(true);
            childrenObjectsManager.shield.SetActive(true);
        }
    }

    #endregion

    #region Events handlers

    private void OnDashStart()
    {
        PlayDashAnimation();
    }
    
    private void OnDeath(Stats stats)
    {
        PlayDeathAnimation();
    }

    private void OnReincarnation(GameObject player) { 
        SetAllBoolsToFalse();    
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
        isBlocking = true;  
    }

    private void OnStopBlock(Block sender)
    {
        isBlocking = false;
    }
    
    private void OnBlockSuccess(Block sender, GenericAttack genericAttack, NormalCombat attackernormalcombat)
    {
        // no need to do this if i'm blocking in the animation
        if(!isBlocking)
            PlayParryAnimation();
    }

    private void OnStartRecruit(Recruit sender)
    {
        isRecruiting = true;
    }

    private void OnStopRecruit(Recruit sender)
    {
        isRecruiting = false;
    }

    // i need this for allies
    private void OnAllyMovementStart()
    {
        allyIsMoving = true;
    }

    private void OnAllyMovementEnd()
    {
        allyIsMoving = false;
    }

    private void OnScriptedMovementStart()
    {
        playerScriptedMovement = true;
    }

    private void OnScriptedMovementEnd()
    {
        playerScriptedMovement = false;
    }

    #endregion
}
