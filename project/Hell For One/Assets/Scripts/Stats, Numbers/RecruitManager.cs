using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

public class RecruitManager : MonoBehaviour
{
    // TODO - Optimize
    //private GroupBehaviour[] groups = new GroupBehaviour[ 4 ];
    private AllyDemonSpawnerTest allyDemonSpawnerTest;
    private bool timerStarted;
    private float timeWhenTimerStarted;
    // The time in which the next imp will be spawned
    private float timerEnd;

    // Just counts the total imps recruiting for all groups
    public int CountImpsRecruiting()
    {
        int impsRecruiting = 0;
        foreach ( GameObject group in GroupsManager.Instance.Groups )
        {
            GroupBehaviour groupBehaviour = group.GetComponent<GroupBehaviour>();
            GroupManager groupManager = group.GetComponent<GroupManager>();

            if ( groupBehaviour.currentState == GroupBehaviour.State.Recruit )
            {
                impsRecruiting += groupManager.ImpsInGroupNumber;
            }
        }

        return impsRecruiting;
    }

    void Awake()
    {
        //groups[ 0 ] = GameObject.Find( "GroupAzure" ).GetComponent<GroupBehaviour>();
        //groups[ 1 ] = GameObject.Find( "GroupPink" ).GetComponent<GroupBehaviour>();
        //groups[ 2 ] = GameObject.Find( "GroupGreen" ).GetComponent<GroupBehaviour>();
        //groups[ 3 ] = GameObject.Find( "GroupYellow" ).GetComponent<GroupBehaviour>();

        allyDemonSpawnerTest = GameObject.Find( "AllyDemonSpawner" ).GetComponent<AllyDemonSpawnerTest>();
    }

    void Update()
    {
        // Check if in Battle
        if ( BattleEventsHandler.IsInBattle )
        {
            // Imps can be spawned
            if ( CountImpsRecruiting() != 0 && AlliesManager.Instance.AlliesList.Count < 16 )
            {
                float timeTillNextImp = -15 / 11 * CountImpsRecruiting() + 25;

                if ( !timerStarted )
                {
                    timerStarted = true;
                    timeWhenTimerStarted = Time.time;
                    //Debug.Log( "Timer Started at: " + timeWhenTimerStarted.ToString() );
                }

                timerEnd = timeWhenTimerStarted + timeTillNextImp;
                //Debug.Log( "Imp should spawn around: " + timerEnd.ToString() );

                // Spawn imp and reset fields
                if ( Time.time >= timerEnd )
                {
                    timerStarted = false;
                    allyDemonSpawnerTest.Spawn();
                    //Debug.Log( "Imp spawned at: " + Time.time.ToString() );
                }
            }
        }
    }
}
