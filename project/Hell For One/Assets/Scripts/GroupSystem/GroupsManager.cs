using System;
using System.Collections.Generic;
using UnityEngine;

namespace GroupSystem
{
    public class GroupsManager : MonoBehaviour
    {
        public Dictionary<GroupManager.Group, GameObject> Groups { get; private set; } =
            new Dictionary<GroupManager.Group, GameObject>();
        public static GroupsManager Instance { get; private set; }

        private void Awake()
        {
            if(Instance != null && Instance != this) { 
                Destroy(this.gameObject);
            }
            else { 
                Instance = this;
            }
            
            GameObject[] groups = GameObject.FindGameObjectsWithTag("Group");

            foreach (GameObject group in groups)
            {
                Groups.Add(group.GetComponent<GroupManager>().ThisGroupName,group);
            }
        }
    }
}
