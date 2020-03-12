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
    
    // TODO -   Replaced with a dictionary in order to avoid a lot of GameObject.Find
    //          remove this if the new solution works better
    //private Group[] groupsArray;
    
    private Dictionary<GroupManager.Group, GameObject> groupsDict = new Dictionary<GroupManager.Group, GameObject>();

    [SerializeField]
    //private Group currentShowedGroup;
    private GroupManager.Group currentMostRappresentedGroup;

    private int tacticsIndex = 0;

    // TODO - used for HUD and rotate groups, remove this
    //private int groupsIndex = 0;

    private GroupsInRangeDetector groupsInRangeDetector;

    public GroupBehaviour.State CurrentShowedState { get => currentShowedState; set => currentShowedState = value; }
    
    /// <summary>
    /// Current most rappresented group in range
    /// </summary>
    public GroupManager.Group CurrentMostRappresentedGroup { get => currentMostRappresentedGroup; private set => currentMostRappresentedGroup = value; }

    public void FillArrays()
    {
        tacticsArray = new GroupBehaviour.State[ 5 ];
        foreach ( GroupBehaviour.State s in ( GroupBehaviour.State[] ) Enum.GetValues( typeof( GroupBehaviour.State ) ) )
        {
            tacticsArray[ tacticsIndex ] = s;
            tacticsIndex++;
        }

        foreach (GroupManager.Group g in (GroupManager.Group[])Enum.GetValues(typeof(GroupManager.Group)))
        {
            if (g != GroupManager.Group.None)
            {
                groupsDict[g] = GameObject.Find(g.ToString());
            }
            else
            {
                Debug.Log(this.gameObject.name + " TacticsManager.FillArrays is ignoring GroupBehaviour.Group.None");
            }
        }

    }

    private void AssignOrderToGroup( GroupBehaviour.State state, GroupManager.Group group )
    {
        if(group != GroupManager.Group.None) {
            // TODO - optimize this
            // GroupBehaviour groupBehaviour = GameObject.Find( group.ToString() ).GetComponent<GroupBehaviour>();

            GroupBehaviour groupBehaviour = groupsDict[group].GetComponent<GroupBehaviour>();

            if(groupBehaviour != null) {
                if (groupBehaviour.groupFSM.current.stateName != state.ToString())
                {
                    groupBehaviour.newState = state;
                    groupBehaviour.orderConfirmed = true;
                }
            }
            else { 
                Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find " + group.ToString() + " GroupBehaviour");    
            }
        }
        else { 
            Debug.LogError(this.gameObject.name + " TacticsManager.AssignOrderToGroup is trying to assign order to None group");    
        }
    }

    /// <summary>
    /// Assing order to all groups
    /// </summary>
    /// <param name="state">The order to assing</param>
    public void AllGroupsOrder(GroupBehaviour.State state )
    {
        AssignOrderToGroup( state, GroupManager.Group.GroupAzure );
        AssignOrderToGroup( state, GroupManager.Group.GroupPink );
        AssignOrderToGroup( state, GroupManager.Group.GroupGreen );
        AssignOrderToGroup( state, GroupManager.Group.GroupYellow );
    }

    // TODO - used for HUD and rotate groups, remove this 
    public void RotateRightGroups()
    {
        /*
        groupsIndex = IncrementCircularArrayIndex( groupsIndex, groupsDict.Length );
        CurrentShowedGroup = groupsDict[ groupsIndex ];
        //Debug.Log( CurrentShowedGroup );
        */
    }

    // TODO - used for HUD and rotate groups, remove this
    public void RotateLeftGroups()
    {
        /*
        groupsIndex = DecrementCircularArrayIndex( groupsIndex, groupsDict.Length );
        CurrentShowedGroup = groupsDict[ groupsIndex ];
        //Debug.Log( CurrentShowedState );
        */
    }
    
    /// <summary>
    /// Assing order state to the most rappresented group in range
    /// </summary>
    /// <param name="state">The order to assign</param>
    /// <returns></returns>
    public bool AssignOrder(GroupBehaviour.State state)
    {
        bool canAssingOrder = false;

        if (groupsInRangeDetector != null) {
            //canAssingOrder = groupsInRangeDetector.IsTheGroupInRange(currentShowedGroup);

            canAssingOrder = GroupsInRangeDetector.MostRappresentedGroupInRange != GroupManager.Group.None;
        }
        else
        {
            // Insert here code to manage oder assign to None group

            Debug.LogError(this.gameObject.name + " TacticsManager - cannot find GroupsInRangeDetector");
        }

        if (canAssingOrder) {
            AssignOrderToGroup(state, GroupsInRangeDetector.MostRappresentedGroupInRange);
        }

        return canAssingOrder;
    }

    private void Awake()
    {
        groupsInRangeDetector = this.gameObject.GetComponentInChildren<GroupsInRangeDetector>(true);
    }

    private void OnEnable()
    {
        GroupsInRangeDetector.RegisterOnMostRappresentedGroupChanged(OnMostRappresentedGroupChanged);
    }

    private void OnDisable()
    {
        GroupsInRangeDetector.UnregisterOnMostRappresentedGroupChanged(OnMostRappresentedGroupChanged);
    }

    private void Start()
    {
        FillArrays();
        //groupsIndex = 0;
        tacticsIndex = 0;
        CurrentShowedState = tacticsArray[ tacticsIndex ];
        //CurrentShowedGroup = groupsDict[ groupsIndex ];
    }

    private void OnMostRappresentedGroupChanged() { 
        currentMostRappresentedGroup = GroupsInRangeDetector.MostRappresentedGroupInRange;
    }
}
