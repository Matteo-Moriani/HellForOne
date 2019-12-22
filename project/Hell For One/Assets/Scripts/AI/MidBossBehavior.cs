using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MidBossBehavior : AbstractBoss {

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

        if(DemonGroups.Length != 4 && !Player)
            return false;

        if(Random.Range(0f, 1f) < changeTargetProb || !TargetDemon || PursueTimeout || TargetFarFromCenter) {
            GameObject previousTarget = TargetDemon;
            float totalAggro = 0f;
            string aggroDebug = "aggro values: ";
            string probDebug = "probabilities: ";

            for(int i = 0; i < DemonGroups.Length; i++) {
                float groupAggro = 0f;
                // if the group is empty, I give to the group a temporary value of zero
                if(!DemonGroups[i].GetComponent<GroupBehaviour>().IsEmpty())
                    groupAggro = DemonGroups[i].GetComponent<GroupAggro>().GetAggro();
                AggroValues[i] = groupAggro;
                totalAggro = totalAggro + groupAggro;
                Probability[i + 1] = totalAggro;

                aggroDebug = aggroDebug + groupAggro + " - ";
                probDebug = probDebug + totalAggro + " - ";
            }

            AggroValues[DemonGroups.Length] = Player.GetComponent<Stats>().Aggro;
            totalAggro = totalAggro + Player.GetComponent<Stats>().Aggro;
            Probability[DemonGroups.Length + 1] = totalAggro;

            aggroDebug = aggroDebug + Player.GetComponent<Stats>().Aggro + " - ";
            probDebug = probDebug + totalAggro + " - ";

            float random = Random.Range(0f, totalAggro);

            // if I was pursuing the player, I won't choose him again
            if(PursueTimeout)
                random = Random.Range(0f, totalAggro - Player.GetComponent<Stats>().Aggro);

            for(int i = 1; i < Probability.Length; i++) {
                if(random > Probability[i - 1] && random <= Probability[i]) {
                    // if i'm talking about a group (player probability is in the last slot of the array)
                    if(i < Probability.Length - 1) {
                        TargetGroup = DemonGroups[i - 1];
                        ActivateAggroIcon(i - 1);
                        TargetDemon = TargetGroup.GetComponent<GroupBehaviour>().GetRandomDemon();
                    }
                    else {
                        Hud.DeactivateBossFace();
                        TargetDemon = Player;
                    }

                    break;
                }

            }

            // if the chosen demon is too far from arena center I choose one from the most centered group
            if((TargetDemon.transform.position - ArenaCenter.transform.position).magnitude > maxDistFromCenter || TargetFarFromCenter)
                ChooseCentralTarget();

            PursueTimeout = false;
            TargetFarFromCenter = false;
        }

        return true;
    }
    
    public override bool ChooseAttack() {
        if(!IsAttacking) {

            IsAttacking = true;

            float random = Random.Range(0f, singleAttackProb + groupAttackProb);
            if(random < singleAttackProb) {
                Debug.Log("single attack");
                Timer1 = StartCoroutine(Timer(singleAttackDuration, TimerType.attack));
                SingleAttack();
            }
            else {
                Debug.Log("group attack");
                Timer1 = StartCoroutine(Timer(groupAttackDuration, TimerType.attack));
                GroupAttack();
            }

        }

        return true;
    }

    private void SingleAttack() {
        if(BossCombat == null) {
            BossCombat = GetComponent<Combat>();
            if(BossCombat == null)
                Debug.Log("Boss Combat cannot be found");
        }
        if(BossCombat != null) {
            BossCombat.PlayerAttack();
        }
        IsAttacking = false;
    }

    private void GroupAttack() {
        if(BossCombat == null) {
            BossCombat = GetComponent<Combat>();
            if(BossCombat == null)
                Debug.Log("Boss Combat cannot be found");
        }
        if(BossCombat != null) {
            BossCombat.GroupAttack();
        }
        IsAttacking = false;
    }
    
}
