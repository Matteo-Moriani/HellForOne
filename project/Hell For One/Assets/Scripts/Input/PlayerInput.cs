using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : GeneralInput
{

    private Dash dash;
    private Combat combat;
    private TacticsManager tacticsManager;
    //private bool gameInPause = false;
    //public bool GameInPause { get => gameInPause; set => gameInPause = value; }
    private bool playing = true;
    public bool Playing { get => playing; set => playing = value; }
    private bool navigatingMenu = false;
    public bool NavigatingMenu { get => navigatingMenu; set => navigatingMenu = value; }
    public bool InCutscene { get => inCutscene; set => inCutscene = value; }
    public bool HasHat { get => hasHat; set => hasHat = value; }
    public bool Attacking { get => attacking; set => attacking = value; }

    private bool attacking = false;
    private bool hasHat = true;
    private GameObject pauseScreen;
    private CombatEventsManager combatEventsManager;
    private bool inCutscene = false;

    private float dpadUpOld, dpadDownOld, dpadLeftOld, dpadRightOld = 0f;
    private float allGroupsOrderStartTimeLeft, allGroupsOrderStartTimeRight, allGroupsOrderStartTimeUp, allGroupsOrderStartTimeDown = 0f;
    public float heldTime = 1f;
    private NewHUD newHUD;


    private IEnumerator DpadWait( float waitTime )
    {
        yield return new WaitForSeconds( waitTime );
        DpadInUse = false;
    }

    private void Awake()
    {
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        FindPauseScreen();
    }

    private void FindPauseScreen()
    {

        GameObject canvas = GameObject.FindGameObjectWithTag( "Canvas" );
        int childrenNum = canvas.transform.childCount;

        for ( int i = 0; i < childrenNum; i++ )
        {
            if ( canvas.transform.GetChild( i ).name == "PauseScreen" )
            {
                pauseScreen = canvas.transform.GetChild( i ).gameObject;
                break;
            }
        }
    }

    private void OnEnable()
    {
        //    Managers.Instance.onPressPlayButton += GameStart;
        if ( combatEventsManager != null )
        {
            combatEventsManager.onDeath += OnDeath;
        }
        BattleEventsManager.onBattlePreparation += OnBattlePreparation;
        BattleEventsManager.onBossBattleEnter += OnBossBattleEnter;
        combatEventsManager.onReincarnation += DisableOrders;
        combatEventsManager.onStartSingleAttack += DisableLeftStick;
        combatEventsManager.onStartRangedAttack += DisableLeftStick;
        combatEventsManager.onStopSingleAttack += EnableLeftStick;
        combatEventsManager.onStopRangedAttack += EnableLeftStick;
    }

    private void OnDisable()
    {
        //    Managers.Instance.onPressPlayButton -= GameStart;
        if ( combatEventsManager != null )
        {
            combatEventsManager.onDeath -= OnDeath;
        }
        BattleEventsManager.onBattlePreparation -= OnBattlePreparation;
        BattleEventsManager.onBossBattleEnter -= OnBossBattleEnter;
        combatEventsManager.onReincarnation -= DisableOrders;
        combatEventsManager.onStartSingleAttack -= DisableLeftStick;
        combatEventsManager.onStartRangedAttack -= DisableLeftStick;
        combatEventsManager.onStopSingleAttack -= EnableLeftStick;
        combatEventsManager.onStopRangedAttack -= EnableLeftStick;
    }

    public void Start()
    {
        controller = this.gameObject.GetComponent<Controller>();
        canGiveInput = true;
        NavigatingMenu = false;
        dash = GetComponent<Dash>();
        combat = GetComponent<Combat>();
        tacticsManager = GetComponent<TacticsManager>();
        CurrentScreen = pauseScreen.GetComponent<Menu>();
        newHUD = GameObject.Find( "HUD" ).GetComponent<NewHUD>();
    }

    private void Update()
    {

        if ( InputManager.Instance != null && !InCutscene )
        {

            if ( dpadPressedInMenu && (NavigatingMenu) )
            {
                if ( fpsCounterInMenu >= 8 )
                {
                    fpsCounterInMenu = 0;
                    dpadPressedInMenu = false;
                }
                else
                    fpsCounterInMenu++;
            }

            // Left stick (PS3 & XBOX)
            if ( controller != null && !Attacking)
            {
                controller.PassXZValues( InputManager.Instance.LeftStickHorizontal(), InputManager.Instance.LeftStickVertical() );
            }

            // Circle (PS3) / B (XBOX) 
            if ( InputManager.Instance.CircleButtonDown() )
            {
                if ( NavigatingMenu )
                    CurrentScreen.Back();
                else
                {
                    if ( dash != null )
                    {
                        dash.TryDash( InputManager.Instance.LeftStickVertical(), InputManager.Instance.LeftStickHorizontal() );
                    }
                }

            }

            // Cross (PS3) / A (XBOX)
            if ( InputManager.Instance.XButtonDown() )
            {
                if ( NavigatingMenu )
                    CurrentScreen.PressSelectedButton();
                else if ( combat != null && tacticsManager )
                {
                    // TODO - dialogues
                }

            }

            // Square (PS3) / X (XBOX)
            if ( InputManager.Instance.SquareButtonDown() && !NavigatingMenu )
            {
                if ( combat != null )
                {
                    combat.PlayerAttack();
                }
            }

            // Triangle (PS3) / Y (XBOX)
            if(InputManager.Instance.TriangleButtonDown() && !NavigatingMenu) {
                if(combat != null) {
                    combat.RangedAttack(null);
                }
            }

            // L1 (PS3) / LB (XBOX) - Down
            if ( InputManager.Instance.L1ButtonDown() && !NavigatingMenu )
            {
                if ( combat != null )
                {
                    combat.StartBlock();
                }
            }

            // L1 (PS3) / LB (XOBX) - Up
            if ( InputManager.Instance.L1ButtonUp() && !NavigatingMenu )
            {
                if ( combat != null )
                {
                    combat.StopBlock();
                }
            }

            // R1 (PS3) / RB (XBOX) - Down
            if ( InputManager.Instance.R1ButtonDown() && !NavigatingMenu )
            {
                if ( combat != null )
                {
                    combat.StartBlock();
                }
            }

            // R1 (PS3) / RB (XOBX) - Up
            if ( InputManager.Instance.R1ButtonUp() && !NavigatingMenu )
            {
                if ( combat != null )
                {
                    combat.StopBlock();
                }
            }

            // L2 (PS3) / LT (XBOX) - Down
            //if ( inputManager.L2Axis() )
            if ( InputManager.Instance.L2ButtonDown() && !NavigatingMenu)
            {
                if ( combat != null && tacticsManager )
                {
                    tacticsManager.RotateLeftGroups();
                }
            }


            // R2 (PS3) / RT (XBOX) - Down
            //if ( inputManager.R2Axis() )
            if(InputManager.Instance.R2ButtonDown() && !NavigatingMenu) {
                if(combat != null && tacticsManager) {
                    tacticsManager.RotateRightGroups();
                }
            }

            // DPad
            if( HasHat) {

                // DPad UP
                if(InputManager.Instance.DpadVertical() > 0.7f) {
                    if(!DpadInUse) {

                        if(NavigatingMenu) {
                            dpadPressedInMenu = true;
                            if(fpsCounterInMenu == 0)
                                CurrentScreen.PreviousButton();
                        }
                        else if(combat != null && tacticsManager.isActiveAndEnabled) {
                            DpadInUse = true;
                            
                            //tacticsManager.AssignOrderToGroup(GroupBehaviour.State.MeleeAttack, tacticsManager.CurrentShowedGroup);
                            bool hasAssignedOrder = tacticsManager.AssignOrder(GroupBehaviour.State.MeleeAttack);

                            if (hasAssignedOrder) {
                                newHUD.ChangeGroupState(tacticsManager.CurrentShowedGroup, 0);
                            }
                            
                            StartCoroutine(DpadWait(dpadWaitTime));
                        }

                    }
                }

                //DPad HELD UP
                if(InputManager.Instance.DpadVertical() > 0.7f) {
                    dpadUpOld = InputManager.Instance.DpadVertical();

                    if(combat != null && tacticsManager.isActiveAndEnabled && dpadUpOld != 0f) {
                        DpadInUse = true;

                        if(allGroupsOrderStartTimeUp == 0f)
                            allGroupsOrderStartTimeUp = Time.time;

                        if((Time.time - allGroupsOrderStartTimeUp) >= heldTime) {
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupAzure, 0 );
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupPink, 0 );
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupGreen, 0 );
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupYellow, 0 );
                            tacticsManager.AllGroupsOrder(GroupBehaviour.State.MeleeAttack);
                            DpadInUse = false;
                            allGroupsOrderStartTimeUp = 0f;
                            dpadUpOld = 0f;
                        }
                    }
                }
                else if(0f <= InputManager.Instance.DpadVertical() && InputManager.Instance.DpadVertical() < 0.7f) {
                    dpadUpOld = 0f;
                    allGroupsOrderStartTimeUp = 0f;
                }

                // DPad DOWN
                if(InputManager.Instance.DpadVertical() < -0.7f) {
                    if(!DpadInUse) {

                        if(NavigatingMenu) {
                            dpadPressedInMenu = true;
                            if(fpsCounterInMenu == 0)
                                CurrentScreen.NextButton();
                        }
                        else if(combat != null && tacticsManager.isActiveAndEnabled) {
                            DpadInUse = true;
                            //newHUD.ChangeGroupState( tacticsManager.CurrentShowedGroup, 2 );
                            //tacticsManager.AssignOrderToGroup(GroupBehaviour.State.RangeAttack, tacticsManager.CurrentShowedGroup);

                            bool hasAssignedOrder = tacticsManager.AssignOrder(GroupBehaviour.State.RangeAttack);

                            if (hasAssignedOrder)
                            {
                                newHUD.ChangeGroupState(tacticsManager.CurrentShowedGroup, 2);
                            }

                            StartCoroutine(DpadWait(dpadWaitTime));
                        }

                    }
                }

                //DPad HELD DOWN
                if(InputManager.Instance.DpadVertical() < -0.7f) {
                    dpadDownOld = InputManager.Instance.DpadVertical();

                    if(combat != null && tacticsManager.isActiveAndEnabled && dpadDownOld != 0f) {
                        DpadInUse = true;

                        if(allGroupsOrderStartTimeDown == 0f)
                            allGroupsOrderStartTimeDown = Time.time;

                        if((Time.time - allGroupsOrderStartTimeDown) >= heldTime) {
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupAzure, 2 );
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupPink, 2 );
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupGreen, 2 );
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupYellow, 2 );
                            tacticsManager.AllGroupsOrder(GroupBehaviour.State.RangeAttack);
                            DpadInUse = false;
                            allGroupsOrderStartTimeDown = 0f;
                            dpadDownOld = 0f;
                        }
                    }
                }
                else if(-0.7f < InputManager.Instance.DpadVertical() && InputManager.Instance.DpadVertical() <= 0f) {
                    dpadDownOld = 0f;
                    allGroupsOrderStartTimeDown = 0f;
                }

                // DPad RIGHT
                if(InputManager.Instance.DpadHorizontal() > 0.7f && !NavigatingMenu) {
                    if(combat != null && tacticsManager.isActiveAndEnabled && !DpadInUse) {
                        DpadInUse = true;
                        //newHUD.ChangeGroupState( tacticsManager.CurrentShowedGroup, 1 );
                        //tacticsManager.AssignOrderToGroup(GroupBehaviour.State.Tank, tacticsManager.CurrentShowedGroup);

                        bool hasAssignedOrder = tacticsManager.AssignOrder(GroupBehaviour.State.Tank);

                        if (hasAssignedOrder)
                        {
                            newHUD.ChangeGroupState(tacticsManager.CurrentShowedGroup, 1);
                        }

                        StartCoroutine(DpadWait(dpadWaitTime));
                    }
                }

                //DPad HELD RIGHT
                if(InputManager.Instance.DpadHorizontal() > 0.7f) {
                    dpadRightOld = InputManager.Instance.DpadHorizontal();

                    if(combat != null && tacticsManager.isActiveAndEnabled && dpadRightOld != 0f) {
                        DpadInUse = true;

                        if(allGroupsOrderStartTimeRight == 0f)
                            allGroupsOrderStartTimeRight = Time.time;

                        if((Time.time - allGroupsOrderStartTimeRight) >= heldTime) {
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupAzure, 1 );
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupPink, 1 );
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupGreen, 1 );
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupYellow, 1 );
                            tacticsManager.AllGroupsOrder(GroupBehaviour.State.Tank);
                            DpadInUse = false;
                            allGroupsOrderStartTimeRight = 0f;
                            dpadRightOld = 0f;
                        }
                    }
                }
                else if(0f <= InputManager.Instance.DpadHorizontal() && InputManager.Instance.DpadHorizontal() < 0.7f) {
                    dpadRightOld = 0f;
                    allGroupsOrderStartTimeRight = 0f;
                }

                // DPad LEFT
                if(InputManager.Instance.DpadHorizontal() < -0.7f && !NavigatingMenu) {
                    if(combat != null && tacticsManager.isActiveAndEnabled && !DpadInUse) {
                        DpadInUse = true;
                        //newHUD.ChangeGroupState( tacticsManager.CurrentShowedGroup, 3 );
                        //tacticsManager.AssignOrderToGroup(GroupBehaviour.State.Support, tacticsManager.CurrentShowedGroup);

                        bool hasAssignedOrder = tacticsManager.AssignOrder(GroupBehaviour.State.Recruit);

                        if (hasAssignedOrder)
                        {
                            newHUD.ChangeGroupState(tacticsManager.CurrentShowedGroup, 4);
                        }

                        StartCoroutine(DpadWait(dpadWaitTime));
                    }
                }

                //DPad HELD LEFT
                if(InputManager.Instance.DpadHorizontal() < -0.7f) {
                    dpadLeftOld = InputManager.Instance.DpadHorizontal();

                    if(combat != null && tacticsManager.isActiveAndEnabled && dpadLeftOld != 0f) {
                        DpadInUse = true;

                        if(allGroupsOrderStartTimeLeft == 0f)
                            allGroupsOrderStartTimeLeft = Time.time;

                        if((Time.time - allGroupsOrderStartTimeLeft) >= heldTime) {
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupAzure, 4 );
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupPink, 4 );
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupGreen, 4 );
                            newHUD.ChangeGroupState( GroupBehaviour.Group.GroupYellow, 4 );
                            tacticsManager.AllGroupsOrder(GroupBehaviour.State.Recruit);
                            DpadInUse = false;
                            allGroupsOrderStartTimeLeft = 0f;
                            dpadLeftOld = 0f;
                        }
                    }
                }
                else if(-0.7f < InputManager.Instance.DpadHorizontal() && InputManager.Instance.DpadHorizontal() <= 0f) {
                    dpadLeftOld = 0f;
                    allGroupsOrderStartTimeLeft = 0f;
                }

            }
            

            // Need in order to set an internal bool in input manager
            // Im looking for a better solution
            if ( InputManager.Instance.L2ButtonUp() && !NavigatingMenu ) { }


            // Need in order to set an internal bool in input manager
            // Im looking for a better solution
            if ( InputManager.Instance.R2ButtonUp() && !NavigatingMenu ) { }

            // Start (PS3) / Options (PS4)
            if ( InputManager.Instance.StartButtonDown() )
            {
                //if ( combat != null )
                //{
                    //if ( GameInPause && NavigatingMenu )
                    //{
                    //    CurrentScreen.GetComponent<PauseScreen>().Resume();
                    //}
                    //else
                    //{
                        CurrentScreen.gameObject.SetActive( true );
                        //NavigatingMenu = true;
                        CurrentScreen.GetComponent<PauseScreen>().Pause();

                        GameEvents.RaiseOnPause();
                    //}
                //}
            }
        }
        else
            Debug.Log( name + " PlayerInput cannot find InputManager" );
    }


    private void OnDeath()
    {
        this.enabled = false;
    }

    private void OnBattlePreparation()
    {
        canGiveInput = false;
        controller.PassXZValues( 0, 0 );
    }

    private void OnBossBattleEnter()
    {
        canGiveInput = true;
    }

    public void DisableOrders() {
        HasHat = false;
    }

    public void DisableLeftStick() {
        controller.XMovement = 0f;
        controller.ZMovement = 0f;
        Attacking = true;
    }

    public void EnableLeftStick() {
        Attacking = false;
    }
}
