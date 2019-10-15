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

    [Header( "Input" )]
    [SerializeField]
    private bool cross, square, triangle, circle, L1, R1, L2, R2, R3;

    private GroupBehaviour.State[] tacticArray;
    private Group[] groupsArray;

    private int tacticsIndex, groupsIndex = 0;

    public void FillArrays()
    {
        tacticArray = new GroupBehaviour.State[ 4 ];
        foreach ( GroupBehaviour.State s in ( GroupBehaviour.State[] ) Enum.GetValues( typeof( GroupBehaviour.State ) ) )
        {
            tacticArray[ tacticsIndex ] = s;
            tacticsIndex++;
        }

        groupsArray = new Group[ 4 ];
        foreach ( Group g in ( Group[] ) Enum.GetValues( typeof( Group ) ) )
        {
            groupsArray[ groupsIndex ] = g;
            groupsIndex++;
        }
    }

    void Start()
    {
        FillArrays();
    }

    void Update()
    {
        cross = Input.GetButton( "cross" );
        square = Input.GetButton( "square" );
        triangle = Input.GetButtonDown( "triangle" );
        circle = Input.GetButtonDown( "circle" );
        L1 = Input.GetButtonDown( "L1" );
        R1 = Input.GetButtonDown( "R1" );
        L2 = Input.GetButtonDown( "L2" );
        R2 = Input.GetButtonDown( "R2" );
        R3 = Input.GetButtonDown( "R3" );


    }
}
