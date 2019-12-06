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

    private Stats stats;

    private void Start()
    {
        stats = GetComponent<Stats>();
    }

    private void Update()
    {
        if ( !groupFound )
            FindGroup();
    }

    // Balances group entering too
    public void FindGroup()
    {
        GameObject[] groups = GameObject.FindGameObjectsWithTag( "Group" );
        GameObject bestGroup = null;
        int maxFreeSlots = 0;

        foreach ( GameObject group in groups )
        {
            int freeSlots = 0;

            GameObject[] demonsArray = group.GetComponent<GroupBehaviour>().demons;
            GroupBehaviour groupBehaviour = group.GetComponent<GroupBehaviour>();
            freeSlots = groupBehaviour.maxNumDemons - groupBehaviour.GetDemonsNumber();

            if ( freeSlots > maxFreeSlots )
            {
                maxFreeSlots = freeSlots;
                bestGroup = group;
            }
        }

        if ( bestGroup )
        {
            GameObject[] demonsArray = bestGroup.GetComponent<GroupBehaviour>().demons;

            int firstEmpty = -1;

            for ( int i = 0; i < demonsArray.Length; i++ )
            {
                if ( !demonsArray[ i ] )
                {
                    firstEmpty = i;
                    break;
                }
            }

            if ( firstEmpty >= 0 )
            {
                demonsArray[ firstEmpty ] = gameObject;
                groupFound = true;
                groupBelongingTo = bestGroup;
                GroupBehaviour groupBehaviour = bestGroup.GetComponent<GroupBehaviour>();
                groupBehaviour.SetDemonsNumber( groupBehaviour.GetDemonsNumber() + 1 );
            }
        }

        gameObject.GetComponent<ColorManager>().ActivateCircle();
    }

}
