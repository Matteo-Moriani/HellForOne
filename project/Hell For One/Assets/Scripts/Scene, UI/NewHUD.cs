using System.Collections.Generic;
using AI.Imp;
using GroupSystem;
using Player;
using TacticsSystem;
using TacticsSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class NewHUD : MonoBehaviour
{
    private GameObject panelAzure, panelPink, panelGreen, panelYellow, ordersCross, ordersIcons, specialAttacksIcons;
    private Image azureImage, pinkImage, greenImage, yellowImage, aggroIconAzure, aggroIconPink, aggroIconGreen, aggroIconYellow;
    private Sprite meleeSprite, rangeSprite, tankSprite, supportSprite;
    
    private GroupManager groupAzureManager, groupPinkManager, groupGreenManager, groupYellowManager;
    private LeaderTactics _leaderTactics;
    private Vector3 defaultScale = new Vector3( 1f , 1f , 1f );
    private Vector3 enlargedScale = new Vector3( 1.5f , 1.5f , 1f );
    private Dictionary<int , GroupManager> groupDict = new Dictionary<int , GroupManager>();
    private AlliesManager alliesManager;
    private Vector3 groupPanelCorrectionVector = new Vector3( +36.25f , -22.50f , 0f );
    private Vector2 panelPosition = new Vector2( -36.25f , 22.33f );
    private Vector2 xCorrection = new Vector2( 145f , 0f );

    private int meleeIndex = 0;
    private int tankIndex = 1;
    private int rangeIndex = 2;
    private int supportIndex = 3;
    private int recruitIndex = 4;

    public float alfa = 1f;

    public GameObject OrdersCross { get => ordersCross; set => ordersCross = value; }

    private Image UpOn, DownOn, RightOn, LeftOn;

    /*
     * 0 = Melee
     * 1 = Ranged
     * 2 = Recruit
     * 3 = Counter
     */
    private int groupAzureTacticsID = 0;
    private int groupPinkTacticsID = 0;
    private int groupGreenTacticsID = 0;
    private int groupYellowTacticsID = 0;

    public void ActivateAggroIcon( GroupManager.Group group )
    {
        switch ( group )
        {
            case GroupManager.Group.GroupAzure:
                aggroIconAzure.enabled = true;
                aggroIconGreen.enabled = false;
                aggroIconPink.enabled = false;
                aggroIconYellow.enabled = false;
                break;
            case GroupManager.Group.GroupGreen:
                aggroIconAzure.enabled = false;
                aggroIconGreen.enabled = true;
                aggroIconPink.enabled = false;
                aggroIconYellow.enabled = false;
                break;
            case GroupManager.Group.GroupPink:
                aggroIconAzure.enabled = false;
                aggroIconGreen.enabled = false;
                aggroIconPink.enabled = true;
                aggroIconYellow.enabled = false;
                break;
            case GroupManager.Group.GroupYellow:
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
        foreach ( GameObject go in GroupsManager.Instance.Groups.Values )
        {
            switch ( go.name )
            {
                case "GroupAzure":
                    groupAzureManager = go.GetComponent<GroupManager>();
                    break;
                case "GroupPink":
                    groupPinkManager = go.GetComponent<GroupManager>();
                    break;
                case "GroupGreen":
                    groupGreenManager = go.GetComponent<GroupManager>();
                    break;
                case "GroupYellow":
                    groupYellowManager = go.GetComponent<GroupManager>();
                    break;
            }
        }

        alliesManager = GameObject.FindGameObjectWithTag( "Managers" ).GetComponentInChildren<AlliesManager>();

        ordersIcons = GameObject.Find( "IconeOrdini" );
        specialAttacksIcons = GameObject.Find( "IconeAttacchiSpeciali" );

        UpOn = GameObject.Find( "UpON" ).GetComponent<Image>();
        DownOn = GameObject.Find( "DownON" ).GetComponent<Image>();
        RightOn = GameObject.Find( "RightON" ).GetComponent<Image>();
        LeftOn = GameObject.Find( "LeftON" ).GetComponent<Image>();
    }

    // TODO: in ognuno di questi va aggiunto un metodo per il feedback se si prova ad assegnare un ordine quando non si hanno gruppi in range
    private void OnYButtonDown()
    {
        ChangeSpecificGroupHUDOrder( 0 );
    }

    private void OnXButtonDown()
    {
        ChangeSpecificGroupHUDOrder( 2 );
    }

    private void OnBButtonDown()
    {
        ChangeSpecificGroupHUDOrder( 3 );
    }

    private void OnAButtonDown()
    {
        ChangeSpecificGroupHUDOrder( 1 );
    }

    private void OnLTButtonHeldDown()
    {
        ordersIcons.GetComponent<Image>().enabled = false;
        specialAttacksIcons.GetComponent<Image>().enabled = true;
    }

    private void OnLTButtonUp()
    {
        specialAttacksIcons.GetComponent<Image>().enabled = false;
        ordersIcons.GetComponent<Image>().enabled = true;
    }

    private void OnLT_YButtonDown()
    {

    }

    private void OnLT_XButtonDown()
    {

    }

    private void OnLT_BButtonDown()
    {

    }

    private void OnLT_AButtonDown()
    {

    }

    private void OnEnable()
    {
        PlayerInput.OnYButtonDown += OnYButtonDown;
        PlayerInput.OnXButtonDown += OnXButtonDown;
        PlayerInput.OnBButtonDown += OnBButtonDown;
        PlayerInput.OnAButtonDown += OnAButtonDown;
        PlayerInput.OnLTButtonHeldDown += OnLTButtonHeldDown;
        PlayerInput.OnLTButtonUp += OnLTButtonUp;
        PlayerInput.OnLT_YButtonDown += OnLT_YButtonDown;
        PlayerInput.OnLT_XButtonDown += OnLT_XButtonDown;
        PlayerInput.OnLT_BButtonDown += OnLT_BButtonDown;
        PlayerInput.OnLT_AButtonDown += OnLT_AButtonDown;
        
        ImpGroupAi.OnTacticChangedGlobal += OnTacticChangeGlobal;
    }

    private int BondTacticsNameToTacticsID(string tacticsName )
    {
        switch ( tacticsName )
        {
            case "MeleeAttackTactic":
                return 0;
                break;

            case "RangedAttackTactic":
                return 1;
                break;

            case "RecruitTactic":
                return 2;
                break;

            case "CounterTactic":
                return 3;
                break;
        }

        // Error
        return 123456789;
    }

    private void OnTacticChangeGlobal( TacticFactory arg1 , GroupManager.Group arg2 )
    {
        switch ( arg2 )
        {
            case GroupManager.Group.GroupAzure:
                groupAzureTacticsID = BondTacticsNameToTacticsID( arg1.name );
                break;

            case GroupManager.Group.GroupPink:
                groupPinkTacticsID = BondTacticsNameToTacticsID( arg1.name );
                break;

            case GroupManager.Group.GroupGreen:
                groupGreenTacticsID = BondTacticsNameToTacticsID( arg1.name );
                break;

            case GroupManager.Group.GroupYellow:
                groupYellowTacticsID = BondTacticsNameToTacticsID( arg1.name );
                break;
        }
    }

    private void OnDisable()
    {
        PlayerInput.OnYButtonDown -= OnYButtonDown;
        PlayerInput.OnXButtonDown -= OnXButtonDown;
        PlayerInput.OnBButtonDown -= OnBButtonDown;
        PlayerInput.OnAButtonDown -= OnAButtonDown;
        PlayerInput.OnLTButtonHeldDown -= OnLTButtonHeldDown;
        PlayerInput.OnLTButtonUp -= OnLTButtonUp;
        PlayerInput.OnLT_YButtonDown -= OnLT_YButtonDown;
        PlayerInput.OnLT_XButtonDown -= OnLT_XButtonDown;
        PlayerInput.OnLT_BButtonDown -= OnLT_BButtonDown;
        PlayerInput.OnLT_AButtonDown -= OnLT_AButtonDown;

        ImpGroupAi.OnTacticChangedGlobal -= OnTacticChangeGlobal;
    }

    private void ChangeGroupHUDOrder( int groupTacticsID , Color color )
    {
        switch ( groupTacticsID )
        {
            case 0:

                UpOn.enabled = true;
                DownOn.enabled = false;
                RightOn.enabled = false;
                LeftOn.enabled = false;

                var tempColor0 = color;
                tempColor0.a = alfa;
                UpOn.color = tempColor0;
                break;

            case 1:

                UpOn.enabled = false;
                DownOn.enabled = true;
                RightOn.enabled = false;
                LeftOn.enabled = false;

                var tempColor1 = color;
                tempColor1.a = alfa;
                DownOn.color = tempColor1;
                break;

            case 3:

                UpOn.enabled = false;
                DownOn.enabled = false;
                RightOn.enabled = true;
                LeftOn.enabled = false;

                var tempColor2 = color;
                tempColor2.a = alfa;
                RightOn.color = tempColor2;
                break;

            case 2:

                UpOn.enabled = false;
                DownOn.enabled = false;
                RightOn.enabled = false;
                LeftOn.enabled = true;

                var tempColor3 = color;
                tempColor3.a = alfa;
                LeftOn.color = tempColor3;
                break;
        }
    }

    private void ChangeSpecificGroupHUDOrderColor( Color color , int tacticsID )
    {
        switch ( tacticsID )
        {
            case 0:

                UpOn.enabled = true;
                DownOn.enabled = false;
                RightOn.enabled = false;
                LeftOn.enabled = false;

                var tempColor0 = color;
                tempColor0.a = alfa;
                UpOn.color = tempColor0;
                break;

            case 1:

                UpOn.enabled = false;
                DownOn.enabled = true;
                RightOn.enabled = false;
                LeftOn.enabled = false;

                var tempColor1 = color;
                tempColor1.a = alfa;
                DownOn.color = tempColor1;
                break;

            case 3:

                UpOn.enabled = false;
                DownOn.enabled = false;
                RightOn.enabled = true;
                LeftOn.enabled = false;

                var tempColor2 = color;
                tempColor2.a = alfa;
                RightOn.color = tempColor2;
                break;

            case 2:

                UpOn.enabled = false;
                DownOn.enabled = false;
                RightOn.enabled = false;
                LeftOn.enabled = true;

                var tempColor3 = color;
                tempColor3.a = alfa;
                LeftOn.color = tempColor3;
                break;
        }
    }

    private void ChangeSpecificGroupHUDOrder( int tacticsID )
    {
        switch ( GroupsInRangeDetector.MostRepresentedGroupInRange )
        {
            case GroupManager.Group.None:

                UpOn.enabled = false;
                DownOn.enabled = false;
                RightOn.enabled = false;
                LeftOn.enabled = false;
                break;

            case GroupManager.Group.GroupAzure:

                groupAzureTacticsID = tacticsID;

                ChangeSpecificGroupHUDOrderColor( groupAzureManager.GroupColor , tacticsID );
                break;

            case GroupManager.Group.GroupPink:

                groupPinkTacticsID = tacticsID;

                ChangeSpecificGroupHUDOrderColor( groupPinkManager.GroupColor , tacticsID );
                break;

            case GroupManager.Group.GroupGreen:

                groupGreenTacticsID = tacticsID;

                ChangeSpecificGroupHUDOrderColor( groupGreenManager.GroupColor , tacticsID );
                break;

            case GroupManager.Group.GroupYellow:

                groupYellowTacticsID = tacticsID;

                ChangeSpecificGroupHUDOrderColor( groupYellowManager.GroupColor , tacticsID );
                break;
        }
    }

    void Update()
    {
        // To show the tactics of the most represented group on the HUD
        switch ( GroupsInRangeDetector.MostRepresentedGroupInRange )
        {
            case GroupManager.Group.None:

                UpOn.enabled = false;
                DownOn.enabled = false;
                RightOn.enabled = false;
                LeftOn.enabled = false;
                break;

            case GroupManager.Group.GroupAzure:

                ChangeGroupHUDOrder( groupAzureTacticsID , groupAzureManager.GroupColor );
                break;

            case GroupManager.Group.GroupGreen:

                ChangeGroupHUDOrder( groupGreenTacticsID , groupGreenManager.GroupColor );
                break;

            case GroupManager.Group.GroupPink:

                ChangeGroupHUDOrder( groupPinkTacticsID , groupPinkManager.GroupColor );
                break;

            case GroupManager.Group.GroupYellow:

                ChangeGroupHUDOrder( groupYellowTacticsID , groupYellowManager.GroupColor );
                break;
        }
    }
}
