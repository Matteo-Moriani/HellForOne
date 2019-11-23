using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class TacticsManager : MonoBehaviour
{
    public enum Group
    {
        GroupAzure,
        GroupPink,
        GroupGreen,
        GroupYellow
    }

    [Header( "Input" )]
    private bool cross, square, triangle, circle, L1, R1, L2, R2, R3 = false;

    [SerializeField]
    private GroupBehaviour.State currentShowedState;
    private GroupBehaviour.State[] tacticsArray;
    private Group[] groupsArray;
    [SerializeField]
    private Group currentShowedGroup;

    private int tacticsIndex, groupsIndex = 0;

    public GroupBehaviour.State CurrentShowedState { get => currentShowedState; set => currentShowedState = value; }
    public Group CurrentShowedGroup { get => currentShowedGroup; set => currentShowedGroup = value; }

    public void FillArrays()
    {
        tacticsArray = new GroupBehaviour.State[ 4 ];
        foreach ( GroupBehaviour.State s in ( GroupBehaviour.State[] ) Enum.GetValues( typeof( GroupBehaviour.State ) ) )
        {
            tacticsArray[ tacticsIndex ] = s;
            tacticsIndex++;
        }

        groupsArray = new Group[ 4 ];
        foreach ( Group g in ( Group[] ) Enum.GetValues( typeof( Group ) ) )
        {
            groupsArray[ groupsIndex ] = g;
            groupsIndex++;
        }
    }

    public int IncrementCircularArrayIndex( int index, int arrayLength )
    {
        return (index + 1) % arrayLength;
    }

    public void ConfirmOrder()
    {
        cross = true;
    }

    public void AssignOrderToGroup( GroupBehaviour.State state, Group group )
    {
        GroupBehaviour groupBehaviour = GameObject.Find( CurrentShowedGroup.ToString() ).GetComponent<GroupBehaviour>();
        if (groupBehaviour.groupFSM.current.stateName != state.ToString())
        {
            groupBehaviour.newState = state;
            groupBehaviour.orderConfirmed = true;
        }
    }

    void Start()
    {
        FillArrays();
        groupsIndex = 0;
        tacticsIndex = 0;
        CurrentShowedState = tacticsArray[ tacticsIndex ];
        CurrentShowedGroup = groupsArray[ groupsIndex ];
    }

    void Update()
    {
        cross = Input.GetButtonDown( "cross" );
        square = Input.GetButtonDown( "square" );
        triangle = Input.GetButtonDown( "triangle" );
        circle = Input.GetButtonDown( "circle" );
        L1 = Input.GetButtonDown( "L1" );
        R1 = Input.GetButtonDown( "R1" );
        L2 = Input.GetButtonDown( "L2" );
        R2 = Input.GetButtonDown( "R2" );
        R3 = Input.GetButtonDown( "R3" );

        if ( cross )
        {
            AssignOrderToGroup( CurrentShowedState, CurrentShowedGroup );
        }

        if ( L2 )
        {
            groupsIndex = IncrementCircularArrayIndex( groupsIndex, groupsArray.Length );
            CurrentShowedGroup = groupsArray[ groupsIndex ];
            Debug.Log( CurrentShowedGroup );
        }

        if ( R2 )
        {
            tacticsIndex = IncrementCircularArrayIndex( tacticsIndex, tacticsArray.Length );
            CurrentShowedState = tacticsArray[ IncrementCircularArrayIndex( tacticsIndex, tacticsArray.Length ) ];
            Debug.Log( CurrentShowedState );
        }
    }
}
