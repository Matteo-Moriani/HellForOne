using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupsManager : MonoBehaviour
{
    public GameObject[] Groups { get; private set; }
    public static GroupsManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this) { 
            Destroy(this.gameObject);
        }
        else { 
            Instance = this;
        }

        Groups = GameObject.FindGameObjectsWithTag("Group");
    }
}
