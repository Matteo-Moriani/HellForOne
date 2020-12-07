using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using Player;
using TacticsSystem;
using UnityEngine;
using UnityEngine.UI;

public class NewHUD : MonoBehaviour
{
//     private GameObject panelAzure, panelPink, panelGreen, panelYellow, ordersCross, ordersIcons, specialAttacksIcons;
//     private Image azureImage, pinkImage, greenImage, yellowImage, aggroIconAzure, aggroIconPink, aggroIconGreen, aggroIconYellow;
//     private Sprite meleeSprite, rangeSprite, tankSprite, supportSprite;
//     private GroupBehaviour groupAzure, groupPink, groupGreen, groupYellow;
//     private GroupManager groupAzureManager, groupPinkManager, groupGreenManager, groupYellowManager;
//     private TacticsManager tacticsManager;
//     private Vector3 defaultScale = new Vector3( 1f, 1f, 1f );
//     private Vector3 enlargedScale = new Vector3( 1.5f, 1.5f, 1f );
//     private GroupBehaviour[] groupBehaviours = new GroupBehaviour[ 4 ];
//     private Dictionary<GroupBehaviour, GameObject> dict = new Dictionary<GroupBehaviour, GameObject>();
//     private Dictionary<GroupBehaviour, GroupManager> groupDict = new Dictionary<GroupBehaviour, GroupManager>();
//     private AlliesManager alliesManager;
//     private Vector3 groupPanelCorrectionVector = new Vector3( +36.25f, -22.50f, 0f );
//     private Vector2 panelPosition = new Vector2( -36.25f, 22.33f );
//     private Vector2 xCorrection = new Vector2( 145f, 0f );
//
//     private GameObject player;
//     private CombatEventsManager playerCombatEventsManager;
//     private Stats playerStats;
//
//     private int meleeIndex = 0;
//     private int tankIndex = 1;
//     private int rangeIndex = 2;
//     private int supportIndex = 3;
//     private int recruitIndex = 4;
//
//     public GameObject OrdersCross { get => ordersCross; set => ordersCross = value; }
//     public GroupBehaviour.State GroupAzureCurrentState { get => groupAzureCurrentState; set => groupAzureCurrentState = value; }
//     public GroupBehaviour.State GroupPinkCurrentState { get => groupPinkCurrentState; set => groupPinkCurrentState = value; }
//     public GroupBehaviour.State GroupGreenCurrentState { get => groupGreenCurrentState; set => groupGreenCurrentState = value; }
//     public GroupBehaviour.State GroupYellowCurrentState { get => groupYellowCurrentState; set => groupYellowCurrentState = value; }
//
//     private Image UpOn, DownOn, RightOn, LeftOn;
//
//     // To decouple with currentState of GroupBehaviour
//     private GroupBehaviour.State groupAzureCurrentState;
//     private GroupBehaviour.State groupPinkCurrentState;
//     private GroupBehaviour.State groupGreenCurrentState;
//     private GroupBehaviour.State groupYellowCurrentState;
//
//
//     public void ActivateAggroIcon( GroupManager.Group group)
//     {
//         switch ( group )
//         {
//             case GroupManager.Group.GroupAzure:
//                 aggroIconAzure.enabled = true;
//                 aggroIconGreen.enabled = false;
//                 aggroIconPink.enabled = false;
//                 aggroIconYellow.enabled = false;
//                 break;
//             case GroupManager.Group.GroupGreen:
//                 aggroIconAzure.enabled = false;
//                 aggroIconGreen.enabled = true;
//                 aggroIconPink.enabled = false;
//                 aggroIconYellow.enabled = false;
//                 break;
//             case GroupManager.Group.GroupPink:
//                 aggroIconAzure.enabled = false;
//                 aggroIconGreen.enabled = false;
//                 aggroIconPink.enabled = true;
//                 aggroIconYellow.enabled = false;
//                 break;
//             case GroupManager.Group.GroupYellow:
//                 aggroIconAzure.enabled = false;
//                 aggroIconGreen.enabled = false;
//                 aggroIconPink.enabled = false;
//                 aggroIconYellow.enabled = true;
//                 break;
//         }
//     }
//
//     public void DeactivateAggroIcon()
//     {
//         aggroIconAzure.enabled = false;
//         aggroIconGreen.enabled = false;
//         aggroIconPink.enabled = false;
//         aggroIconYellow.enabled = false;
//     }
//
//     void Start()
//     {
//         //GameObject panel = transform.GetChild( 0 ).gameObject;
//         //panelAzure = panel.transform.GetChild( 0 ).gameObject;
//         //panelPink = panel.transform.GetChild( 1 ).gameObject;
//         //panelGreen = panel.transform.GetChild( 2 ).gameObject;
//         //panelYellow = panel.transform.GetChild( 3 ).gameObject;
//         //OrdersCross = transform.GetChild( 1 ).gameObject;
//
//         //azureImage = panelAzure.transform.GetChild( 0 ).gameObject.GetComponent<Image>();
//         //aggroIconAzure = panelAzure.transform.GetChild( panelAzure.transform.childCount - 1 ).gameObject.GetComponent<Image>();
//         //pinkImage = panelPink.transform.GetChild( 0 ).gameObject.GetComponent<Image>();
//         //aggroIconPink = panelPink.transform.GetChild( panelPink.transform.childCount - 1 ).gameObject.GetComponent<Image>();
//         //greenImage = panelGreen.transform.GetChild( 0 ).gameObject.GetComponent<Image>();
//         //aggroIconGreen = panelGreen.transform.GetChild( panelGreen.transform.childCount - 1 ).gameObject.GetComponent<Image>();
//         //yellowImage = panelYellow.transform.GetChild( 0 ).gameObject.GetComponent<Image>();
//         //aggroIconYellow = panelYellow.transform.GetChild( panelYellow.transform.childCount - 1 ).gameObject.GetComponent<Image>();
//
//         //aggroIconAzure.enabled = false;
//         //aggroIconPink.enabled = false;
//         //aggroIconGreen.enabled = false;
//         //aggroIconYellow.enabled = false;
//
//         //meleeSprite = Resources.Load<Sprite>( "Sprites/melee_black" );
//         //rangeSprite = Resources.Load<Sprite>( "Sprites/ranged_black" );
//         //tankSprite = Resources.Load<Sprite>( "Sprites/tank_black" );
//         //supportSprite = Resources.Load<Sprite>( "Sprites/dance_black" );
//
//         player = GameObject.FindGameObjectWithTag( "Player" );
//
//         if ( player != null )
//         {
//             tacticsManager = player.GetComponent<TacticsManager>();
//             
//             playerStats = player.GetComponent<Stats>();
//
//             playerStats.onDeath += OnDeath;
//         }
//         else
//         {
//             Debug.LogError( "HUD cannot find player" );
//         }
//
//         foreach ( GameObject go in GroupsManager.Instance.Groups )
//         {
//             switch ( go.name )
//             {
//                 case "GroupAzure":
//                     groupAzure = go.GetComponent<GroupBehaviour>();
//                     groupAzureManager = go.GetComponent<GroupManager>();
//                     break;
//                 case "GroupPink":
//                     groupPink = go.GetComponent<GroupBehaviour>();
//                     groupPinkManager = go.GetComponent<GroupManager>();
//                     break;
//                 case "GroupGreen":
//                     groupGreen = go.GetComponent<GroupBehaviour>();
//                     groupGreenManager = go.GetComponent<GroupManager>();
//                     break;
//                 case "GroupYellow":
//                     groupYellow = go.GetComponent<GroupBehaviour>();
//                     groupYellowManager = go.GetComponent<GroupManager>();
//                     break;
//             }
//         }
//
//         groupBehaviours[ 0 ] = groupAzure;
//         groupBehaviours[ 1 ] = groupPink;
//         groupBehaviours[ 2 ] = groupGreen;
//         groupBehaviours[ 3 ] = groupYellow;
//
//         dict.Add( groupAzure, panelAzure );
//         dict.Add( groupPink, panelPink );
//         dict.Add( groupGreen, panelGreen );
//         dict.Add( groupYellow, panelYellow );
//
//         groupDict.Add( groupAzure , groupAzureManager );
//         groupDict.Add( groupPink , groupPinkManager );
//         groupDict.Add( groupGreen , groupGreenManager );
//         groupDict.Add( groupYellow , groupYellowManager );
//
//         alliesManager = GameObject.FindGameObjectWithTag( "Managers" ).GetComponentInChildren<AlliesManager>();
//
//         ordersIcons = GameObject.Find( "IconeOrdini" );
//         specialAttacksIcons = GameObject.Find( "IconeAttacchiSpeciali" );
//
//         UpOn = GameObject.Find( "UpON" ).GetComponent<Image>();
//         DownOn = GameObject.Find( "DownON" ).GetComponent<Image>();
//         RightOn = GameObject.Find( "RightON" ).GetComponent<Image>();
//         LeftOn = GameObject.Find( "LeftON" ).GetComponent<Image>();
//
//         GroupAzureCurrentState = groupAzure.currentState;
//         GroupPinkCurrentState = groupPink.currentState;
//         GroupGreenCurrentState = groupGreen.currentState;
//         GroupYellowCurrentState = groupYellow.currentState;
//     }
//
//     // TODO: in ognuno di questi va aggiunto un metodo per il feedback se si prova ad assegnare un ordine quando non si hanno gruppi in range
//     private void OnYButtonDown()
//     {
//         ChangeSpecificGroupHUDOrder(GroupBehaviour.State.MeleeAttack);
//     }
//
//     private void OnXButtonDown()
//     {
//         ChangeSpecificGroupHUDOrder( GroupBehaviour.State.Recruit );
//     }
//
//     private void OnBButtonDown()
//     {
//         ChangeSpecificGroupHUDOrder( GroupBehaviour.State.Tank );
//     }
//
//     private void OnAButtonDown()
//     {
//         ChangeSpecificGroupHUDOrder( GroupBehaviour.State.RangeAttack );
//     }
//
//     private void OnLTButtonHeldDown()
//     {
//         ordersIcons.GetComponent<Image>().enabled = false;
//         specialAttacksIcons.GetComponent<Image>().enabled = true;
//     }
//
//     private void OnLTButtonUp()
//     {
//         specialAttacksIcons.GetComponent<Image>().enabled = false;
//         ordersIcons.GetComponent<Image>().enabled = true;
//     }
//
//     private void OnLT_YButtonDown()
//     {
//         
//     }
//
//     private void OnLT_XButtonDown()
//     {
//         
//     }
//
//     private void OnLT_BButtonDown()
//     {
//         
//     }
//
//     private void OnLT_AButtonDown()
//     {
//         
//     }
//
//     // Firma useless
//     private void OnGroupAzureOrderChanged( GroupBehaviour sender , GroupBehaviour.State newState )
//     {
//         groupAzureCurrentState = groupAzure.currentState;
//     }
//
//     private void OnGroupPinkOrderChanged( GroupBehaviour sender , GroupBehaviour.State newState )
//     {
//         groupPinkCurrentState = groupPink.currentState;
//     }
//
//     private void OnGroupGreenOrderChanged( GroupBehaviour sender , GroupBehaviour.State newState )
//     {
//         groupGreenCurrentState = groupGreen.currentState;
//     }
//
//     private void OnGroupYellowOrderChanged( GroupBehaviour sender , GroupBehaviour.State newState )
//     {
//         groupYellowCurrentState = groupYellow.currentState;
//     }
//
//     private void OnEnable()
//     {
//         PlayerInput.OnYButtonDown += OnYButtonDown;
//         PlayerInput.OnXButtonDown += OnXButtonDown;
//         PlayerInput.OnBButtonDown += OnBButtonDown;
//         PlayerInput.OnAButtonDown += OnAButtonDown;
//         PlayerInput.OnLTButtonHeldDown += OnLTButtonHeldDown;
//         PlayerInput.OnLTButtonUp += OnLTButtonUp;
//         PlayerInput.OnLT_YButtonDown += OnLT_YButtonDown;
//         PlayerInput.OnLT_XButtonDown += OnLT_XButtonDown;
//         PlayerInput.OnLT_BButtonDown += OnLT_BButtonDown;
//         PlayerInput.OnLT_AButtonDown += OnLT_AButtonDown;
//
//         groupAzure.onOrderChanged += OnGroupAzureOrderChanged;
//         groupPink.onOrderChanged += OnGroupPinkOrderChanged;
//         groupGreen.onOrderChanged += OnGroupGreenOrderChanged;
//         groupYellow.onOrderChanged += OnGroupYellowOrderChanged;
//     }
//
//     private void OnDisable()
//     {
//         playerStats.onDeath -= OnDeath;
//         PlayerInput.OnYButtonDown -= OnYButtonDown;
//         PlayerInput.OnXButtonDown -= OnXButtonDown;
//         PlayerInput.OnBButtonDown -= OnBButtonDown;
//         PlayerInput.OnAButtonDown -= OnAButtonDown;
//         PlayerInput.OnLTButtonHeldDown -= OnLTButtonHeldDown;
//         PlayerInput.OnLTButtonUp -= OnLTButtonUp;
//         PlayerInput.OnLT_YButtonDown -= OnLT_YButtonDown;
//         PlayerInput.OnLT_XButtonDown -= OnLT_XButtonDown;
//         PlayerInput.OnLT_BButtonDown -= OnLT_BButtonDown;
//         PlayerInput.OnLT_AButtonDown -= OnLT_AButtonDown;
//
//         groupAzure.onOrderChanged -= OnGroupAzureOrderChanged;
//         groupPink.onOrderChanged -= OnGroupPinkOrderChanged;
//         groupGreen.onOrderChanged -= OnGroupGreenOrderChanged;
//         groupYellow.onOrderChanged -= OnGroupYellowOrderChanged;
//     }
//
//     private void OnDeath(Stats sender)
//     {
//         OnPlayerDeath();
//     }
//     
//     public void ChangeGroupState(GroupManager.Group group, int index)
//     {
//         GroupBehaviour gb = null;
//
//         switch ( group )
//         {
//             case GroupManager.Group.GroupAzure:
//                 gb = groupAzure;
//                 break;
//             case GroupManager.Group.GroupPink:
//                 gb = groupPink;
//                 break;
//             case GroupManager.Group.GroupGreen:
//                 gb = groupGreen;
//                 break;
//             case GroupManager.Group.GroupYellow:
//                 gb = groupYellow;
//                 break;
//         }
//
//         switch ( index )
//         {
//             // Melee
//             case 0:
//                 dict[ gb ].transform.GetChild( meleeIndex ).GetComponent<Image>().enabled = true;
//                 dict[ gb ].transform.GetChild( tankIndex ).GetComponent<Image>().enabled = false;
//                 dict[ gb ].transform.GetChild( rangeIndex ).GetComponent<Image>().enabled = false;
//                 dict[ gb ].transform.GetChild( supportIndex ).GetComponent<Image>().enabled = false;
//                 break;
//
//             // Tank
//             case 1:
//                 dict[ gb ].transform.GetChild( meleeIndex ).GetComponent<Image>().enabled = false;
//                 dict[ gb ].transform.GetChild( tankIndex ).GetComponent<Image>().enabled = true;
//                 dict[ gb ].transform.GetChild( rangeIndex ).GetComponent<Image>().enabled = false;
//                 dict[ gb ].transform.GetChild( supportIndex ).GetComponent<Image>().enabled = false;
//                 break;
//
//             // Range
//             case 2:
//                 dict[ gb ].transform.GetChild( meleeIndex ).GetComponent<Image>().enabled = false;
//                 dict[ gb ].transform.GetChild( tankIndex ).GetComponent<Image>().enabled = false;
//                 dict[ gb ].transform.GetChild( rangeIndex ).GetComponent<Image>().enabled = true;
//                 dict[ gb ].transform.GetChild( supportIndex ).GetComponent<Image>().enabled = false;
//                 break;
//
//             // Support
//             case 3:
//                 dict[ gb ].transform.GetChild( meleeIndex ).GetComponent<Image>().enabled = false;
//                 dict[ gb ].transform.GetChild( tankIndex ).GetComponent<Image>().enabled = false;
//                 dict[ gb ].transform.GetChild( rangeIndex ).GetComponent<Image>().enabled = false;
//                 dict[ gb ].transform.GetChild( supportIndex ).GetComponent<Image>().enabled = true;
//                 break;
//
//             // Recruit
//             case 4:
//                 dict[ gb ].transform.GetChild( meleeIndex ).GetComponent<Image>().enabled = false;
//                 dict[ gb ].transform.GetChild( tankIndex ).GetComponent<Image>().enabled = false;
//                 dict[ gb ].transform.GetChild( rangeIndex ).GetComponent<Image>().enabled = false;
//                 dict[ gb ].transform.GetChild( supportIndex ).GetComponent<Image>().enabled = true;
//                 break;
//         }
//     }
//
//     public void ChangeGroupHUDOrder(GroupBehaviour.State state, GroupBehaviour groupBehaviour)
//     {
//         switch ( state )
//         {
//             case GroupBehaviour.State.MeleeAttack:
//
//                 UpOn.enabled = true;
//                 DownOn.enabled = false;
//                 RightOn.enabled = false;
//                 LeftOn.enabled = false;
//
//                 UpOn.color = groupDict[groupBehaviour].GroupColor;
//                 break;
//
//             case GroupBehaviour.State.RangeAttack:
//
//                 UpOn.enabled = false;
//                 DownOn.enabled = true;
//                 RightOn.enabled = false;
//                 LeftOn.enabled = false;
//
//                 DownOn.color = groupDict[ groupBehaviour ].GroupColor;
//                 break;
//
//             case GroupBehaviour.State.Tank:
//
//                 UpOn.enabled = false;
//                 DownOn.enabled = false;
//                 RightOn.enabled = true;
//                 LeftOn.enabled = false;
//
//                 RightOn.color = groupDict[ groupBehaviour ].GroupColor;
//                 break;
//
//             case GroupBehaviour.State.Recruit:
//
//                 UpOn.enabled = false;
//                 DownOn.enabled = false;
//                 RightOn.enabled = false;
//                 LeftOn.enabled = true;
//
//                 LeftOn.color = groupDict[ groupBehaviour ].GroupColor;
//                 break;
//         }
//     }
//
//     private void ChangeSpecificGroupHUDOrderColor(GroupBehaviour groupBehaviour, GroupBehaviour.State state )
//     {
//         switch ( state )
//         {
//             case GroupBehaviour.State.MeleeAttack:
//
//                 UpOn.enabled = true;
//                 DownOn.enabled = false;
//                 RightOn.enabled = false;
//                 LeftOn.enabled = false;
//
//                 UpOn.color = groupDict[ groupBehaviour ].GroupColor;
//                 break;
//
//             case GroupBehaviour.State.RangeAttack:
//
//                 UpOn.enabled = false;
//                 DownOn.enabled = true;
//                 RightOn.enabled = false;
//                 LeftOn.enabled = false;
//
//                 DownOn.color = groupDict[ groupBehaviour ].GroupColor;
//                 break;
//
//             case GroupBehaviour.State.Tank:
//
//                 UpOn.enabled = false;
//                 DownOn.enabled = false;
//                 RightOn.enabled = true;
//                 LeftOn.enabled = false;
//
//                 RightOn.color = groupDict[ groupBehaviour ].GroupColor;
//                 break;
//
//             case GroupBehaviour.State.Recruit:
//
//                 UpOn.enabled = false;
//                 DownOn.enabled = false;
//                 RightOn.enabled = false;
//                 LeftOn.enabled = true;
//
//                 LeftOn.color = groupDict[ groupBehaviour ].GroupColor;
//                 break;
//         }
//     } 
//
//     public void ChangeSpecificGroupHUDOrder(GroupBehaviour.State state)
//     {
//         switch ( GroupsInRangeDetector.MostRappresentedGroupInRange )
//         {
//             case GroupManager.Group.None:
//
//                 UpOn.enabled = false;
//                 DownOn.enabled = false;
//                 RightOn.enabled = false;
//                 LeftOn.enabled = false;
//                 break;
//
//             case GroupManager.Group.GroupAzure:
//
//                 groupAzureCurrentState = state;
//
//                 ChangeSpecificGroupHUDOrderColor( groupAzure, state );
//                 break;
//
//             case GroupManager.Group.GroupPink:
//
//                 GroupPinkCurrentState = state;
//
//                 ChangeSpecificGroupHUDOrderColor( groupPink, state );
//                 break;
//
//             case GroupManager.Group.GroupGreen:
//
//                 groupGreenCurrentState = state;
//
//                 ChangeSpecificGroupHUDOrderColor( groupGreen, state );
//                 break;
//
//             case GroupManager.Group.GroupYellow:
//
//                 groupYellowCurrentState = state;
//
//                 ChangeSpecificGroupHUDOrderColor( groupYellow, state );
//                 break;
//         }
//     }
//
//     void Update()
//     {
//         // To show the tactics of the most represented group on the HUD
//         switch ( GroupsInRangeDetector.MostRappresentedGroupInRange )
//         {
//             case GroupManager.Group.None:
//
//                 UpOn.enabled = false;
//                 DownOn.enabled = false;
//                 RightOn.enabled = false;
//                 LeftOn.enabled = false;
//                 break;
//
//             case GroupManager.Group.GroupAzure:
//
//                 ChangeGroupHUDOrder( GroupAzureCurrentState, groupAzure );
//                 break;
//
//             case GroupManager.Group.GroupGreen:
//
//                 ChangeGroupHUDOrder( groupGreenCurrentState, groupGreen );
//                 break;
//
//             case GroupManager.Group.GroupPink:
//
//                 ChangeGroupHUDOrder( GroupPinkCurrentState, groupPink );
//                 break;
//
//             case GroupManager.Group.GroupYellow:
//
//                 ChangeGroupHUDOrder( groupYellowCurrentState, groupYellow );
//                 break;
//         }
//     }
//
//     /*
//     void Update()
//     {
//         //UpdateGroupsIcon();
//
//         //Canvas.ForceUpdateCanvases();
//
//         //switch ( tacticsManager.CurrentMostRappresentedGroup )
//         //{
//         //    case GroupBehaviour.Group.GroupAzure:
//         //        panelAzure.transform.localScale = enlargedScale;
//         //        panelAzure.transform.SetAsLastSibling();
//         //        panelAzure.transform.localPosition = panelPosition;
//         //        panelPink.transform.localScale = defaultScale;
//         //        panelPink.transform.localPosition = xCorrection;
//         //        panelGreen.transform.localScale = defaultScale;
//         //        panelYellow.transform.localScale = defaultScale;
//         //        panelYellow.transform.localPosition = xCorrection * 3;
//         //        break;
//         //    case GroupBehaviour.Group.GroupPink:
//         //        panelAzure.transform.localScale = defaultScale;
//         //        panelAzure.transform.localPosition = Vector3.zero;
//         //        panelPink.transform.localScale = enlargedScale;
//         //        panelPink.transform.SetAsLastSibling();
//         //        panelPink.transform.localPosition = panelPosition + xCorrection;
//         //        panelGreen.transform.localScale = defaultScale;
//         //        panelGreen.transform.localPosition = xCorrection * 2;
//         //        panelYellow.transform.localScale = defaultScale;
//         //        break;
//         //    case GroupBehaviour.Group.GroupGreen:
//         //        panelAzure.transform.localScale = defaultScale;
//         //        panelPink.transform.localScale = defaultScale;
//         //        panelPink.transform.localPosition = xCorrection;
//         //        panelGreen.transform.localScale = enlargedScale;
//         //        panelGreen.transform.SetAsLastSibling();
//         //        panelGreen.transform.localPosition = panelPosition + xCorrection * 2;
//         //        panelYellow.transform.localScale = defaultScale;
//         //        panelYellow.transform.localPosition = xCorrection * 3;
//         //        break;
//         //    case GroupBehaviour.Group.GroupYellow:
//         //        panelAzure.transform.localScale = defaultScale;
//         //        panelAzure.transform.localPosition = Vector3.zero;
//         //        panelPink.transform.localScale = defaultScale;
//         //        panelGreen.transform.localScale = defaultScale;
//         //        panelGreen.transform.localPosition = xCorrection * 2;
//         //        panelYellow.transform.localScale = enlargedScale;
//         //        panelYellow.transform.SetAsLastSibling();
//         //        panelYellow.transform.localPosition = panelPosition + xCorrection * 3;
//
//         //        break;
//         //}
//
//         //Canvas.ForceUpdateCanvases();
//     }
//     */
//
//     private void OnPlayerDeath()
//     {
//         if(playerStats != null)
//             playerStats.onDeath -= OnDeath;
//
//         player = GameObject.FindGameObjectWithTag( "Player" );
//
//         if ( player != null )
//         {
//             tacticsManager = player.GetComponent<TacticsManager>();
//
//             playerStats = player.GetComponent<Stats>();
//
//             if (playerStats != null)
//                 playerStats.onDeath += OnDeath;
//         }
//         else
//         {
//             Debug.LogError( "HUD cannot find player" );
//         }
//     }
}
