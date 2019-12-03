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
