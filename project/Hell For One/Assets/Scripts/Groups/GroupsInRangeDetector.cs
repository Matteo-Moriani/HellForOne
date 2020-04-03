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
    
    private static event Action OnMostRappresentedGroupChanged;

    private List<GroupManager.Group> groupsInRange = new List<GroupManager.Group>();

    private Dictionary<GroupManager.Group, int> impsInRange = new Dictionary<GroupManager.Group, int>();

    private static GroupManager.Group mostRappresentedGroupInRange = GroupManager.Group.None;

    /// <summary>
    /// List that contains all the groups in range of this Imp
    /// </summary>
    public List<GroupManager.Group> GroupsInRange { get => groupsInRange; private set => groupsInRange = value; }

    /// <summary>
    /// Dictionary that contains, for all groups, the number of imps in range
    /// </summary>
    public Dictionary<GroupManager.Group, int> ImpsInRange { get => impsInRange; private set => impsInRange = value; }

    /// <summary>
    /// Current most rappresented group in range
    /// </summary>
    public static GroupManager.Group MostRappresentedGroupInRange { get => mostRappresentedGroupInRange; private set => mostRappresentedGroupInRange = value; }

    private void Awake()
    {
        mostRappresentedGroupInRange = GroupManager.Group.None;    
    }

    private void OnEnable()
    {
        foreach (GroupManager.Group group in (GroupManager.Group[])Enum.GetValues(typeof(GroupManager.Group)))
        {
            if (group != GroupManager.Group.None && group != GroupManager.Group.All)
            {
                impsInRange.Add(group, 0);
            }
        }
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
        GroupManager.Group mostRappresentedGroup = GroupManager.Group.None;

        int temp = 0;

        foreach (KeyValuePair<GroupManager.Group, int> item in impsInRange)
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
            Stats stats = other.transform.root.gameObject.GetComponent<Stats>();

            if(stats != null) {
                if (!stats.IsDying) {
                    GroupFinder groupFinder = other.gameObject.GetComponent<GroupFinder>();

                    if (groupFinder != null)
                    {
                        GroupManager groupManager = groupFinder.GroupBelongingTo.GetComponent<GroupManager>();

                        if (groupManager != null)
                        {
                            switch (action)
                            {
                                case Actions.Add:
                                    // Update Groups in range
                                    if (!groupsInRange.Contains(groupManager.ThisGroupName))
                                    {
                                        groupsInRange.Add(groupManager.ThisGroupName);
                                    }

                                    // Update Imps in range
                                    if (impsInRange[groupManager.ThisGroupName] < 4)
                                    {
                                        impsInRange[groupManager.ThisGroupName]++;
                                    }
                                    else
                                    {
                                        Debug.LogError(this.transform.root.name + " GroupInRangeDetector is trying to decrease " + groupManager.ThisGroupName + " count but it is already 4");
                                    }

                                    UpdateMostRappresentedGroup();

                                    break;
                                case Actions.Remove:
                                    // Update Groups in range
                                    if (groupsInRange.Contains(groupManager.ThisGroupName))
                                    {
                                        if(impsInRange[groupManager.ThisGroupName] == 1) {
                                            groupsInRange.Remove(groupManager.ThisGroupName);
                                        }
                                    }

                                    // Update Imps in range
                                    if (impsInRange[groupManager.ThisGroupName] > 0)
                                    {
                                        impsInRange[groupManager.ThisGroupName]--;
                                    }
                                    else
                                    {
                                        Debug.LogError(this.transform.root.name + " GroupInRangeDetector is trying to decrease " + groupManager.ThisGroupName + " count but it is already 0");
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
        }
    }

    /// <summary>
    /// Checks if group is in range
    /// </summary>
    /// <param name="group">The group to check</param>
    /// <returns></returns>
    public bool IsTheGroupInRange(GroupManager.Group group)
    {
        switch (group)
        {
            case GroupManager.Group.GroupAzure:
                if (groupsInRange.Contains(GroupManager.Group.GroupAzure))
                {
                    return true;
                }
                break;
            case GroupManager.Group.GroupGreen:
                if (groupsInRange.Contains(GroupManager.Group.GroupGreen))
                {
                    return true;
                }
                break;
            case GroupManager.Group.GroupPink:
                if (groupsInRange.Contains(GroupManager.Group.GroupPink))
                {
                    return true;
                }
                break;
            case GroupManager.Group.GroupYellow:
                if (groupsInRange.Contains(GroupManager.Group.GroupYellow))
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
    public static void RegisterOnMostRappresentedGroupChanged(Action method)
    {
        OnMostRappresentedGroupChanged += method;
    }

    /// <summary>
    /// Unregister method to onMostRappresentedGroupChanged event
    /// </summary>
    /// <param name="method">The method to unregister</param>
    public static void UnregisterOnMostRappresentedGroupChanged(Action method)
    {
        OnMostRappresentedGroupChanged -= method;
    }

    /// <summary>
    /// Decrease imps in range count for group
    /// </summary>
    /// <param name="group">The group to decrease</param>
    public void DecreaseImpInRangeCount(GroupManager.Group group) { 
        if(impsInRange[group] > 0) {
            impsInRange[group]--;
        }
        else { 
            Debug.LogError(this.transform.root.name + " " + this.name + " DecreaseImpsInRangeCount is trying to decrease imps number but it is 0");   
        }    
    }

    private void RaiseOnMostRappresentedGroupChanged()
    {
        if (OnMostRappresentedGroupChanged != null)
        {
            OnMostRappresentedGroupChanged();
        }
    }
}
