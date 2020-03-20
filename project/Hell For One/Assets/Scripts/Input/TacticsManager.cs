using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class TacticsManager : MonoBehaviour
{
    #region Fields

    [Header( "Input" )]
    private bool cross, square, triangle, circle, L1, R1, L2, R2, R3 = false;

    [SerializeField]
    private GroupBehaviour.State currentShowedState;
    
    private GroupBehaviour.State[] tacticsArray;
    
    private Dictionary<GroupManager.Group, GameObject> groupsDict = new Dictionary<GroupManager.Group, GameObject>();

    [SerializeField]
    private GroupManager.Group currentMostRappresentedGroup;

    private int tacticsIndex = 0;

    private GroupsInRangeDetector groupsInRangeDetector;

    public GroupBehaviour.State CurrentShowedState { get => currentShowedState; private set => currentShowedState = value; }

    #endregion

    #region Properties

    /// <summary>
    /// Current most rappresented group in range
    /// </summary>
    public GroupManager.Group CurrentMostRappresentedGroup { get => currentMostRappresentedGroup; private set => currentMostRappresentedGroup = value; }

    #endregion
    
    #region Methods

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
            GroupBehaviour groupBehaviour = groupsDict[group].GetComponent<GroupBehaviour>();

            if(groupBehaviour != null) {
                groupBehaviour.AssignOrder(state);
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
    
    /// <summary>
    /// Assing order state to the most rappresented group in range
    /// </summary>
    /// <param name="state">The order to assign</param>
    /// <returns></returns>
    public bool AssignOrder(GroupBehaviour.State state)
    {
        bool canAssingOrder = false;

        if (groupsInRangeDetector != null) {
            canAssingOrder = GroupsInRangeDetector.MostRappresentedGroupInRange != GroupManager.Group.None;
        }
        else
        {
            Debug.LogError(this.gameObject.name + " TacticsManager - cannot find GroupsInRangeDetector");
        }

        if (canAssingOrder) {
            AssignOrderToGroup(state, GroupsInRangeDetector.MostRappresentedGroupInRange);
        }

        return canAssingOrder;
    }

    #endregion

    #region Unity methods

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


    #endregion

    #region Events handler

    private void OnMostRappresentedGroupChanged() { 
        currentMostRappresentedGroup = GroupsInRangeDetector.MostRappresentedGroupInRange;
    }

    #endregion
}
