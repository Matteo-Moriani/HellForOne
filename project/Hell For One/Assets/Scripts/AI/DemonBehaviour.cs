using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class DemonBehaviour : MonoBehaviour
{
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
        GameObject bestGroup = null;
        int maxFreeSlots = 0;

        foreach ( GameObject group in GroupsManager.Instance.Groups )
        {
            int freeSlots = 0;

            GroupManager groupManager = group.GetComponent<GroupManager>();

            GameObject[] demonsArray = groupManager.Imps;
            
            freeSlots = groupManager.MaxImpNumber - groupManager.ImpsInGroupNumber;

            if ( freeSlots > maxFreeSlots )
            {
                maxFreeSlots = freeSlots;
                bestGroup = group;
            }
        }
        
        if ( bestGroup )
        {
            if (bestGroup.GetComponent<GroupManager>().AddDemonToGroup(this.gameObject)) {
                groupFound = true;
                groupBelongingTo = bestGroup;
            }
        }

        gameObject.GetComponent<ChildrenObjectsManager>().ActivateCircle();
    }

}
