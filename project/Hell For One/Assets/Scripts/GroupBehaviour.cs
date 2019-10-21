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
    public State currentState;
    public State newState;
    public bool orderConfirmed = false;
    FSMState meleeState, tankState, rangeAttackState, supportState, idleState;

    public GameObject[] demons;

    // The time after the next update of the FSM
    [SerializeField]
    private float reactionTime = 1f;

    #region Conditions

    public bool MeleeOrderGiven()
    {
        if ( (newState != currentState) && (orderConfirmed) && (newState == State.MeleeAttack) )
        {
            currentState = newState;
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

    #region Actions

    // Maybe all the CheckDemons() can be avoided by putting only 1 CheckDemons() inside the FSMUpdate()
    public void MeleeAttack()
    {
        if ( !CheckDemons() )
            return;
        foreach ( GameObject demon in demons )
        {
            // This check must be done in every tactic
            if ( demon != null )
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
            Combat combat = demon.GetComponent<Combat>();
            combat.StopAttack();
            combat.combatManager.isIdle = true;
        }
    }

    public void Tank()
    {
        if ( !CheckDemons() )
            return;
        foreach ( GameObject demon in demons )
        {
            Combat combat = demon.GetComponent<Combat>();
            combat.StartBlock();
            //combat.combatManager.isIdle = true;
        }
    }

    public void StopTank()
    {
        if ( !CheckDemons() )
            return;
        foreach ( GameObject demon in demons )
        {
            Combat combat = demon.GetComponent<Combat>();
            combat.StopBlock();
            //combat.combatManager.isIdle = true;
        }
    }

    public void RangeAttack()
    {
        if ( !CheckDemons() )
            return;
        foreach ( GameObject demon in demons )
        {
            Combat combat = demon.GetComponent<Combat>();
            combat.Attack();
        }
    }

    public void Support()
    {

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

    // The coroutine that cycles through the FSM
    public IEnumerator MoveThroughFSM()
    {
        while ( true )
        {
            yield return new WaitForSeconds( reactionTime );
            groupFSM.Update();
        }
    }

    void Start()
    {
        currentState = State.MeleeAttack;
        // Just to test
        inCombat = true;

        demons = new GameObject[ 4 ];

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

        rangeAttackState.stayActions.Add( MeleeAttack );
        rangeAttackState.exitActions.Add( StopAttack );

        tankState.enterActions.Add( Tank );
        tankState.exitActions.Add( StopTank );

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
