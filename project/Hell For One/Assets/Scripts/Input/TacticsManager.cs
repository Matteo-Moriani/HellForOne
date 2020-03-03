using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class TacticsManager : MonoBehaviour
{   
    /*
    public enum Group
    {
        GroupAzure,
        GroupPink,
        GroupGreen,
        GroupYellow
    }
    */

    [Header( "Input" )]
    private bool cross, square, triangle, circle, L1, R1, L2, R2, R3 = false;

    [SerializeField]
    private GroupBehaviour.State currentShowedState;
    private GroupBehaviour.State[] tacticsArray;
    //private Group[] groupsArray;
    private GroupBehaviour.Group[] groupsArray;
    [SerializeField]
    //private Group currentShowedGroup;
    private GroupBehaviour.Group currentShowedGroup;

    private int tacticsIndex, groupsIndex = 0;

    private GroupsInRangeDetector groupsInRangeDetector;

    public GroupBehaviour.State CurrentShowedState { get => currentShowedState; set => currentShowedState = value; }
    public GroupBehaviour.Group CurrentShowedGroup { get => currentShowedGroup; set => currentShowedGroup = value; }

    public void FillArrays()
    {
        tacticsArray = new GroupBehaviour.State[ 4 ];
        foreach ( GroupBehaviour.State s in ( GroupBehaviour.State[] ) Enum.GetValues( typeof( GroupBehaviour.State ) ) )
        {
            tacticsArray[ tacticsIndex ] = s;
            tacticsIndex++;
        }

        groupsArray = new GroupBehaviour.Group[ 4 ];
        foreach ( GroupBehaviour.Group g in ( GroupBehaviour.Group[] ) Enum.GetValues( typeof( GroupBehaviour.Group ) ) )
        {
            groupsArray[ groupsIndex ] = g;
            groupsIndex++;
        }
    }

    public int IncrementCircularArrayIndex( int index, int arrayLength )
    {
        return (index + 1) % arrayLength;
    }

    public int DecrementCircularArrayIndex( int index, int arrayLength )
    {
        return (index + arrayLength - 1) % arrayLength;
    }

    public void ConfirmOrder()
    {
        cross = true;
    }

    public void AssignOrderToGroup( GroupBehaviour.State state, GroupBehaviour.Group group )
    {
        // TODO - optimize this
        GroupBehaviour groupBehaviour = GameObject.Find( "Group" + group.ToString() ).GetComponent<GroupBehaviour>();
        
        if ( groupBehaviour.groupFSM.current.stateName != state.ToString() )
        {
            groupBehaviour.newState = state;
            groupBehaviour.orderConfirmed = true;
        }
    }

    public void AllGroupsOrder(GroupBehaviour.State state )
    {
        AssignOrderToGroup( state, GroupBehaviour.Group.Azure );
        AssignOrderToGroup( state, GroupBehaviour.Group.Pink );
        AssignOrderToGroup( state, GroupBehaviour.Group.Green );
        AssignOrderToGroup( state, GroupBehaviour.Group.Yellow );
    }

    public void RotateRightGroups()
    {
        groupsIndex = IncrementCircularArrayIndex( groupsIndex, groupsArray.Length );
        CurrentShowedGroup = groupsArray[ groupsIndex ];
        //Debug.Log( CurrentShowedGroup );
    }

    public void RotateLeftGroups()
    {
        groupsIndex = DecrementCircularArrayIndex( groupsIndex, groupsArray.Length );
        CurrentShowedGroup = groupsArray[ groupsIndex ];
        //Debug.Log( CurrentShowedState );
    }

    public bool AssignOrder(GroupBehaviour.State state)
    {
        bool canAssingOrder = false;

        if (groupsInRangeDetector != null) {
            canAssingOrder = groupsInRangeDetector.IsTheGroupInRange(currentShowedGroup);
        }
        else
        {
            Debug.LogError(this.gameObject.name + " TacticsManager - cannot find GroupsInRangeDetector");
        }

        if (canAssingOrder) {
            AssignOrderToGroup(state, CurrentShowedGroup);
        }

        return canAssingOrder;
    }

    void Start()
    {
        FillArrays();
        groupsIndex = 0;
        tacticsIndex = 0;
        CurrentShowedState = tacticsArray[ tacticsIndex ];
        CurrentShowedGroup = groupsArray[ groupsIndex ];

        groupsInRangeDetector = this.gameObject.GetComponentInChildren<GroupsInRangeDetector>(true);
    }
}
