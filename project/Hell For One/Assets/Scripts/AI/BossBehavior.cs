using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//TODO ResetTimer at the beginning of ChooseTarget but I don't understand why it doesn't stop the second timer
//TODO minor error: some times it does only the choosetarget action and than it jumps everything else to choose the target again

public class BossBehavior : MonoBehaviour
{
    public float speed = 8f;
    [Range( 0f, 1f )]
    public float rotSpeed = 0.1f;
    public float stopDist = 4.5f;
    public float stare = 2f;
    public float timeout = 5f;
    [Range( 0f, 1f )]
    public float changeTargetProb = 0.3f;
    public GameObject arenaCenter;
    public float maxDistFromCenter = 23f;

    private GameObject[] demonGroups;
    private GameObject targetGroup;
    private GameObject targetDemon;
    private GameObject player;
    private float[] aggroValues;
    private float[] probability;
    private readonly float singleAttackProb = 0.6f;
    private readonly float groupAttackProb = 0.3f;
    private readonly float globalAttackProb = 0.1f;
    private float crisisMax = 50f;
    private float hp;
    private FSM bossFSM;
    private bool timerStarted = false;
    private bool timerStillGoing = false;
    private bool resetFightingBT = false;
    private CRBT.BehaviorTree FightingBT;
    private Coroutine fightingCR;
    private Coroutine timer;
    private bool canWalk = false;
    [SerializeField]
    private float fsmReactionTime = 0.5f;
    [SerializeField]
    private float btReactionTime = 0.05f;
    private Stats stats;
    private bool demonsReady = false;
    private bool needsCentering = false;
    private float centeringDist;

    private int debugIndex;

    private Combat bossCombat;

    public GameObject TargetGroup { get => targetGroup; set => targetGroup = value; }
    public GameObject TargetDemon { get => targetDemon; set => targetDemon = value; }

    #region Finite State Machine

    FSMState waitingState, fightingState, stunnedState, winState;

