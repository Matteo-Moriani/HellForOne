using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupBehaviour : MonoBehaviour
{
    /// This script is attached to invisible gameobjects that manages the single group

    public int maxNumDemons = 4;
    [SerializeField]
    private int demonsInGroup = 0;

    #region 

    // Useful to distinguish between States in game as Tactics for group and FSMState (e.g. Idle not present here since it's not in game)
    public enum State
    {
        MeleeAttack,
        Tank,
        RangeAttack,
        Support
    }

    // The time after the next update of the FSM
    public float fsmReactionTime = 1f;
    public State currentState;
    public State newState;
    public bool orderConfirmed = false;
    public GameObject[] demons;

    private FSM groupFSM;
    // Used to know if the group is in combat or not (don't want to add a state in State enum cause it's simpler this way)
    private bool inCombat = false;
    FSMState meleeState, tankState, rangeAttackState, supportState, idleState;
    [SerializeField]
    private GameObject target;

    #region Conditions

    public bool MeleeOrderGiven()
    {
        if ( (newState != currentState) && (orderConfirmed) && (newState == State.MeleeAttack) )
        {
            currentState = newState;
            //ConfirmEffects();
            orderConfirmed = false;
            return true;
        }
        return false;
    }

    public bool TankOrderGiven()
    {
        if ( (newState != currentState) && (orderConfirmed) && (newState == State.Tank) )
        {
            currentState = newState;
            //ConfirmEffects();
            orderConfirmed = false;
            return true;
        }
        return false;
    }

    public bool RangeAttackOrderGiven()
    {
        if ( (newState != currentState) && (orderConfirmed) && (newState == State.RangeAttack) )
        {
            currentState = newState;
            //ConfirmEffects();
            orderConfirmed = false;
            return true;
        }
        return false;
    }

    public bool SupportOrderGiven()
    {
        if ( (newState != currentState) && (orderConfirmed) && (newState == State.Support) )
        {
            currentState = newState;
            //ConfirmEffects();
            orderConfirmed = false;
            return true;
        }
        return false;
    }

    public bool Idle()
    {
        if ( !inCombat )
            return true;
        return false;
    }

    public bool EnterCombat()
    {
        if ( !Idle() )
            return true;
        return false;
    }

    private void ConfirmEffects()
    {
        foreach ( GameObject demon in demons )
        {
            demon.GetComponent<SelectedUnitEffects>().ConfirmOrder();
        }
    }

    #endregion

    #region Actions

    // Maybe all the CheckDemons() can be avoided by putting only 1 CheckDemons() inside the FSMUpdate()
    public void MeleeAttack()
    {
        if ( !CheckDemons() )
            return;
        foreach ( GameObject demon in demons )
        {
            // This check must be done in every tactic
            if ( demon )
            {
                Combat combat = demon.GetComponent<Combat>();
                combat.Attack();
            }
        }
    }

    public void StopAttack()
    {
        if ( !CheckDemons() )
            return;
        foreach ( GameObject demon in demons )
        {
            if ( demon )
            {
                Combat combat = demon.GetComponent<Combat>();
                combat.StopAttack();
            }
        }
    }

    public void Tank()
    {
        if ( !CheckDemons() )
            return;
        foreach ( GameObject demon in demons )
        {
            if ( demon )
            {
                Combat combat = demon.GetComponent<Combat>();
                combat.StartBlock();
            }
        }
    }

    public void StopTank()
    {
        if ( !CheckDemons() )
            return;
        foreach ( GameObject demon in demons )
        {
            if ( demon )
            {
                Combat combat = demon.GetComponent<Combat>();
                combat.StopBlock();
            }
        }
    }

    public void RangeAttack()
    {
        if ( !CheckDemons() )
            return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag( "LittleEnemy" );
        GameObject boss = GameObject.FindGameObjectWithTag( "Boss" );

        if ( boss )
            target = boss;
        else if ( enemies != null )
            target = CameraManager.FindNearestEnemy( gameObject, enemies );
        else
            return;


        foreach ( GameObject demon in demons )
        {
            if ( demon )
            {
                Combat combat = demon.GetComponent<Combat>();
                combat.RangedAttack( target );
            }
        }
    }

    public void StopRangeAttack()
    {
        if ( !CheckDemons() )
            return;

        foreach ( GameObject demon in demons )
        {
            if ( demon )
            {
                Combat combat = demon.GetComponent<Combat>();
                combat.StopRangedAttack();
            }
        }
    }

    public void Support()
    {

    }

    public void UpdateSupportAggro()
    {
        foreach ( GameObject demon in demons )
        {
            demon.GetComponent<Stats>().Aggro *= 1.08f;
        }
    }

    #endregion

    #endregion

    //TODO To know if all demons found their group (can be improved by just setting a single boolean in a single gameobjact, without checking for all demons)
    public bool CheckDemons()
    {
        GameObject[] allDemons = GameObject.FindGameObjectsWithTag( "Demon" );
        foreach ( GameObject go in allDemons )
        {
            if ( !go.GetComponent<DemonBehaviour>().groupFound )
                return false;
        }
        return true;
    }

    public FSMState GetCurrentFSMState( State state )
    {
        switch ( state )
        {
            case State.MeleeAttack:
                return meleeState;
            case State.Tank:
                return tankState;
            case State.RangeAttack:
                return rangeAttackState;
            case State.Support:
                return supportState;
        }
        return null;
    }

    // The coroutine that cycles through the FSM
    public IEnumerator MoveThroughFSM()
    {
        while ( true )
        {
            yield return new WaitForSeconds( fsmReactionTime );
            groupFSM.Update();
        }
    }

    void Start()
    {
        currentState = State.MeleeAttack;
        // Just to test
        inCombat = true;

        demons = new GameObject[ maxNumDemons ];

        #region FSM

        FSMTransition t1 = new FSMTransition( MeleeOrderGiven );
        FSMTransition t2 = new FSMTransition( TankOrderGiven );
        FSMTransition t3 = new FSMTransition( RangeAttackOrderGiven );
        FSMTransition t4 = new FSMTransition( SupportOrderGiven );
        FSMTransition t5 = new FSMTransition( Idle );
        FSMTransition t6 = new FSMTransition( EnterCombat );

        meleeState = new FSMState();
        tankState = new FSMState();
        rangeAttackState = new FSMState();
        supportState = new FSMState();
        idleState = new FSMState();

        meleeState.stayActions.Add( MeleeAttack );
        meleeState.exitActions.Add( StopAttack );

        rangeAttackState.enterActions.Add( RangeAttack );
        rangeAttackState.exitActions.Add( StopRangeAttack );

        tankState.enterActions.Add( Tank );
        tankState.exitActions.Add( StopTank );

        supportState.stayActions.Add( UpdateSupportAggro );

        meleeState.AddTransition( t2, tankState );
        meleeState.AddTransition( t3, rangeAttackState );
        meleeState.AddTransition( t4, supportState );
        meleeState.AddTransition( t5, idleState );

        tankState.AddTransition( t1, meleeState );
        tankState.AddTransition( t3, rangeAttackState );
        tankState.AddTransition( t4, supportState );
        tankState.AddTransition( t5, idleState );

        rangeAttackState.AddTransition( t1, meleeState );
        rangeAttackState.AddTransition( t2, tankState );
        rangeAttackState.AddTransition( t4, supportState );
        rangeAttackState.AddTransition( t5, idleState );

        supportState.AddTransition( t1, meleeState );
        supportState.AddTransition( t2, tankState );
        supportState.AddTransition( t3, rangeAttackState );
        supportState.AddTransition( t5, idleState );

        idleState.AddTransition( t6, GetCurrentFSMState( currentState ) );

        //groupFSM = new FSM( idleState );
        groupFSM = new FSM( meleeState );
        StartCoroutine( MoveThroughFSM() );

        #endregion
    }

    public bool IsEmpty()
    {
        foreach ( GameObject demon in demons )
        {
            if ( demon != null )
                return false;
        }
        return true;
    }

    public int GetDemonsNumber()
    {
        return demonsInGroup;
    }

    public void SetDemonsNumber( int i )
    {
        demonsInGroup = i;
    }

    public GameObject GetRandomDemon()
    {
        int index = Random.Range( 0, demonsInGroup );
        return demons[ index ];
    }
}
