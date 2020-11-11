using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBehavior : AbstractBoss
{
    [SerializeField]
    private EnemyAttack swipeAttack;

    [SerializeField] private EnemyAttack flameCircle;
    [SerializeField] private EnemyAttack flameExplosion;
    
    
    // sum between single and group must be 1
    public float singleAttackProb = 0.6f;
    public float groupAttackProb = 0.4f;
    public int attacksBeforeGlobal = 5;
    public int targetsBeforePlayer = 4;
    // this attack will be available only after 5 normal attacks (single or group)
    public float globalAttackProb = 0.4f;

    public float speed = 8f;
    [Range(0f, 1f)]
    public float rotSpeed = 0.1f;
    public float stopDist = 2.5f;
    public float stareTime = 2f;
    public float pursueTime = 5f;
    [Range(0f, 1f)]
    public float changeTargetProb = 0.3f;
    public float maxDistFromCenter = 16.5f;
    public float maxTargetDistFromCenter = 18f;

    private float singleAttackDuration;
    private float groupAttackDuration;
    private float globalAttackDuration;
    private int normalAttacksCount = 0;
    private int targetsCount = 0;

    public override void InitializeValues() {
        Speed = speed;
        RotSpeed = rotSpeed;
        StopDist = stopDist;
        StareTime = stareTime;
        PursueTime = pursueTime;
        ChangeTargetProb = changeTargetProb;
        MaxDistFromCenter = maxDistFromCenter;
        MaxTargetDistFromCenter = maxTargetDistFromCenter;

        // this is the true probability value
        float originalGlobalProb = globalAttackProb;
        globalAttackProb = ((singleAttackProb + groupAttackProb + globalAttackProb) * globalAttackProb) / (singleAttackProb + groupAttackProb);
        //Debug.Log("boss global attack probability will be " + globalAttackProb + "/" + (singleAttackProb + groupAttackProb + originalGlobalProb) + " when available");
    }

    void Start() {
        InitializeValues();

        singleAttackDuration = AnimationsManager.GetAnimation("SingleAttack").length;
        groupAttackDuration = AnimationsManager.GetAnimation("GroupAttack").length;
        globalAttackDuration = AnimationsManager.GetAnimation("GlobalAttack").length + AnimationsManager.GetAnimation("Charge").length - 1f;  // 1 is the transition lenght between the two animations

        FSMTransition t0 = new FSMTransition(PlayerApproaching);
        FSMTransition t1 = new FSMTransition(EnemiesAreDead);
        FSMTransition t2 = new FSMTransition(Death);

        waitingState = new FSMState();

        fightingState = new FSMState();
        fightingState.enterActions.Add(StartFightingCoroutine);
        fightingState.exitActions.Add(StopFightingBT);

        winState = new FSMState();

        deathState = new FSMState();

        waitingState.AddTransition(t0, fightingState);
        fightingState.AddTransition(t1, winState);
        fightingState.AddTransition(t2, deathState);

        Fsm = new FSM(waitingState);
    }

    public override CRBT.BehaviorTree FightingBTBuilder() {

        CRBT.BTCondition timerStarted = new CRBT.BTCondition(TimerStarted);
        CRBT.BTCondition timerGoing = new CRBT.BTCondition(TimerStillGoing);
        CRBT.BTCondition nearArenaCenter = new CRBT.BTCondition(TargetNearArenaCenter);

        CRBT.BTAction target = new CRBT.BTAction(ChooseTarget);
        CRBT.BTAction stare = new CRBT.BTAction(StareAtTarget);
        CRBT.BTAction timeout = new CRBT.BTAction(TimeoutPursue);
        CRBT.BTAction walk = new CRBT.BTAction(WalkToTarget);
        CRBT.BTAction attack = new CRBT.BTAction(ChooseAttack);

        CRBT.BTSelector sel1 = new CRBT.BTSelector(new CRBT.IBTTask[] { timerStarted, timeout });
        CRBT.BTSelector sel3 = new CRBT.BTSelector(new CRBT.IBTTask[] { timerStarted, stare });
        CRBT.BTSelector sel4 = new CRBT.BTSelector(new CRBT.IBTTask[] { timerStarted, attack });
        CRBT.BTSequence seq4 = new CRBT.BTSequence(new CRBT.IBTTask[] { timerGoing, nearArenaCenter, walk });

        CRBT.BTSequence seq1 = new CRBT.BTSequence(new CRBT.IBTTask[] { sel1, seq4 });
        CRBT.BTDecoratorUntilFail uf2 = new CRBT.BTDecoratorUntilFail(seq1);

        CRBT.BTSequence seq3 = new CRBT.BTSequence(new CRBT.IBTTask[] { sel3, timerGoing });
        CRBT.BTDecoratorUntilFail uf1 = new CRBT.BTDecoratorUntilFail(seq3);

        CRBT.BTSequence seq5 = new CRBT.BTSequence(new CRBT.IBTTask[] { sel4, timerGoing });
        CRBT.BTDecoratorUntilFail uf3 = new CRBT.BTDecoratorUntilFail(seq5);

        CRBT.BTSequence seq2 = new CRBT.BTSequence(new CRBT.IBTTask[] { target, uf1, uf2, uf3 });

        CRBT.BTDecoratorUntilFail root = new CRBT.BTDecoratorUntilFail(seq2);

        return new CRBT.BehaviorTree(root);
    }
    
    public override bool ChooseTarget()
    {
        ResetTimer();

        if (GroupsManager.Instance.Groups.Length != 4 && !Player )
            return false;
        
        if ( UnityEngine.Random.Range( 0f, 1f ) < ChangeTargetProb || !TargetDemon || PursueTimeout || TargetFarFromCenter)
        {
            // I always target the player after x non-player targets
            if(targetsCount >= targetsBeforePlayer) {
                ChangeTarget(Player);
                targetsCount = 0;
            }
            else {
                ChooseByAggro();
            }

            PursueTimeout = false;
            TargetFarFromCenter = false;
        }
        // same target as before
        else {
            // check if the previous target has become the player
            if(TargetDemon.GetComponent<Stats>().ThisUnitType == Stats.Type.Player)
                HUD.DeactivateAggroIcon();
        }

        return true;
    }
    
    public override bool ChooseAttack()
    {
        if(!IsAttacking) {

            IsAttacking = true;
            float random = 0f;

            if(normalAttacksCount < attacksBeforeGlobal)
                random = UnityEngine.Random.Range(0f, singleAttackProb + groupAttackProb);
            else
                random = UnityEngine.Random.Range(0f, singleAttackProb + groupAttackProb + globalAttackProb);

            if(random < singleAttackProb) {
                Timer1 = StartCoroutine(Timer(singleAttackDuration, TimerType.attack));
                SwipeAttack();
                normalAttacksCount++;
            } 
            else if(random >= singleAttackProb && random < singleAttackProb + groupAttackProb) {
                Timer1 = StartCoroutine(Timer(groupAttackDuration, TimerType.attack));
                FlameCircle();
                normalAttacksCount++;
            }
            else {
                Timer1 = StartCoroutine(Timer(globalAttackDuration, TimerType.attack));
                FlameExplosion();
                normalAttacksCount = 0;
            }

        }
        
        return true;
    }

    // TODO - now this boss can only swipe attack, we need to add magical attacks
    private void SwipeAttack()
    {
        if ( NormalCombat == null )
        {
            NormalCombat = GetComponentInChildren<NormalCombat>();
            //if ( BossCombat == null )
            //Debug.Log( "Boss Combat cannot be found" );
        }
        if ( NormalCombat != null) {
            NormalCombat.StartAttack(swipeAttack);
        }
        IsAttacking = false;
    }
    
    // TODO - now this boss can only swipe attack, we need to add magical attacks
    private void FlameCircle()
    {
        if ( NormalCombat == null )
        {
            NormalCombat = GetComponentInChildren<NormalCombat>();
            //if ( BossCombat == null )
            //    Debug.Log( "Boss Combat cannot be found" );
        }
        if ( NormalCombat != null) {
            NormalCombat.StartAttack(flameCircle);
        }
        IsAttacking = false;
    }

    // TODO - now this boss can only swipe attack, we need to add magical attacks
    private void FlameExplosion() {
        if ( NormalCombat == null )
        {
            NormalCombat = GetComponentInChildren<NormalCombat>();
            //if ( BossCombat == null )
            //    Debug.Log( "Boss Combat cannot be found" );
        }
        if ( NormalCombat != null) {
            NormalCombat.StartAttack(flameExplosion);
        }
        IsAttacking = false;
    }

    private bool LifeIsHalven() {
        if(Hp <= Stats.health / 2)
            return true;
        return false;
    }
}
