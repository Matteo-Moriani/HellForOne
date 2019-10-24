using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class DemonBehaviour : MonoBehaviour
{
    [SerializeField]
    public GameObject groupBelongingTo;
    public bool groupFound = false;

    private void Start()
    {
        FindGroup();
    }

    private void Update()
    {
        if ( !groupFound )
            FindGroup();
    }

    // Balances group entering too
    public void FindGroup()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag( "Group" );
        GameObject bestGroup = null;
        int maxFreeSlots = 0;

        foreach ( GameObject g in go )
        {
            int freeSlots = 0;

            GameObject[] demonsArray = g.GetComponent<GroupBehaviour>().demons;
            for ( int i = 0; i < demonsArray.Length; i++ )
            {
                if ( demonsArray[ i ] == null )
                {
                    freeSlots++;
                }
            }

            if ( freeSlots > 0 && freeSlots > maxFreeSlots )
                bestGroup = g;
        }

        if ( bestGroup != null )
        {
            GameObject[] demonsArray = bestGroup.GetComponent<GroupBehaviour>().demons;

            /// Maybe the best solution is:
            //int firstEmpty = System.Array.IndexOf( demonsArray, null );
            //demonsArray[ firstEmpty ] = gameObject;
            //groupFound = true;
            //groupBelongingTo = bestGroup;

            for ( int i = 0; i < demonsArray.Length; i++ )
            {
                if ( demonsArray[ i ] == null )
                {
                    demonsArray[ i ] = gameObject;
                    groupFound = true;
                    groupBelongingTo = bestGroup;
                    break;
                }
            }
        }
    }
}
