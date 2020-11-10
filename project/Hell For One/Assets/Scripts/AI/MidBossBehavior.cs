using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MidBossBehavior : AbstractBoss
{
    [SerializeField]
    private EnemyAttack swipeAttack;

    public float singleAttackProb = 0.7f;
    public float groupAttackProb = 0.3f;

    public float speed = 5f;
    [Range(0f, 1f)]
    public float rotSpeed = 0.1f;
    public float stopDist = 2f;
    public float stareTime = 2.5f;
    public float pursueTime = 5f;
    [Range(0f, 1f)]
    public float changeTargetProb = 0.2f;
    public float maxDistFromCenter = 9.5f;
    public float maxTargetDistFromCenter = 11f;

    private float singleAttackDuration;
    private float groupAttackDuration;

    public override void InitializeValues() {
        Speed = speed;
        RotSpeed = rotSpeed;
        StopDist = stopDist;
        StareTime = stareTime;
        PursueTime = pursueTime;
        ChangeTargetProb = changeTargetProb;
        MaxDistFromCenter = maxDistFromCenter;
        MaxTargetDistFromCenter = maxTargetDistFromCenter;
    }
    
    void Start() {
        InitializeValues();

        singleAttackDuration = AnimationsManager.GetAnimation("SingleAttack").length;
        groupAttackDuration = AnimationsManager.GetAnimation("GroupAttack").length;

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

    public override bool ChooseTarget() {
        ResetTimer();

        if(GroupsManager.Instance.Groups.Length != 4 && !Player)
            return false;

        if(UnityEngine.Random.Range(0f, 1f) < changeTargetProb || !TargetDemon || PursueTimeout || TargetFarFromCenter) {
            ChooseByAggro();

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

    public override bool ChooseAttack() {
        if(!IsAttacking) {

            IsAttacking = true;

            float random = UnityEngine.Random.Range(0f, singleAttackProb + groupAttackProb);
            if(random < singleAttackProb) {
                Timer1 = StartCoroutine(Timer(singleAttackDuration, TimerType.attack));
                SingleAttack();
            }
            else {
                Timer1 = StartCoroutine(Timer(groupAttackDuration, TimerType.attack));
                GroupAttack();
            }

        }

        return true;
    }

    // TODO - Mid boss now can only swipe attack, I left both methods if we need to add attacks
    private void SingleAttack() {
        if (!isStunned)
        {
            if(NormalCombat == null)
            {
                NormalCombat = GetComponentInChildren<NormalCombat>();
            }
            if(NormalCombat != null) {
                NormalCombat.StartAttack(swipeAttack);
            }    
        }

        IsAttacking = false;
    }
    
    // TODO - Mid boss now can only swipe attack, I left both methods if we need to add attacks
    private void GroupAttack() {
        if (!isStunned)
        {
            if(NormalCombat == null)
            {
                NormalCombat = GetComponentInChildren<NormalCombat>();
            }
            if(NormalCombat != null) {
                NormalCombat.StartAttack(swipeAttack);
            }    
        }
        
        IsAttacking = false;
    }
    
}
