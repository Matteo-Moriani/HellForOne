using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupsManager : MonoBehaviour
{
    private GameObject[] groups;
    private static GroupsManager instance;

    public GameObject[] Groups { get => groups; private set => groups = value; }
    public static GroupsManager Instance { get => instance; private set => instance = value; }

    private void Awake()
    {
        if(instance != null && instance != this) { 
            Destroy(this.gameObject);
        }
        else { 
            instance = this;
        }

        Groups = GameObject.FindGameObjectsWithTag("Group");
    }
}
