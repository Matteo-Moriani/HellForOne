using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupAggro : MonoBehaviour
{
    public GameObject[] groups;
    private GroupBehaviour groupBehaviour;

    private float groupAggroValue = 1;
    private float tankMultiplier = 1.5f;
    private float orderGivenMultiplier = 1.2f;

    public float OrderGivenMultiplier { get => orderGivenMultiplier; set => orderGivenMultiplier = value; }
    public float TankMultiplier { get => tankMultiplier; set => tankMultiplier = value; }
    public float GroupAggroValue { get => groupAggroValue; set => groupAggroValue = value; }

    private void Start()
    {
        groupBehaviour = GetComponent<GroupBehaviour>();

        groups = GameObject.FindGameObjectsWithTag( "Group" );
    }

    public float GetAggro()
    {
        return GroupAggroValue;
    }

    // TODO -   Insert check if the group is supportin etc etc
    //          in order to update group aggro properly
    public void UpdateGroupAggro()
    {
        GroupAggroValue = 1;

        if ( groupBehaviour == null )
            groupBehaviour = GetComponent<GroupBehaviour>();

        if ( groupBehaviour != null )
        {
            foreach ( GameObject demon in groupBehaviour.demons )
            {
                if ( demon != null )
                {
                    Stats stats = demon.GetComponent<Stats>();

                    if ( stats != null )
                    {
                        GroupAggroValue += stats.Aggro;
                    }
                    else
                    {
                        Debug.Log( this.transform.root.gameObject.name + " GroupAggro.ResetGroupAggro cannot find stats in " + demon.name );
                    }
                }
            }
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " GroupAggro.ResetGroupAggro cannot find GroupBehaviour" );
        }
    }

    public void ResetGroupAggro()
    {
        GroupAggroValue = gameObject.GetComponent<GroupBehaviour>().GetDemonsNumber(); ;

        if ( groupBehaviour == null )
            groupBehaviour = GetComponent<GroupBehaviour>();

        if ( groupBehaviour != null )
        {
            foreach ( GameObject demon in groupBehaviour.demons )
            {
                Stats stats = demon.GetComponent<Stats>();

                if ( stats != null )
                {
                    stats.Aggro = 0;
                }
                else
                {
                    Debug.Log( this.transform.root.gameObject.name + " GuopAggro.ResetGroupAggro cannot find stats in " + demon.name );
                }
            }
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " GuopAggro.ResetGroupAggro cannot find GroupBehaviour" );
        }
    }

    public void RaiseGroupAggro( float n )
    {
        //if ( !shouldStayFixed )
        {
            GroupAggroValue += n;
        }
    }

    public float CalculateAverageAggro()
    {
        float totalAggro = 0;

        if ( groups != null )
        {
            foreach ( GameObject group in groups )
            {
                totalAggro += group.GetComponent<GroupAggro>().GetAggro();
            }
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " GroupAggro.CalculateAverageAggro cannot find other groups" );
        }
        return totalAggro;
    }
}
