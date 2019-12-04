﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBehavior : MonoBehaviour
{
    enum TimerType
    {
        stare,
        pursue,
        attack
    }

    public float speed = 8f;
    [Range( 0f, 1f )]
    public float rotSpeed = 0.1f;
    public float stopDist = 2.5f;
    public float stareTime = 2f;
    public float pursueTime = 5f;
    [Range( 0f, 1f )]
    public float changeTargetProb = 0.3f;
    public GameObject arenaCenter;

    // 15 is the ray of boss arena
    public float maxDistFromCenter = 12.5f;
    public float maxTargetDistFromCenter = 14f;

    private GameObject[] demonGroups;
    private GameObject targetGroup;
    private GameObject targetDemon;
    private GameObject player;
    private float[] aggroValues;
    private float[] probability;
    private readonly float singleAttackProb = 0.34f;
    private readonly float groupAttackProb = 0.33f;
    private readonly float globalAttackProb = 0.33f;
    private float crisisMax = 50f;
    private float hp;
    private FSM bossFSM;
    private bool timerStarted = false;
    private bool timerStillGoing = false;
    private bool pursueTimeout = false;
    private bool targetFarFromCenter = false;
    private bool resetFightingBT = false;
    private CRBT.BehaviorTree FightingBT;
    private Coroutine fightingCR;
    private Coroutine timer;
    private Coroutine attackCR;
    private bool canWalk = false;
    [SerializeField]
    private float fsmReactionTime = 0.5f;
    [SerializeField]
    private float btReactionTime = 0.05f;
    private Stats stats;
    private bool demonsReady = false;
    private bool needsCentering = false;
    private float centeringDist;
    public bool isWalking = false;
    public bool isIdle = true;
    public bool isAttacking = false;
    private float singleAttackDuration;
    private float groupAttackDuration;
    private float globalAttackDuration;
    private CombatEventsManager combatEventsManager;
    private AnimationsManager animationsManager;

    private int debugIndex;

    private Combat bossCombat;

    public GameObject TargetGroup { get => targetGroup; set => targetGroup = value; }
    public GameObject TargetDemon { get => targetDemon; set => targetDemon = value; }
    public bool IsWalking { get => isWalking; set => isWalking = value; }
    public bool IsIdle { get => isIdle; set => isIdle = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }

    #region Finite State Machine

    FSMState waitingState, fightingState, stunnedState, winState, deathState;

    public bool PlayerApproaching()
    {
        //TODO - transition between wait and fight event
        //if(playerDistance < tot)
        //    return true;
        return true;
    }

    public bool CrisisFull()
    {
        if ( stats.Crisis >= crisisMax )
            return true;
        return false;
    }

    public bool LifeIsHalven()
    {
        if ( hp <= stats.health / 2 )
            return true;
        return false;
    }

    public bool RecoverFromStun()
    {
        //TODO
        //StartCoroutine(WaitSeconds(5));
        return true;
    }

    public bool EnemiesAreDead()
    {
        int enemies = 0;
        foreach ( GameObject demonGroup in demonGroups )
        {
            enemies += demonGroup.GetComponent<GroupBehaviour>().GetDemonsNumber();
        }
        if ( GameObject.FindGameObjectWithTag( "Player" ) )
            enemies += 1;
        if ( enemies != 0 )
            return false;
        else
            return true;

    }

    public bool Death() {
        if(stats.health <= 0)
            return true;
        else
            return false;
    }
    
    public IEnumerator MoveThroughFSM()
    {
        while ( true )
        {
            bossFSM.Update();
            yield return new WaitForSeconds( fsmReactionTime );
        }
    }

    #endregion

    #region Fighting State Behavior Tree

    public bool TimerStarted()
    {
        return timerStarted;
    }

    public bool TimerStillGoing()
    {
        if ( !timerStillGoing )
        {
            ResetTimer();
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool TargetNearArenaCenter() {
        if((arenaCenter.transform.position - targetDemon.transform.position).magnitude > maxTargetDistFromCenter && HorizDistFromTarget(arenaCenter) > maxDistFromCenter) {
            targetFarFromCenter = true;
            canWalk = false;
            return false;
        }
        else
            return true;
    }

    private void ResetTimer()
    {
        timerStarted = false;
        timerStillGoing = false;
        if(timer != null)
            StopCoroutine(timer);
    }

    public bool ChooseTarget()
    {
        ResetTimer();

        if ( demonGroups.Length != 4 && !player )
            return false;
        
        if ( Random.Range( 0f, 1f ) < changeTargetProb || !TargetDemon || pursueTimeout || targetFarFromCenter)
        {
            GameObject previousTarget = TargetDemon;
            float totalAggro = 0f;
            string aggroDebug = "aggro values: ";
            string probDebug = "probabilities: ";

            for ( int i = 0; i < demonGroups.Length; i++ )
            {
                float groupAggro = 0f;
                // if the group is empty, I give to the group a temporary value of zero
                if ( !demonGroups[ i ].GetComponent<GroupBehaviour>().IsEmpty() )
                    groupAggro = demonGroups[ i ].GetComponent<GroupAggro>().GetAggro();
                aggroValues[ i ] = groupAggro;
                totalAggro = totalAggro + groupAggro;
                probability[ i + 1 ] = totalAggro;

                aggroDebug = aggroDebug + groupAggro + " - ";
                probDebug = probDebug + totalAggro + " - ";
            }

            aggroValues[ demonGroups.Length ] = player.GetComponent<Stats>().Aggro;
            totalAggro = totalAggro + player.GetComponent<Stats>().Aggro;
            probability[ demonGroups.Length + 1 ] = totalAggro;

            aggroDebug = aggroDebug + player.GetComponent<Stats>().Aggro + " - ";
            probDebug = probDebug + totalAggro + " - ";

            float random = Random.Range( 0f, totalAggro );

            // if I was pursuing the player, I won't choose him again
            if ( pursueTimeout)
                random = Random.Range(0f, totalAggro - player.GetComponent<Stats>().Aggro);

            for ( int i = 1; i < probability.Length; i++ )
            {
                if ( random > probability[ i - 1 ] && random <= probability[ i ] )
                {
                    // if i'm talking about a group (player probability is in the last slot of the array)
                    if ( i < probability.Length - 1 )
                    {
                        TargetGroup = demonGroups[ i - 1 ];
                        TargetDemon = TargetGroup.GetComponent<GroupBehaviour>().GetRandomDemon();
                    }
                    else
                    {
                        TargetDemon = player;
                    }

                    break;
                }

            }

            // if the chosen demon is too far from arena center I choose one from the most centered group
            if ( (TargetDemon.transform.position - arenaCenter.transform.position).magnitude > maxDistFromCenter || targetFarFromCenter )
                ChooseCentralTarget();

            pursueTimeout = false;
            targetFarFromCenter = false;
        }

        return true;
    }

    public bool StareAtTarget()
    {
        if ( timer != null )
            StopCoroutine( timer );
        timer = StartCoroutine( Timer( stareTime, TimerType.stare ) );
        return true;
    }

    public bool WalkToTarget()
    {
        if ( HorizDistFromTarget( TargetDemon ) > stopDist)
        {
            canWalk = true;
            return true;
        }
        else
        {
            canWalk = false;
            ResetTimer();
            return false;
        }
    }

    public bool TimeoutPursue()
    {
        StopCoroutine( timer );
        timer = StartCoroutine( Timer( pursueTime, TimerType.pursue ) );
        return true;
    }

    public bool RandomAttack()
    {
        if(!isAttacking) {

            isAttacking = true;

            float random = Random.Range(0f, singleAttackProb + groupAttackProb + globalAttackProb);
            if(random < singleAttackProb) {
                Debug.Log("single attack");
                timer = StartCoroutine(Timer(singleAttackDuration, TimerType.attack));
                SingleAttack();
            } 
            else if(random >= singleAttackProb && random < singleAttackProb + groupAttackProb) {
                Debug.Log("group attack");
                timer = StartCoroutine(Timer(groupAttackDuration, TimerType.attack));
                GroupAttack();
            }
            else {
                Debug.Log("global attack");
                timer = StartCoroutine(Timer(globalAttackDuration, TimerType.attack));
                GlobalAttack();
            }

        }
        
        return true;
    }

    public CRBT.BehaviorTree FightingBTBuilder()
    {

        CRBT.BTCondition timerStarted = new CRBT.BTCondition( TimerStarted );
        CRBT.BTCondition timerGoing = new CRBT.BTCondition( TimerStillGoing );
        CRBT.BTCondition nearArenaCenter = new CRBT.BTCondition(TargetNearArenaCenter);

        CRBT.BTAction target = new CRBT.BTAction( ChooseTarget );
        CRBT.BTAction stare = new CRBT.BTAction( StareAtTarget );
        CRBT.BTAction timeout = new CRBT.BTAction( TimeoutPursue );
        CRBT.BTAction walk = new CRBT.BTAction( WalkToTarget );
        CRBT.BTAction attack = new CRBT.BTAction( RandomAttack );

        CRBT.BTSelector sel1 = new CRBT.BTSelector( new CRBT.IBTTask[] { timerStarted, timeout } );
        CRBT.BTSelector sel3 = new CRBT.BTSelector( new CRBT.IBTTask[] { timerStarted, stare } );
        CRBT.BTSelector sel4 = new CRBT.BTSelector(new CRBT.IBTTask[] { timerStarted, attack });
        CRBT.BTSequence seq4 = new CRBT.BTSequence( new CRBT.IBTTask[] { timerGoing, nearArenaCenter, walk } );

        CRBT.BTSequence seq1 = new CRBT.BTSequence( new CRBT.IBTTask[] { sel1, seq4 } );
        CRBT.BTDecoratorUntilFail uf2 = new CRBT.BTDecoratorUntilFail( seq1 );

        CRBT.BTSequence seq3 = new CRBT.BTSequence( new CRBT.IBTTask[] { sel3, timerGoing } );
        CRBT.BTDecoratorUntilFail uf1 = new CRBT.BTDecoratorUntilFail( seq3 );

        CRBT.BTSequence seq5 = new CRBT.BTSequence(new CRBT.IBTTask[] { sel4, timerGoing });
        CRBT.BTDecoratorUntilFail uf3 = new CRBT.BTDecoratorUntilFail(seq5);

        CRBT.BTSequence seq2 = new CRBT.BTSequence( new CRBT.IBTTask[] { target, uf1, uf2, uf3 } );

        CRBT.BTDecoratorUntilFail root = new CRBT.BTDecoratorUntilFail( seq2 );

        return new CRBT.BehaviorTree( root );
    }

    public IEnumerator FightingLauncherCR()
    {
        while ( FightingBT.Step() )
            yield return new WaitForSeconds( btReactionTime );
    }

    public void StartFightingCoroutine()
    {
        if ( resetFightingBT )
        {
            FightingBT = FightingBTBuilder();
            resetFightingBT = false;
        }

        fightingCR = StartCoroutine( FightingLauncherCR() );
    }

    public void StopFightingBT()
    {
        StopCoroutine( fightingCR );
        fightingCR = null;
        resetFightingBT = true;
    }

    #endregion

    private void Awake() {
        combatEventsManager = GetComponent<CombatEventsManager>();
        animationsManager = GetComponent<AnimationsManager>();
        stats = GetComponent<Stats>();
    }

    void Start() {
        singleAttackDuration = animationsManager.GetAnimation("SingleAttack").length;
        groupAttackDuration = animationsManager.GetAnimation("GroupAttack").length;
        globalAttackDuration = animationsManager.GetAnimation("GlobalAttack").length + animationsManager.GetAnimation("Charge").length;
        arenaCenter = GameObject.Find( "ArenaCenter" );
        hp = stats.health;
        demonGroups = GameObject.FindGameObjectsWithTag( "Group" );
        player = GameObject.FindGameObjectWithTag( "Player" );
        aggroValues = new float[ demonGroups.Length + 1 ];
        probability = new float[ demonGroups.Length + 2 ];
        probability[ 0 ] = 0f;
        centeringDist = maxDistFromCenter - 5f;

        FSMTransition t0 = new FSMTransition( PlayerApproaching );
        FSMTransition t1 = new FSMTransition( CrisisFull );
        FSMTransition t2 = new FSMTransition( LifeIsHalven );
        FSMTransition t3 = new FSMTransition( RecoverFromStun );
        FSMTransition t4 = new FSMTransition( EnemiesAreDead );
        FSMTransition t5 = new FSMTransition( Death );

        FightingBT = FightingBTBuilder();

        waitingState = new FSMState();

        fightingState = new FSMState();
        fightingState.enterActions.Add( StartFightingCoroutine );
        fightingState.exitActions.Add( StopFightingBT );

        stunnedState = new FSMState();

        winState = new FSMState();
        //TODO - if we will have a transparent gameover screen, the bosse must do the roar animation to celebrate in the background

        deathState = new FSMState();

        waitingState.AddTransition( t0, fightingState );
        fightingState.AddTransition( t1, stunnedState );
        fightingState.AddTransition( t2, stunnedState );
        fightingState.AddTransition( t4, winState );
        stunnedState.AddTransition( t3, fightingState );
        fightingState.AddTransition( t5, deathState );
        stunnedState.AddTransition( t5, deathState );

        bossFSM = new FSM( waitingState );
    }

    void FixedUpdate()
    {
        if ( !player )
            player = GameObject.FindGameObjectWithTag( "Player" );

        if ( !demonsReady && demonGroups[ 0 ].GetComponent<GroupBehaviour>().CheckDemons() )
        {
            demonsReady = true;
            StartCoroutine( MoveThroughFSM() );
        }

        if ( TargetDemon && !isAttacking && stats.health > 0)
        {
            Face( TargetDemon );

            if ( canWalk ) {
                if(!isWalking) {
                    IsWalking = true;
                    IsIdle = false;
                    combatEventsManager.RaiseOnStartWalking();

                }

                transform.position += transform.forward * speed * Time.deltaTime;

            } else {
                if(!isIdle) {
                    IsIdle = true;
                    IsWalking = false;
                    combatEventsManager.RaiseOnStartIdle();
                }
            }

        }
        else if ( !EnemiesAreDead() )
        {
            ChooseTarget();
        }
    }

    private IEnumerator Timer( float s, TimerType type )
    {
        timerStarted = true;
        timerStillGoing = true;
        yield return new WaitForSeconds( s );
        timerStillGoing = false;
        if ( type == TimerType.pursue )
        {
            pursueTimeout = true;
            canWalk = false;
        }
        else if (type == TimerType.attack ) 
        {
            combatEventsManager.RaiseOnStartIdle();
            // TODO - don't know why it doesn't work here
            //isAttacking = false;
        }

    }

    private void SingleAttack()
    {
        if ( bossCombat == null )
        {
            bossCombat = GetComponent<Combat>();
            if ( bossCombat == null )
                Debug.Log( "Boss Combat cannot be found" );
        }
        if ( bossCombat != null) {
            bossCombat.PlayerAttack();
        }
        isAttacking = false;
    }

    private void GroupAttack()
    {
        if ( bossCombat == null )
        {
            bossCombat = GetComponent<Combat>();
            if ( bossCombat == null )
                Debug.Log( "Boss Combat cannot be found" );
        }
        if ( bossCombat != null) {
            bossCombat.GroupAttack();
        }
        isAttacking = false;
    }

    private void GlobalAttack() {
        if ( bossCombat == null )
        {
            bossCombat = GetComponent<Combat>();
            if ( bossCombat == null )
                Debug.Log( "Boss Combat cannot be found" );
        }
        if ( bossCombat != null) {
            bossCombat.GlobalAttack();
        }
        isAttacking = false;
    }

    private float HorizDistFromTarget( GameObject target )
    {
        Vector3 targetPosition = new Vector3( target.transform.position.x, transform.position.y, target.transform.position.z );
        return (targetPosition - transform.position).magnitude;
    }

    private void Face( GameObject target )
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.y = 0f;
        Quaternion facingDir = Quaternion.LookRotation( vectorToTarget );
        Quaternion newRotation = Quaternion.Slerp( transform.rotation, facingDir, rotSpeed );
        transform.rotation = newRotation;
    }

    private GameObject ClosestGroupTo( Vector3 position )
    {

        GameObject closest = demonGroups[ 0 ];
        float minDist = float.MaxValue;

        foreach ( GameObject group in demonGroups )
        {
            if ( group.GetComponent<GroupBehaviour>().IsEmpty() == false )
            {
                if ( (group.transform.position - position).magnitude < minDist )
                {
                    minDist = (group.transform.position - position).magnitude;
                    closest = group;
                }
            }
        }

        return closest;
    }

    private void ChooseCentralTarget()
    {
        TargetGroup = ClosestGroupTo( arenaCenter.transform.position );
        foreach ( GameObject demon in TargetGroup.GetComponent<GroupBehaviour>().demons )
        {
            if ( demon != null )
            {
                TargetDemon = demon;
                break;
            }
        }
    }

    public GameObject[] GetDemonGroups()
    {
        return demonGroups;
    }
}
