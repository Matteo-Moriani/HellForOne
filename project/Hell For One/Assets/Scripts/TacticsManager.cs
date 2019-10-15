using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsManager : MonoBehaviour
{
    public enum Group
    {
        GroupA,
        GroupB,
        GroupC,
        GroupD
    }

    private GroupBehaviour.State[] tacticArray;
    private Group[] groupsArray;

    private int tacticsIndex, groupsIndex = 0;

    void Start()
    {
        tacticArray = new GroupBehaviour.State[ 4 ];
        foreach(GroupBehaviour.State s in (GroupBehaviour.State[]) Enum.GetValues( typeof( GroupBehaviour.State ) ) )
        {
            tacticArray[ tacticsIndex ] = s;
            tacticsIndex ++;
        }

        groupsArray
    }

    void Update()
    {
        
    }
}
