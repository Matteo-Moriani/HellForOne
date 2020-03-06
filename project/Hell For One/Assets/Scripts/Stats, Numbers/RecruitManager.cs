using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitManager : MonoBehaviour
{
    private GroupBehaviour[] groups = new GroupBehaviour[ 4 ];
    private AllyDemonSpawnerTest allyDemonSpawnerTest;
    private bool timerStarted;
    private float timeWhenTimerStarted;
    // The time in which the next imp will be spawned
    private float timerEnd;

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

            if ( !timerStarted )
            {
                timerStarted = true;
                timeWhenTimerStarted = Time.time;
            }

            timerEnd = timeWhenTimerStarted + timeTillNextImp;

            // Spawn imp and reset fields
            if (Time.time >= timerEnd )
            {
                timerStarted = false;
                allyDemonSpawnerTest.Spawn();
            }
        }
    }
}
