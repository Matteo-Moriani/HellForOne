using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private GameObject panelAzure, panelPink, PanelGreen, panelYellow;
    private Image azureImage, pinkImage, greenImage, yellowImage;
    private Sprite meleeSprite, rangeSprite, tankSprite, supportSprite;
    private GroupBehaviour groupAzure, groupPink, groupGreen, groupYellow;
    private TacticsManager tacticsManager;
    private Vector3 defaultScale = new Vector3( 1f, 1f, 1f );
    private Vector3 enlargedScale = new Vector3( 1.5f, 1.5f, 1f );
    private GroupBehaviour[] groupBehaviours = new GroupBehaviour[ 4 ];
    private Dictionary<GroupBehaviour, Image> dict = new Dictionary<GroupBehaviour, Image>();
    private List<Image> healthPoolList = new List<Image>();
    private Image[] healthPoolArray = new Image[ 17 ];
    private int impsCount = 1;
    private int healthIconsCount = 17;
    private AlliesManager alliesManager;
    GameObject healthPool;

    /// <summary>
    /// Used to update the health pool if imps die or join the horde
    /// </summary>
    public void ResizeHealthPool()
    {
        // An Imp died
        if (alliesManager.AlliesList.Count < impsCount )
        {
            for ( int i = alliesManager.AlliesList.Count; i < healthPool.transform.childCount; i++ )
            {
                healthPool.transform.GetChild( i ).gameObject.SetActive( false );
            }
        }

        // An Imp joined
        if ( alliesManager.AlliesList.Count > impsCount )
        {
            for ( int i = healthPool.transform.childCount; i < alliesManager.AlliesList.Count; i++ )
            {
                healthPool.transform.GetChild( i ).gameObject.SetActive( true );
            }
        }
    }

    public void CheckImpsHealth() { }

    void Start()
    {
        GameObject panel = transform.GetChild( 0 ).gameObject;
        panelAzure = panel.transform.GetChild( 0 ).gameObject;
        panelPink = panel.transform.GetChild( 1 ).gameObject;
        PanelGreen = panel.transform.GetChild( 2 ).gameObject;
        panelYellow = panel.transform.GetChild( 3 ).gameObject;

        azureImage = panelAzure.GetComponentInChildren<Image>();
        pinkImage = panelPink.GetComponentInChildren<Image>();
        greenImage = PanelGreen.GetComponentInChildren<Image>();
        yellowImage = panelYellow.GetComponentInChildren<Image>();

        meleeSprite = Resources.Load<Sprite>( "Sprites/sword0" );
        rangeSprite = Resources.Load<Sprite>( "Sprites/bow" );
        tankSprite = Resources.Load<Sprite>( "Sprites/shield" );
        supportSprite = Resources.Load<Sprite>( "Sprites/support" );

        tacticsManager = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<TacticsManager>();

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

        //foreach (GameObject go in alliesManager.AlliesList )
        //{

        //}

        healthPool = transform.GetChild( 1 ).gameObject;

        // Player's Health
        healthPoolArray[ 0 ] = healthPool.transform.GetChild( 0 ).gameObject.GetComponent<Image>();

        for (int i = 1; i < alliesManager.AlliesList.Count; i++ )
        {
            healthPoolArray[i] = healthPool.transform.GetChild( i ).gameObject.GetComponent<Image>();
            impsCount++;
        }
        
        for (int i = alliesManager.AlliesList.Count; i < healthPool.transform.childCount; i++ )
        {
            healthPool.transform.GetChild( i ).gameObject.SetActive( false );
            healthIconsCount--;
        }
    }

    void Update()
    {
        ResizeHealthPool();

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
            case TacticsManager.Group.GroupAzure:
                panelAzure.transform.localScale = enlargedScale;
                panelPink.transform.localScale = defaultScale;
                PanelGreen.transform.localScale = defaultScale;
                panelYellow.transform.localScale = defaultScale;
                break;
            case TacticsManager.Group.GroupPink:
                panelAzure.transform.localScale = defaultScale;
                panelPink.transform.localScale = enlargedScale;
                PanelGreen.transform.localScale = defaultScale;
                panelYellow.transform.localScale = defaultScale;
                break;
            case TacticsManager.Group.GroupGreen:
                panelAzure.transform.localScale = defaultScale;
                panelPink.transform.localScale = defaultScale;
                PanelGreen.transform.localScale = enlargedScale;
                panelYellow.transform.localScale = defaultScale;
                break;
            case TacticsManager.Group.GroupYellow:
                panelAzure.transform.localScale = defaultScale;
                panelPink.transform.localScale = defaultScale;
                PanelGreen.transform.localScale = defaultScale;
                panelYellow.transform.localScale = enlargedScale;
                break;
        }

        
    }

}
