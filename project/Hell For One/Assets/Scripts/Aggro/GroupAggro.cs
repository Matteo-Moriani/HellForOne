using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupAggro : MonoBehaviour
{
    public GameObject[] groups;
    private GroupBehaviour groupBehaviour;
    private bool shouldStayFixed = false;

    private float groupAggroValue = 1;
    private float tankMultiplier = 1.5f;
    private float orderGivenMultiplier = 1.04f;

    public float OrderGivenMultiplier { get => orderGivenMultiplier; set => orderGivenMultiplier = value; }
    public float TankMultiplier { get => tankMultiplier; set => tankMultiplier = value; }
    public float GroupAggroValue { get => groupAggroValue; set => groupAggroValue = value; }

    private void Start()
    {
        groupBehaviour = GetComponent<GroupBehaviour>();

        groups = GameObject.FindGameObjectsWithTag( "Group" );
    }

    private void Update()
    {
        //if ( groupBehaviour.MeleeOrderGiven() )
        //{
        //    //if ( shouldStayFixed )
        //    //{
        //    //    ManageLockingAggroInDemons( false );
        //        UpdateGroupAggro();
        //    GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>().RaiseAggro(OrderGivenMultiplier);
        //        //shouldStayFixed = false;
        //    //}
        //}
        //if ( groupBehaviour.RangeAttackOrderGiven() )
        //{
        //    //if ( shouldStayFixed )
        //    //{
        //    //    ManageLockingAggroInDemons( false );
        //        UpdateGroupAggro();
        //    GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>().RaiseAggro(OrderGivenMultiplier);
        //    //shouldStayFixed = false;
        //    //}
        //}
        //if ( groupBehaviour.TankOrderGiven() )
        //{
        //    //ManageLockingAggroInDemons( true );
        //    //shouldStayFixed = true;
        //    GroupAggroValue = Mathf.Max( Mathf.CeilToInt( (CalculateAverageAggro() / groups.Length) * TankMultiplier ), GroupAggroValue );
        //    GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>().RaiseAggro(OrderGivenMultiplier);

        //}
        //if ( groupBehaviour.SupportOrderGiven() )
        //{
        //    //ManageLockingAggroInDemons( true );
        //    UpdateGroupAggro();
        //    GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>().RaiseAggro(OrderGivenMultiplier);
        //    //shouldStayFixed = true;
        //    //groupAggro = Mathf.Max( Mathf.CeilToInt((CalculateAverageAggro() / groups.Length) * supportMultiplier), groupAggro );
        //}
    }

    public float GetAggro()
    {
        return GroupAggroValue;
    }

    public void UpdateGroupAggro()
    {
        GroupAggroValue = 0;

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

    //public void LowerGroupAggro(int n) {
    //    if (!shouldStayFixed) {
    //        if (groupAggro > 0)
    //        {
    //            if (groupAggro - n < 0)
    //            {
    //                groupAggro = 0;
    //            }
    //            else
    //            {
    //                groupAggro -= n;
    //            }
    //        }
    //    }
    //}

    //private void ManageLockingAggroInDemons( bool lockAggro )
    //{
    //    if ( groupBehaviour == null )
    //        groupBehaviour = GetComponent<GroupBehaviour>();

    //    if ( groupBehaviour != null )
    //    {
    //        foreach ( GameObject demon in groupBehaviour.demons )
    //        {
    //            Stats stats = demon.GetComponent<Stats>();

    //            if ( stats != null )
    //            {
    //                if ( lockAggro )
    //                    stats.LockAggro();
    //                if ( !lockAggro )
    //                    stats.UnlockAggro();
    //            }
    //            else
    //            {
    //                Debug.Log( this.transform.root.gameObject.name + " GruopAggro.ResetGroupAggro cannot find stats in " + demon.name );
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log( this.transform.root.gameObject.name + " GruopAggro.ResetGroupAggro cannot find GroupBehaviour" );
    //    }
    //}

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

    //// aggiorna l'aggro dei singoli demoni in support
    //public void UpdateSupportAggro() {

    //}
}
