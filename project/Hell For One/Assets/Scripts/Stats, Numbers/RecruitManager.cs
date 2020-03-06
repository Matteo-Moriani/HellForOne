using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitManager : MonoBehaviour
{
    private GroupBehaviour[] groups = new GroupBehaviour[ 4 ];
    private AllyDemonSpawnerTest allyDemonSpawnerTest;
    private bool timerStarted;
    private Time time;

    // Just counts the total imps recruiting for all groups
    public int CountImpsRecruiting()
    {
        int impsRecruiting = 0;
        foreach (GroupBehaviour groupBehaviour in groups )
        {
            if (groupBehaviour.currentState == GroupBehaviour.State.Recruit )
            {
                impsRecruiting += groupBehaviour.DemonsInGroup;
            }
        }

        return impsRecruiting;
    }

    void Awake()
    {
        groups[0] = GameObject.Find( "GroupAzure" ).GetComponent<GroupBehaviour>();
        groups[1] = GameObject.Find( "GroupPink" ).GetComponent<GroupBehaviour>();
        groups[2] = GameObject.Find( "GroupGreen" ).GetComponent<GroupBehaviour>();
        groups[3] = GameObject.Find( "GroupYellow" ).GetComponent<GroupBehaviour>();

        allyDemonSpawnerTest = GameObject.Find( "AllyDemonSpawner" ).GetComponent<AllyDemonSpawnerTest>();
    }

    void Update()
    {
        // Imps can be spawned
        if ( CountImpsRecruiting() != 0 && AlliesManager.Instance.AlliesList.Count < 16)
        {
            float timeTillNextImp = -15 / 11 * CountImpsRecruiting() + 25;
            
        }
    }
}
