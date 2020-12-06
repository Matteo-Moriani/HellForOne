using AI;
using UnityEngine;

public class GroupAggro : MonoBehaviour
{
    #region Fields

    private GroupManager groupManager;
    private GroupBehaviour groupBehaviour;

    private float groupAggroValue = 1;
    private float tankMultiplier = 1.5f;

    #endregion

    #region Properties

    public float GroupAggroValue { get => groupAggroValue; private set => groupAggroValue = value; }

    #endregion

    #region Unity methods

    private void Awake()
    {
        groupManager = GetComponent<GroupManager>();
        groupBehaviour = GetComponent<GroupBehaviour>();
    }

    private void OnEnable()
    {
        groupManager.onImpJoined += OnImpJoined;
        groupManager.onImpRemoved += OnImpRemoved;

        groupBehaviour.onTankOrderGiven += OnTankOrderGiven;
    }

    private void OnDisable()
    {
        groupManager.onImpJoined -= OnImpJoined;
        groupManager.onImpRemoved -= OnImpRemoved;

        groupBehaviour.onTankOrderGiven -= OnTankOrderGiven;
    }

    #endregion

    #region Methods

    // TODO - Check if we need Total aggro or average aggro
    private float CalculateAverageAggro()
    {
        float totalAggro = 0;

        if ( GroupsManager.Instance.Groups != null )
        {
            foreach ( GameObject group in GroupsManager.Instance.Groups )
            {
                totalAggro += group.GetComponent<GroupAggro>().GroupAggroValue;
            }

            return totalAggro / GroupsManager.Instance.Groups.Length;
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " GroupAggro.CalculateAverageAggro cannot find other groups" );
        }

        return -1;
    }

    #endregion

    #region Event handlers

    // NOTES for Aggro and Orders
    // IF SUPPORT : Every single imp update his aggro every second (GroupBehaviour.OnSupportStayAction event)
    // IF MELEE : Every hit will update imp and group aggro
    // IF RANGED : Every hit will update imp and group aggro
    // IF TANK : Aggro is fixed
    // IF RECRUIT : not implemented yet

    private void OnImpAggroChanged( ImpAggro sender , float oldValue )
    {
        groupAggroValue -= oldValue;
        groupAggroValue += sender.Aggro;
    }

    private void OnImpJoined( GroupManager sender , GameObject impJoined )
    {
        impJoined.GetComponent<ImpAggro>().onImpAggroChanged += OnImpAggroChanged;
        groupAggroValue += impJoined.GetComponent<ImpAggro>().Aggro;
    }

    private void OnImpRemoved( GroupManager sender , GameObject impRemoved )
    {
        impRemoved.GetComponent<ImpAggro>().onImpAggroChanged -= OnImpAggroChanged;
        groupAggroValue -= impRemoved.GetComponent<ImpAggro>().Aggro;
    }

    private void OnTankOrderGiven( GroupBehaviour sender )
    {
        groupAggroValue = Mathf.Max( Mathf.CeilToInt( CalculateAverageAggro() * tankMultiplier ) , groupAggroValue );
    }

    #endregion
}
