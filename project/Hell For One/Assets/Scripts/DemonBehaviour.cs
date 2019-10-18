using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject groupBelongingTo;
    private bool groupFound = false;

    public void FindGroup()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag( "group" );
        foreach ( GameObject g in go )
        {
            GameObject[] demonsArray = g.GetComponent<GroupBehaviour>().demons;
            for ( int i = 0; i < demonsArray.Length; i++ )
            {
                if ( demonsArray[ i ] == null )
                {
                    demonsArray[ i ] = gameObject;
                    groupFound = true;
                    break;
                }
            }
            if ( groupFound )
            {
                groupBelongingTo = g;
                break;
            }
        }
    }

    private void Start()
    {
        FindGroup();
    }

    private void Update()
    {
        if ( !groupFound )
            FindGroup();
    }

}
