using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class TacticsManager : MonoBehaviour
{
    #region Fields

    private bool canAssignOrder = true;
    
    private GroupManager.Group currentMostRepresentedGroup;

    #endregion

    #region Properties

    // TODO - GroupsInRangeDetector.MostRappresentedGroupInRange so this property is useless
    /// <summary>
    /// Current most rappresented group in range
    /// </summary>
    public GroupManager.Group CurrentMostRepresentedGroup { get => currentMostRepresentedGroup; private set => currentMostRepresentedGroup = value; }

    #endregion

    #region Delegates and events

    public delegate void OnTryOrderAssign(GroupBehaviour.State state, GroupManager.Group group);
    public static event OnTryOrderAssign onTryOrderAssign;

    #region Methods

    private void RaiseOnTryOrderAssign(GroupBehaviour.State state, GroupManager.Group group)
    {
        onTryOrderAssign?.Invoke(state,group);
    }

    #endregion
    
    #endregion
    
    #region Methods

    private void AssignOrderToGroup( GroupBehaviour.State state, GroupManager.Group group )
    {
        RaiseOnTryOrderAssign(state,group);
    }

    /// <summary>
    /// Assing order to all groups
    /// </summary>
    /// <param name="state">The order to assing</param>
    public void AllGroupsOrder(GroupBehaviour.State state )
    {
        if(canAssignOrder)
            AssignOrderToGroup( state, GroupManager.Group.All );
    }
    
    /// <summary>
    /// Assing order state to the most rappresented group in range
    /// </summary>
    /// <param name="state">The order to assign</param>
    /// <returns></returns>
    public bool AssignOrder(GroupBehaviour.State state)
    {
        bool hasAssignedOrder = false;

        if (canAssignOrder)
        {
            if (GroupsInRangeDetector.MostRappresentedGroupInRange != GroupManager.Group.None && GroupsInRangeDetector.MostRappresentedGroupInRange != GroupManager.Group.All) {
                AssignOrderToGroup(state, GroupsInRangeDetector.MostRappresentedGroupInRange);
                hasAssignedOrder = true;
            }    
        }
        
        return hasAssignedOrder;
    }

    #endregion

    #region Unity methods

    private void OnEnable()
    {
        GroupsInRangeDetector.RegisterOnMostRappresentedGroupChanged(OnMostRappresentedGroupChanged);

        StunReceiver stunReceiver = GetComponentInChildren<StunReceiver>();
        stunReceiver.onStartStun += OnStartStun;
        stunReceiver.onStopStun += OnStopStun;
    }

    private void OnDisable()
    {
        GroupsInRangeDetector.UnregisterOnMostRappresentedGroupChanged(OnMostRappresentedGroupChanged);
        
        StunReceiver stunReceiver = GetComponentInChildren<StunReceiver>();
        stunReceiver.onStartStun -= OnStartStun;
        stunReceiver.onStopStun -= OnStopStun;
    }

    #endregion

    #region Events handler

    private void OnStopStun()
    {
        canAssignOrder = true;
    }

    private void OnStartStun()
    {
        canAssignOrder = false;
    }
    
    private void OnMostRappresentedGroupChanged() { 
        currentMostRepresentedGroup = GroupsInRangeDetector.MostRappresentedGroupInRange;
    }

    #endregion
}
