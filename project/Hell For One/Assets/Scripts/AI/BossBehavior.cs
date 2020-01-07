using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBehavior : AbstractBoss
{
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

        if ( DemonGroups.Length != 4 && !Player )
            return false;
        
        if ( Random.Range( 0f, 1f ) < ChangeTargetProb || !TargetDemon || PursueTimeout || TargetFarFromCenter)
        {
            // I always target the player after x non-player targets
            if(targetsCount >= targetsBeforePlayer) {
                HUD.DeactivateAggroIcon();
                TargetDemon = Player;
                targetsCount = 0;
            }
            else {
                GameObject previousTarget = TargetDemon;
                float totalAggro = 0f;

                for(int i = 0; i < DemonGroups.Length; i++) {
                    float groupAggro = 0f;
                    // if the group is empty, I give to the group a temporary value of zero
                    if(!DemonGroups[i].GetComponent<GroupBehaviour>().IsEmpty())
                        groupAggro = DemonGroups[i].GetComponent<GroupAggro>().GetAggro();
                    AggroValues[i] = groupAggro;
                    totalAggro = totalAggro + groupAggro;
                    Probability[i + 1] = totalAggro;
                }

                AggroValues[DemonGroups.Length] = Player.GetComponent<Stats>().Aggro;
                totalAggro = totalAggro + Player.GetComponent<Stats>().Aggro;
                Probability[DemonGroups.Length + 1] = totalAggro;

                float random = Random.Range(0f, totalAggro);

                // if I was pursuing the player, I won't choose him again
                if(PursueTimeout)
                    random = Random.Range(0f, totalAggro - Player.GetComponent<Stats>().Aggro);

                for(int i = 1; i < Probability.Length; i++) {
                    if(random > Probability[i - 1] && random <= Probability[i]) {
                        // if i'm talking about a group (player probability is in the last slot of the array)
                        if(i < Probability.Length - 1) {
                            TargetGroup = DemonGroups[i - 1];
                            SwitchAggroIcon(i - 1);
                            TargetDemon = TargetGroup.GetComponent<GroupBehaviour>().GetRandomDemon();
                            targetsCount++;
                        }
                        else {
                            HUD.DeactivateAggroIcon();
                            TargetDemon = Player;
                            targetsCount = 0;
                        }

                        break;
                    }

                }
            }

            // if the chosen demon is too far from arena center I choose one from the most centered group
            if ( (TargetDemon.transform.position - ArenaCenter.transform.position).magnitude > MaxDistFromCenter || TargetFarFromCenter )
                ChooseCentralTarget();

            PursueTimeout = false;
            TargetFarFromCenter = false;
        }
        // same target as before
        else {
            // check if the previous target has become the player
            if(TargetDemon.GetComponent<Stats>().type == Stats.Type.Player)
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
                random = Random.Range(0f, singleAttackProb + groupAttackProb);
            else
                random = Random.Range(0f, singleAttackProb + groupAttackProb + globalAttackProb);

            if(random < singleAttackProb) {
                Timer1 = StartCoroutine(Timer(singleAttackDuration, TimerType.attack));
                SingleAttack();
                normalAttacksCount++;
            } 
            else if(random >= singleAttackProb && random < singleAttackProb + groupAttackProb) {
                Timer1 = StartCoroutine(Timer(groupAttackDuration, TimerType.attack));
                GroupAttack();
                normalAttacksCount++;
            }
            else {
                Timer1 = StartCoroutine(Timer(globalAttackDuration, TimerType.attack));
                GlobalAttack();
                normalAttacksCount = 0;
            }

        }
        
        return true;
    }

    private void SingleAttack()
    {
        if ( BossCombat == null )
        {
            BossCombat = GetComponent<Combat>();
            //if ( BossCombat == null )
                //Debug.Log( "Boss Combat cannot be found" );
        }
        if ( BossCombat != null) {
            BossCombat.PlayerAttack();
        }
        IsAttacking = false;
    }

    private void GroupAttack()
    {
        if ( BossCombat == null )
        {
            BossCombat = GetComponent<Combat>();
            //if ( BossCombat == null )
            //    Debug.Log( "Boss Combat cannot be found" );
        }
        if ( BossCombat != null) {
            BossCombat.GroupAttack();
        }
        IsAttacking = false;
    }

    private void GlobalAttack() {
        if ( BossCombat == null )
        {
            BossCombat = GetComponent<Combat>();
            //if ( BossCombat == null )
            //    Debug.Log( "Boss Combat cannot be found" );
        }
        if ( BossCombat != null) {
            BossCombat.GlobalAttack();
        }
        IsAttacking = false;
    }

    private bool LifeIsHalven() {
        if(Hp <= Stats.health / 2)
            return true;
        return false;
    }
}