    public bool PlayerApproaching()
    {
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

    // The coroutine that cycles through the FSM
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

    private void ResetTimer()
    {
        timerStarted = false;
        timerStillGoing = false;
    }

    public bool ChooseTarget()
    {
        ResetTimer();
        debugIndex++;

        if ( demonGroups.Length != 4 && !player )
            return false;

        //if ( (transform.position - arenaCenter.transform.position).magnitude > maxDistFromCenter )
        //{
        //    ChooseCentralTarget();
        //}
        //else 

        if ( Random.Range( 0f, 1f ) < changeTargetProb || !TargetDemon)
        {
            float totalAggro = 0f;
            string aggroDebug = "aggro values: ";
            string probDebug = "probabilities: ";

            for ( int i = 0; i < demonGroups.Length; i++ )
            {
                float groupAggro = 0f;
                // if the group is empty, I give to the group a value of zero
                if(!demonGroups[i].GetComponent<GroupBehaviour>().IsEmpty())
                    groupAggro = demonGroups[i].GetComponent<GroupAggro>().GetAggro();
                aggroValues[ i ] = groupAggro;
                totalAggro = totalAggro + groupAggro;
                probability[ i + 1 ] = totalAggro;

                aggroDebug = aggroDebug + groupAggro + " - ";
                probDebug = probDebug + totalAggro + " - ";
            }
            // questo controllo va tolto prima o poi
            if ( player )
            {
                aggroValues[ demonGroups.Length ] = player.GetComponent<Stats>().Aggro;
                totalAggro = totalAggro + player.GetComponent<Stats>().Aggro;
                probability[ demonGroups.Length + 1 ] = totalAggro;

                aggroDebug = aggroDebug + player.GetComponent<Stats>().Aggro + " - ";
                probDebug = probDebug + totalAggro + " - ";
            }
            
            //Debug.Log(aggroDebug);
            //Debug.Log(probDebug);

            float random = Random.Range( 0f, totalAggro );

            for ( int i = 1; i < probability.Length; i++ )
            {
                if ( random > probability[ i - 1 ] && random <= probability[ i ] )
                {
                    // if i'm talking about a group (player probability is in the last slot of the array)
                    if ( i < probability.Length - 1 )
                    {
                        TargetGroup = demonGroups[ i - 1 ];
                        //Debug.Log("target group's emptiness is " + targetGroup.GetComponent<GroupBehaviour>().IsEmpty() 
                        //    + " with aggro values between " + probability[i - 1] + " and " + probability[i] + ", random: " + random);
                        TargetDemon = TargetGroup.GetComponent<GroupBehaviour>().GetRandomDemon();
                    }
                    else {
                        //Debug.Log("new target is the player");
                        TargetDemon = player;
                    }

                    break;
                }

            }
            Debug.Log(debugIndex + " - new target is " + TargetDemon + " with random " + random + " and total aggro " + totalAggro);
        }
        else
        {
            Debug.Log(debugIndex + " - target won't change this time");
        }

        Debug.Log("________________________________________");
        return true;
    }

    public bool StareAtTarget()
    {
        if ( timer != null )
            StopCoroutine( timer );
        timer = StartCoroutine( Timer( stare ) );
        return true;
    }

    public bool WalkToTarget()
    {
        if ( HorizDistFromTarget( TargetDemon ) > stopDist )
        {
            canWalk = true;
            return true;
        }
        else
        {
            canWalk = false;
            return false;
        }
    }

    public bool TimeoutAttack()
    {
        StopCoroutine( timer );
        timer = StartCoroutine( Timer( timeout ) );
        return true;
    }

    public bool RandomAttack()
    {
        float random = Random.Range( 0f, singleAttackProb + groupAttackProb + globalAttackProb );
        if ( random < singleAttackProb )
            SingleAttack();
        else if ( random >= singleAttackProb && random < singleAttackProb + groupAttackProb )
            GroupAttack();
        else
            GlobalAttack();

        return true;
    }

    public CRBT.BehaviorTree FightingBTBuilder()
    {

        CRBT.BTCondition started = new CRBT.BTCondition( TimerStarted );
        CRBT.BTCondition going = new CRBT.BTCondition( TimerStillGoing );

        CRBT.BTAction target = new CRBT.BTAction( ChooseTarget );
        CRBT.BTAction stare = new CRBT.BTAction( StareAtTarget );
        CRBT.BTAction timeout = new CRBT.BTAction( TimeoutAttack );
        CRBT.BTAction walk = new CRBT.BTAction( WalkToTarget );
        CRBT.BTAction attack = new CRBT.BTAction( RandomAttack );

        CRBT.BTSelector sel1 = new CRBT.BTSelector( new CRBT.IBTTask[] { started, timeout } );
        CRBT.BTSelector sel3 = new CRBT.BTSelector( new CRBT.IBTTask[] { started, stare } );
        CRBT.BTSequence seq4 = new CRBT.BTSequence( new CRBT.IBTTask[] { going, walk } );

        CRBT.BTSequence seq1 = new CRBT.BTSequence( new CRBT.IBTTask[] { sel1, seq4 } );
        CRBT.BTDecoratorUntilFail uf1 = new CRBT.BTDecoratorUntilFail( seq1 );

        CRBT.BTSequence seq3 = new CRBT.BTSequence( new CRBT.IBTTask[] { sel3, going } );
        CRBT.BTDecoratorUntilFail uf2 = new CRBT.BTDecoratorUntilFail( seq3 );

        CRBT.BTSequence seq2 = new CRBT.BTSequence( new CRBT.IBTTask[] { target, uf2, uf1, attack } );

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

    void Start()
    {
        // the initial target is himself to stay on his place for the first seconds
        arenaCenter = GameObject.Find( "ArenaCenter" );
        stats = GetComponent<Stats>();
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

        FightingBT = FightingBTBuilder();

        waitingState = new FSMState();

        fightingState = new FSMState();
        fightingState.enterActions.Add( StartFightingCoroutine );
        fightingState.exitActions.Add( StopFightingBT );

        stunnedState = new FSMState();

        winState = new FSMState();
        //roar animation to celebrate

        waitingState.AddTransition( t0, fightingState );
        fightingState.AddTransition( t1, stunnedState );
        fightingState.AddTransition( t2, stunnedState );
        fightingState.AddTransition( t4, winState );
        stunnedState.AddTransition( t3, fightingState );

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

        // in case I don't have a target anymore for some reason
        if ( TargetDemon )
        {

            //if ( (transform.position - arenaCenter.transform.position).magnitude >= maxDistFromCenter )
            //{
            //    needsCentering = true;
            //    ChooseCentralTarget();
            //}
            //else if ( (transform.position - arenaCenter.transform.position).magnitude <= centeringDist )
            //    needsCentering = false;


            // If I'm far from arena borders, I'm always facing my last target
            //if ( needsCentering )
            //    Face( arenaCenter );
            //else
                Face( TargetDemon );

            if ( canWalk )
                transform.position += transform.forward * speed * Time.deltaTime;

        }
        else if ( !EnemiesAreDead() )
        {
            ChooseTarget();
        }
    }

    private IEnumerator Timer( float s )
    {
        timerStarted = true;
        timerStillGoing = true;
        yield return new WaitForSeconds( s );
        timerStillGoing = false;
    }

    private void SingleAttack()
    {
        if ( bossCombat == null )
        {
            bossCombat = GetComponent<Combat>();
            if ( bossCombat == null )
                Debug.Log( "Boss Combat cannot be found" );
        }
        if ( bossCombat != null )
            bossCombat.Attack();
        //Debug.Log("single attack!");
    }

    private void GroupAttack()
    {
        if ( bossCombat == null )
        {
            bossCombat = GetComponent<Combat>();
            if ( bossCombat == null )
                Debug.Log( "Boss Combat cannot be found" );
        }
        if ( bossCombat != null )
            bossCombat.Sweep();
        //Debug.Log("group attack!");
    }

    private void GlobalAttack()
    {
        if ( bossCombat == null )
        {
            bossCombat = GetComponent<Combat>();
            if ( bossCombat == null )
                Debug.Log( "Boss Combat cannot be found" );
        }
        if ( bossCombat != null )
            bossCombat.GlobalAttack();
        //Debug.Log("global attack!");
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

    //private void ChooseCentralTarget()
    //{
    //    targetGroup = ClosestGroupTo( arenaCenter.transform.position );
    //    foreach ( GameObject demon in targetGroup.GetComponent<GroupBehaviour>().demons )
    //    {
    //        if ( demon != null )
    //        {
    //            targetDemon = demon;
    //            Debug.Log(debugIndex + " - target is " + targetDemon.name + ", the most centered one");
    //            break;
    //        }
    //    }
    //}

    public GameObject[] GetDemonGroups() {
        return demonGroups;
    }
}
