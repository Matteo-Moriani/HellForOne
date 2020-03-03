using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupsInRangeDetector : MonoBehaviour
{
    private enum Actions{ 
        Add,
        Remove
    }
    
    [SerializeField]
    [Tooltip("The range of this Imp's group detection")]
    private float detectionRange = 1.0f;

    private List<GroupBehaviour.Group> groupsInRange = new List<GroupBehaviour.Group>();
    
    /// <summary>
    /// List that contains all the groups in range of this Imp
    /// </summary>
    public List<GroupBehaviour.Group> GroupsInRange { get => groupsInRange; private set => groupsInRange = value; }

    private void Start()
    {
        this.transform.localScale = new Vector3(detectionRange,detectionRange,detectionRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        ManageRange(GroupsInRangeDetector.Actions.Add, other);    
    }

    private void OnTriggerExit(Collider other)
    {
        ManageRange(GroupsInRangeDetector.Actions.Remove, other);   
    }

    private void ManageRange(GroupsInRangeDetector.Actions action, Collider other) {
        if (other.gameObject.tag == "Demon")
        {
            DemonBehaviour demonBehaviour = other.gameObject.GetComponent<DemonBehaviour>();

            if (demonBehaviour != null)
            {
                GroupBehaviour groupBehaviour = demonBehaviour.groupBelongingTo.GetComponent<GroupBehaviour>();

                if (groupBehaviour != null)
                {
                    switch (action) { 
                        case Actions.Add:
                            if (!groupsInRange.Contains(groupBehaviour.ThisGroupName))
                            {
                                groupsInRange.Add(groupBehaviour.ThisGroupName);

                                // TODO - Debug for testing, remove this
                                Debug.Log(groupBehaviour.ThisGroupName + " added to aviable groups for " + this.transform.root.gameObject.name);
                            }

                            break;
                        case Actions.Remove:
                            if (groupsInRange.Contains(groupBehaviour.ThisGroupName))
                            {
                                groupsInRange.Remove(groupBehaviour.ThisGroupName);
                                
                                // TODO - Debug for testing, remove this
                                Debug.Log(groupBehaviour.ThisGroupName + " removed from aviable groups for " + this.transform.root.gameObject.name);
                            }

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
