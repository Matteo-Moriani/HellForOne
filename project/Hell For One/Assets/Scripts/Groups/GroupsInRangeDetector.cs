using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GroupsInRangeDetector : MonoBehaviour
{
    private enum Actions
    {
        Add,
        Remove
    }

    [SerializeField]
    [Tooltip("The range of this Imp's group detection")]
    private float detectionRange = 1.0f;

    /// <summary>
    /// Delegate for the OnMostRappresentedGroupChanged event
    /// </summary>
    //public delegate void OnMostRappresentedGroupChanged();

    //private event OnMostRappresentedGroupChanged onMostRappresentedGroupChanged;

    private event Action onMostRappresentedGroupChanged;

    private List<GroupBehaviour.Group> groupsInRange = new List<GroupBehaviour.Group>();

    private Dictionary<GroupBehaviour.Group, int> impsInRange = new Dictionary<GroupBehaviour.Group, int>();

    private GroupBehaviour.Group mostRappresentedGroupInRange = GroupBehaviour.Group.None;

    /// <summary>
    /// List that contains all the groups in range of this Imp
    /// </summary>
    public List<GroupBehaviour.Group> GroupsInRange { get => groupsInRange; private set => groupsInRange = value; }

    /// <summary>
    /// Dictionary that contains, for all groups, the number of imps in range
    /// </summary>
    public Dictionary<GroupBehaviour.Group, int> ImpsInRange { get => impsInRange; private set => impsInRange = value; }

    /// <summary>
    /// Current most rappresented group in range
    /// </summary>
    public GroupBehaviour.Group MostRappresentedGroupInRange { get => mostRappresentedGroupInRange; private set => mostRappresentedGroupInRange = value; }

    private void OnEnable()
    {
        foreach (GroupBehaviour.Group group in (GroupBehaviour.Group[])Enum.GetValues(typeof(GroupBehaviour.Group)))
        {
            if (group != GroupBehaviour.Group.None)
            {
                impsInRange.Add(group, 0);
            }
        }

        RegisterOnMostRappresentedGroupChanged(PrintMostRappresentedGroup);
    }

    private void OnDisable()
    {
        UnregisterOnMostRappresentedGroupChanged(PrintMostRappresentedGroup);
    }

    private void Start()
    {
        this.transform.localScale = new Vector3(detectionRange, detectionRange, detectionRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        ManageRange(GroupsInRangeDetector.Actions.Add, other);
    }

    private void OnTriggerExit(Collider other)
    {
        ManageRange(GroupsInRangeDetector.Actions.Remove, other);
    }

    private void UpdateMostRappresentedGroup() {
        GroupBehaviour.Group mostRappresentedGroup = GroupBehaviour.Group.None;

        int temp = 0;

        foreach (KeyValuePair<GroupBehaviour.Group, int> item in impsInRange)
        {

            if (item.Value > temp)
            {
                temp = item.Value;

                mostRappresentedGroup = item.Key;
            }
        }

        if (mostRappresentedGroup != mostRappresentedGroupInRange)
        {
            MostRappresentedGroupInRange = mostRappresentedGroup;

            RaiseOnMostRappresentedGroupChanged();
        }
    }

    private void ManageRange(GroupsInRangeDetector.Actions action, Collider other)
    {
        if (other.gameObject.tag == "Demon")
        {
            DemonBehaviour demonBehaviour = other.gameObject.GetComponent<DemonBehaviour>();

            if (demonBehaviour != null)
            {
                GroupBehaviour groupBehaviour = demonBehaviour.groupBelongingTo.GetComponent<GroupBehaviour>();

                if (groupBehaviour != null)
                {
                    switch (action)
                    {
                        case Actions.Add:
                            // Update Groups in range
                            if (!groupsInRange.Contains(groupBehaviour.ThisGroupName))
                            {
                                groupsInRange.Add(groupBehaviour.ThisGroupName);

                                // TODO - Debug for testing, remove this
                                Debug.Log(groupBehaviour.ThisGroupName + " added to aviable groups for " + this.transform.root.gameObject.name);
                            }

                            // Update Imps in range
                            if (impsInRange[groupBehaviour.ThisGroupName] < 4)
                            {
                                impsInRange[groupBehaviour.ThisGroupName]++;

                                // TODO - Debug for testing, remove this
                                Debug.Log(this.transform.root.name + " GroupInRangeDetector - ImpsInRange[" + groupBehaviour + "]:" + impsInRange[groupBehaviour.ThisGroupName]);
                            }
                            else
                            {
                                Debug.LogError(this.transform.root.name + " GroupInRangeDetector is trying to decrease " + groupBehaviour.ThisGroupName + " count but it is already 4");
                            }

                            UpdateMostRappresentedGroup();

                            break;
                        case Actions.Remove:
                            // Update Groups in range
                            if (groupsInRange.Contains(groupBehaviour.ThisGroupName))
                            {
                                groupsInRange.Remove(groupBehaviour.ThisGroupName);

                                // TODO - Debug for testing, remove this
                                Debug.Log(groupBehaviour.ThisGroupName + " removed from aviable groups for " + this.transform.root.gameObject.name);
                            }

                            // Update Imps in range
                            if (impsInRange[groupBehaviour.ThisGroupName] > 0)
                            {
                                impsInRange[groupBehaviour.ThisGroupName]--;

                                // TODO - Debug for testing, remove this
                                Debug.Log(this.transform.root.name + " GroupInRangeDetector - ImpsInRange[" + groupBehaviour + "]:" + impsInRange[groupBehaviour.ThisGroupName]);
                            }
                            else
                            {
                                Debug.LogError(this.transform.root.name + " GroupInRangeDetector is trying to decrease " + groupBehaviour.ThisGroupName + " count but it is already 0");
                            }

                            UpdateMostRappresentedGroup();

                            break;
                    }
                }
                else
                {
                    Debug.LogError("GroupInRangeDetector - " + other.name + " cannot find GroupBehaviour of his group");
                }
            }
            else
            {
                Debug.LogError("GroupInRangeDetector - " + other.name + " does not have DemonBehaviour attached");
            }
        }
    }

    /// <summary>
    /// Checks if group is in range
    /// </summary>
    /// <param name="group">The group to check</param>
    /// <returns></returns>
    public bool IsTheGroupInRange(GroupBehaviour.Group group)
    {
        switch (group)
        {
            case GroupBehaviour.Group.GroupAzure:
                if (groupsInRange.Contains(GroupBehaviour.Group.GroupAzure))
                {
                    return true;
                }
                break;
            case GroupBehaviour.Group.GroupGreen:
                if (groupsInRange.Contains(GroupBehaviour.Group.GroupGreen))
                {
                    return true;
                }
                break;
            case GroupBehaviour.Group.GroupPink:
                if (groupsInRange.Contains(GroupBehaviour.Group.GroupPink))
                {
                    return true;
                }
                break;
            case GroupBehaviour.Group.GroupYellow:
                if (groupsInRange.Contains(GroupBehaviour.Group.GroupYellow))
                {
                    return true;
                }
                break;
        }
        return false;
    }

    /// <summary>
    /// Register method to onMostRappresentedGroupChanged event
    /// </summary>
    /// <param name="method">The method to register</param>
    public void RegisterOnMostRappresentedGroupChanged(Action method)
    {
        onMostRappresentedGroupChanged += method;
    }

    /// <summary>
    /// Unregister method to onMostRappresentedGroupChanged event
    /// </summary>
    /// <param name="method">The method to unregister</param>
    public void UnregisterOnMostRappresentedGroupChanged(Action method)
    {
        onMostRappresentedGroupChanged -= method;
    }

    private void RaiseOnMostRappresentedGroupChanged()
    {
        if (onMostRappresentedGroupChanged != null)
        {
            onMostRappresentedGroupChanged();
        }
    }

    // TODO - used for testing, remove this
    private void PrintMostRappresentedGroup() { 
        Debug.Log("Most rappresented group: " + mostRappresentedGroupInRange.ToString());    
    }
}
