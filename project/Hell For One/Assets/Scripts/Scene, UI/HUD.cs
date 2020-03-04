using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private GameObject panelAzure, panelPink, panelGreen, panelYellow, ordersCross;
    private Image azureImage, pinkImage, greenImage, yellowImage, aggroIconAzure, aggroIconPink, aggroIconGreen, aggroIconYellow;
    private Sprite meleeSprite, rangeSprite, tankSprite, supportSprite;
    private GroupBehaviour groupAzure, groupPink, groupGreen, groupYellow;
    private TacticsManager tacticsManager;
    private Vector3 defaultScale = new Vector3( 1f, 1f, 1f );
    private Vector3 enlargedScale = new Vector3( 1.5f, 1.5f, 1f );
    private GroupBehaviour[] groupBehaviours = new GroupBehaviour[ 4 ];
    private Dictionary<GroupBehaviour, Image> dict = new Dictionary<GroupBehaviour, Image>();
    private Image[] healthPoolArray = new Image[ 17 ];
    private int impsCount = 1;
    private int healthIconsCount = 17;
    private AlliesManager alliesManager;
    GameObject healthPool;
    private Vector3 groupPanelCorrectionVector = new Vector3( +36.25f, -22.50f, 0f );
    private Vector2 panelPosition = new Vector2( -36.25f, 22.33f );
    private Vector2 xCorrection = new Vector2( 145f, 0f );

    private GameObject player;
    private CombatEventsManager playerCombatEventsManager;

    public GameObject OrdersCross { get => ordersCross; set => ordersCross = value; }

    /// <summary>
    /// Used to update the health pool if imps die or join the horde
    /// </summary>
    public void ResizeHealthPool()
    {
        // An Imp died
        if ( alliesManager.AlliesList.Count < impsCount - 1 )
        {
            impsCount = alliesManager.AlliesList.Count + 1;

            for ( int i = alliesManager.AlliesList.Count; i < healthPool.transform.childCount; i++ )
            {
                healthPool.transform.GetChild( i + 1 ).gameObject.SetActive( false );
            }

            for ( int i = 0; i < alliesManager.AlliesList.Count; i++ )
            {
                healthPool.transform.GetChild( i ).gameObject.SetActive( true );
            }
        }

        // An Imp joined
        //if ( alliesManager.AlliesList.Count >= impsCount - 1 )
        if ( alliesManager.AlliesList.Count >= impsCount )
        {
            impsCount = alliesManager.AlliesList.Count + 1;

            for ( int i = 0; i < alliesManager.AlliesList.Count + 1; i++ )
            {
                healthPool.transform.GetChild( i ).gameObject.SetActive( true );
            }
        }
    }

    public void ActivateAggroIcon( GroupBehaviour.Group group )
    {
        switch ( group )
        {
            case GroupBehaviour.Group.GroupAzure:
                aggroIconAzure.enabled = true;
                aggroIconGreen.enabled = false;
                aggroIconPink.enabled = false;
                aggroIconYellow.enabled = false;
                break;
            case GroupBehaviour.Group.GroupGreen:
                aggroIconAzure.enabled = false;
                aggroIconGreen.enabled = true;
                aggroIconPink.enabled = false;
                aggroIconYellow.enabled = false;
                break;
            case GroupBehaviour.Group.GroupPink:
                aggroIconAzure.enabled = false;
                aggroIconGreen.enabled = false;
                aggroIconPink.enabled = true;
                aggroIconYellow.enabled = false;
                break;
            case GroupBehaviour.Group.GroupYellow:
                aggroIconAzure.enabled = false;
                aggroIconGreen.enabled = false;
                aggroIconPink.enabled = false;
                aggroIconYellow.enabled = true;
                break;
        }
    }

    public void DeactivateAggroIcon()
    {
        aggroIconAzure.enabled = false;
        aggroIconGreen.enabled = false;
        aggroIconPink.enabled = false;
        aggroIconYellow.enabled = false;
    }

    void Start()
    {
        GameObject panel = transform.GetChild( 0 ).gameObject;
        panelAzure = panel.transform.GetChild( 0 ).gameObject;
        panelPink = panel.transform.GetChild( 1 ).gameObject;
        panelGreen = panel.transform.GetChild( 2 ).gameObject;
        panelYellow = panel.transform.GetChild( 3 ).gameObject;
        OrdersCross = transform.GetChild( 1 ).gameObject;

        azureImage = panelAzure.transform.GetChild( 0 ).gameObject.GetComponent<Image>();
        aggroIconAzure = panelAzure.transform.GetChild( 1 ).gameObject.GetComponent<Image>();
        pinkImage = panelPink.transform.GetChild( 0 ).gameObject.GetComponent<Image>();
        aggroIconPink = panelPink.transform.GetChild( 1 ).gameObject.GetComponent<Image>();
        greenImage = panelGreen.transform.GetChild( 0 ).gameObject.GetComponent<Image>();
        aggroIconGreen = panelGreen.transform.GetChild( 1 ).gameObject.GetComponent<Image>();
        yellowImage = panelYellow.transform.GetChild( 0 ).gameObject.GetComponent<Image>();
        aggroIconYellow = panelYellow.transform.GetChild( 1 ).gameObject.GetComponent<Image>();

        aggroIconAzure.enabled = false;
        aggroIconPink.enabled = false;
        aggroIconGreen.enabled = false;
        aggroIconYellow.enabled = false;

        meleeSprite = Resources.Load<Sprite>( "Sprites/melee_black" );
        rangeSprite = Resources.Load<Sprite>( "Sprites/ranged_black" );
        tankSprite = Resources.Load<Sprite>( "Sprites/tank_black" );
        supportSprite = Resources.Load<Sprite>( "Sprites/dance_black" );

        player = GameObject.FindGameObjectWithTag( "Player" );

        if ( player != null )
        {
            tacticsManager = player.GetComponent<TacticsManager>();
            playerCombatEventsManager = player.GetComponent<CombatEventsManager>();

            if ( playerCombatEventsManager != null )
            {
                playerCombatEventsManager.onDeath += OnPlayerDeath;
            }
            else
            {
                Debug.LogError( "HUD cannot find player's CombatEventsManager" );
            }
        }
        else
        {
            Debug.LogError( "HUD cannot find player" );
        }

        GameObject[] groups = GameObject.FindGameObjectsWithTag( "Group" );

        foreach ( GameObject go in groups )
        {
            switch ( go.name )
            {
                case "GroupAzure":
                    groupAzure = go.GetComponent<GroupBehaviour>();
                    break;
                case "GroupPink":
                    groupPink = go.GetComponent<GroupBehaviour>();
                    break;
                case "GroupGreen":
                    groupGreen = go.GetComponent<GroupBehaviour>();
                    break;
                case "GroupYellow":
                    groupYellow = go.GetComponent<GroupBehaviour>();
                    break;
            }
        }

        groupBehaviours[ 0 ] = groupAzure;
        groupBehaviours[ 1 ] = groupPink;
        groupBehaviours[ 2 ] = groupGreen;
        groupBehaviours[ 3 ] = groupYellow;

        dict.Add( groupAzure, azureImage );
        dict.Add( groupPink, pinkImage );
        dict.Add( groupGreen, greenImage );
        dict.Add( groupYellow, yellowImage );

        alliesManager = GameObject.FindGameObjectWithTag( "Managers" ).GetComponentInChildren<AlliesManager>();
    }

    private void OnDisable()
    {
        if ( playerCombatEventsManager != null )
        {
            playerCombatEventsManager.onDeath -= OnPlayerDeath;
        }
    }

    void Update()
    {
        foreach ( GroupBehaviour gb in groupBehaviours )
        {
            switch ( gb.currentState )
            {
                case GroupBehaviour.State.MeleeAttack:
                    dict[ gb ].overrideSprite = meleeSprite;
                    break;
                case GroupBehaviour.State.RangeAttack:
                    dict[ gb ].overrideSprite = rangeSprite;
                    break;
                case GroupBehaviour.State.Tank:
                    dict[ gb ].overrideSprite = tankSprite;
                    break;
                case GroupBehaviour.State.Support:
                    dict[ gb ].overrideSprite = supportSprite;
                    break;
            }
        }

        switch ( tacticsManager.CurrentShowedGroup )
        {
            case GroupBehaviour.Group.GroupAzure:
                panelAzure.transform.localScale = enlargedScale;
                panelAzure.transform.SetAsLastSibling();
                panelAzure.transform.localPosition = panelPosition;
                panelPink.transform.localScale = defaultScale;
                panelPink.transform.localPosition = xCorrection;
                panelGreen.transform.localScale = defaultScale;
                panelYellow.transform.localScale = defaultScale;
                panelYellow.transform.localPosition = xCorrection * 3;
                break;
            case GroupBehaviour.Group.GroupPink:
                panelAzure.transform.localScale = defaultScale;
                panelAzure.transform.localPosition = Vector3.zero;
                panelPink.transform.localScale = enlargedScale;
                panelPink.transform.SetAsLastSibling();
                panelPink.transform.localPosition = panelPosition + xCorrection;
                panelGreen.transform.localScale = defaultScale;
                panelGreen.transform.localPosition = xCorrection * 2;
                panelYellow.transform.localScale = defaultScale;
                break;
            case GroupBehaviour.Group.GroupGreen:
                panelAzure.transform.localScale = defaultScale;
                panelPink.transform.localScale = defaultScale;
                panelPink.transform.localPosition = xCorrection;
                panelGreen.transform.localScale = enlargedScale;
                panelGreen.transform.SetAsLastSibling();
                panelGreen.transform.localPosition = panelPosition + xCorrection * 2;
                panelYellow.transform.localScale = defaultScale;
                panelYellow.transform.localPosition = xCorrection * 3;
                break;
            case GroupBehaviour.Group.GroupYellow:
                panelAzure.transform.localScale = defaultScale;
                panelAzure.transform.localPosition = Vector3.zero;
                panelPink.transform.localScale = defaultScale;
                panelGreen.transform.localScale = defaultScale;
                panelGreen.transform.localPosition = xCorrection * 2;
                panelYellow.transform.localScale = enlargedScale;
                panelYellow.transform.SetAsLastSibling();
                panelYellow.transform.localPosition = panelPosition + xCorrection * 3;

                break;
        }
    }

    private void OnPlayerDeath()
    {
        if ( playerCombatEventsManager != null )
        {
            playerCombatEventsManager.onDeath -= OnPlayerDeath;
        }

        player = GameObject.FindGameObjectWithTag( "Player" );

        if ( player != null )
        {
            tacticsManager = player.GetComponent<TacticsManager>();
            playerCombatEventsManager = player.GetComponent<CombatEventsManager>();

            if ( playerCombatEventsManager != null )
            {
                playerCombatEventsManager.onDeath += OnPlayerDeath;
            }
            else
            {
                Debug.LogError( "HUD cannot find player's CombatEventsManager" );
            }
        }
        else
        {
            Debug.LogError( "HUD cannot find player" );
        }
    }
}
