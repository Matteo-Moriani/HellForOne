using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupBehaviour : MonoBehaviour
{
    // This script is attached to invisible gameobjacts that manage the single group

    #region FSM

    private FSM groupFSM;

    // Useful to distinguish between States in game as Tactics for group and FSMState (e.g. Idle not present here since it's not in game)
    public enum State
    {
        MeleeAttack,
        Tank,
        RangeAttack,
        Support
    }

    // Used to know if the group is in combat or not (don't want to add a state in State enum cause it's simpler this way)
    private bool inCombat = false;
    private State currentState;
    public State newState;
    public bool orderConfirmed = false;
    FSMState meleeState, tankState, rangeAttackState, supportState, idleState;

    // The time after the next update of the FSM
    [SerializeField]
    private float reactionTime = 1f;

    public bool MeleeOrderGiven()
    {
        if ( (newState != currentState) && (orderConfirmed) && (newState == State.MeleeAttack) )
        {
            orderConfirmed = false;
            return true;
        }
        return false;
    }

    public bool TankOrderGiven()
    {
        if ( (newState != currentState) && (orderConfirmed) && (newState == State.Tank) )
        {
            orderConfirmed = false;
            return true;
        }
        return false;
    }

    public bool RangeAttackOrderGiven()
    {
        if ( (newState != currentState) && (orderConfirmed) && (newState == State.RangeAttack) )
        {
            orderConfirmed = false;
            return true;
        }
        return false;
    }

    public bool SupportOrderGiven()
    {
        if ( (newState != currentState) && (orderConfirmed) && (newState == State.Support) )
        {
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

    #endregion

    public FSMState getCurrentFSMState( State state )
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

    public void IamMelee()
    {
        Debug.Log( "Melee state" );
    }

    public void IamTank()
    {
        Debug.Log( "Tank state" );
    }

    public void IamRange()
    {
        Debug.Log( "Range state" );
    }

    public void IamSupport()
    {
        Debug.Log( "Support state" );
    }

    // The coroutine that cycles through the FSM
    public IEnumerator MoveThroughFSM()
    {
        while ( true )
        {
            groupFSM.Update();
            yield return new WaitForSeconds( reactionTime );
        }
    }

    void Start()
    {
        currentState = State.MeleeAttack;
        // Just to test
        inCombat = true;

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

        meleeState.enterActions.Add( IamMelee );
        tankState.enterActions.Add( IamTank );
        rangeAttackState.enterActions.Add( IamRange );
        supportState.enterActions.Add( IamSupport );

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

        idleState.AddTransition( t6, getCurrentFSMState( currentState ) );

        //groupFSM = new FSM( idleState );
        groupFSM = new FSM( meleeState );
        StartCoroutine( MoveThroughFSM() );

        #endregion
    }
}
