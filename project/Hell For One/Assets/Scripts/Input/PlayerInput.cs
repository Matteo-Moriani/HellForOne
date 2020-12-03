﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FactoryBasedCombatSystem;

public class PlayerInput : GeneralInput
{
    private Stats stats;
    private Dash dash;
    private Combat combat;
    private TacticsManager tacticsManager;
    private Reincarnation reincarnation;

    private Block block;
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

    #region Delegates and events

    public delegate void OnOLDXButtonDown();
    public static event OnOLDXButtonDown onXButtonDown;

    public delegate void OnXButtonUp();
    public static event OnXButtonUp onXButtonUp;

    public delegate void OnXButtonHeldDown();
    public static event OnXButtonHeldDown onXButtonHeldDown;

    public static event Action OnYButtonDown;
    public static event Action OnXButtonDown;
    public static event Action OnBButtonDown;
    public static event Action OnAButtonDown;
    public static event Action OnLTButtonHeldDown;
    public static event Action OnLTButtonUp;
    public static event Action OnLT_YButtonDown;
    public static event Action OnLT_XButtonDown;
    public static event Action OnLT_BButtonDown;
    public static event Action OnLT_AButtonDown;

    private void RaiseOnXButtonDown()
    {
        onXButtonDown?.Invoke();
    }

    private void RaiseOnXButtonUp()
    {
        onXButtonUp?.Invoke();
    }

    private void RaiseOnXButtonHeldDown()
    {
        onXButtonHeldDown?.Invoke();
    }

    #endregion

    private IEnumerator DpadWait( float waitTime )
    {
        yield return new WaitForSeconds( waitTime );
        DpadInUse = false;
    }

    private void Awake()
    {
        stats = GetComponent<Stats>();
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        reincarnation = this.gameObject.GetComponent<Reincarnation>();
        block = GetComponentInChildren<Block>();
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
        stats.onDeath += OnDeath;

        if ( reincarnation != null )
        {
            reincarnation.onReincarnation += OnReincarnation;
        }
        BattleEventsManager.onBattlePreparation += OnBattlePreparation;
        BattleEventsManager.onBattleEnter += OnBattleEnter;

    }

    private void OnDisable()
    {
        stats.onDeath -= OnDeath;

        if ( reincarnation != null )
        {
            reincarnation.onReincarnation -= OnReincarnation;
        }
        BattleEventsManager.onBattlePreparation -= OnBattlePreparation;
        BattleEventsManager.onBattleEnter -= OnBattleEnter;

    }

    public void Start()
    {
        playerController = this.gameObject.GetComponent<PlayerController>();
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
        if ( Input.GetButtonDown( "XBoxY" ) )
        {
            OnYButtonDown?.Invoke();
            // Registrarsi all'evento sia in NewHUD e TacticsManager
        }

        if ( Input.GetButtonDown( "XBoxX" ) )
        {
            OnXButtonDown?.Invoke();
            // Registrarsi all'evento sia in NewHUD e TacticsManager
        }

        if ( Input.GetButtonDown( "XBoxB" ) )
        {
            OnBButtonDown?.Invoke();
            // Registrarsi all'evento sia in NewHUD e TacticsManager
        }

        if ( Input.GetButtonDown( "XBoxA" ) )
        {
            OnAButtonDown?.Invoke();
            // Registrarsi all'evento sia in NewHUD e TacticsManager
        }

        if ( Input.GetButtonUp( "XBoxLT" ) )
        {
            OnLTButtonUp?.Invoke();
        }

        // TODO non funge per ora
        if ( Input.GetButton( "XBoxLT" ) )
        {
            OnLTButtonHeldDown?.Invoke();

            if ( Input.GetButtonDown( "XBoxY" ) )
            {
                OnLT_YButtonDown?.Invoke();
                // Registrarsi all'evento sia in NewHUD e TacticsManager
            }

            if ( Input.GetButtonDown( "XBoxX" ) )
            {
                OnLT_XButtonDown?.Invoke();
                // Registrarsi all'evento sia in NewHUD e TacticsManager
            }

            if ( Input.GetButtonDown( "XBoxB" ) )
            {
                OnLT_BButtonDown?.Invoke();
                // Registrarsi all'evento sia in NewHUD e TacticsManager
            }

            if ( Input.GetButtonDown( "XBoxA" ) )
            {
                OnLT_AButtonDown?.Invoke();
                // Registrarsi all'evento sia in NewHUD e TacticsManager
            }
        }

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
            if ( playerController != null && !Attacking )
            {
                playerController.PassXZValues( InputManager.Instance.LeftStickHorizontal() , InputManager.Instance.LeftStickVertical() );
            }

            // Circle (PS3) / B (XBOX) 
            if ( InputManager.Instance.CircleButtonDown() )
            {
                if ( NavigatingMenu )
                    CurrentScreen.Back();
                if ( combat != null && tacticsManager.isActiveAndEnabled )
                {
                    bool hasAssignedOrder = tacticsManager.AssignOrder( GroupBehaviour.State.Tank );

                    if ( hasAssignedOrder )
                    {
                        newHUD.ChangeGroupState( tacticsManager.CurrentMostRepresentedGroup , 1 );
                    }
                }
                else if ( combat != null && tacticsManager.isActiveAndEnabled )
                {
                    bool hasAssignedOrder = tacticsManager.AssignOrder( GroupBehaviour.State.MeleeAttack );

                    if ( hasAssignedOrder )
                    {
                        newHUD.ChangeGroupState( tacticsManager.CurrentMostRepresentedGroup , 0 );
                    }

                    StartCoroutine( DpadWait( dpadWaitTime ) );
                }
                else
                {
                    if ( dash != null )
                    {
                        dash.TryDash( InputManager.Instance.LeftStickVertical() , InputManager.Instance.LeftStickHorizontal() );
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
                else if ( combat != null && tacticsManager.isActiveAndEnabled )
                {
                    bool hasAssignedOrder = tacticsManager.AssignOrder( GroupBehaviour.State.RangeAttack );

                    if ( hasAssignedOrder )
                    {
                        newHUD.ChangeGroupState( tacticsManager.CurrentMostRepresentedGroup , 2 );
                    }
                }

            }

            // X (XBOX)
            if ( InputManager.Instance.SquareButtonDown() && !NavigatingMenu )
            {
                //RaiseOnXButtonDown();

                if ( combat != null && tacticsManager.isActiveAndEnabled )
                {
                    bool hasAssignedOrder = tacticsManager.AssignOrder( GroupBehaviour.State.Recruit );

                    if ( hasAssignedOrder )
                    {
                        newHUD.ChangeGroupState( tacticsManager.CurrentMostRepresentedGroup , 4 );
                    }
                }
            }
            if ( InputManager.Instance.SquareButtonUp() && !NavigatingMenu )
            {
                RaiseOnXButtonUp();
            }
            if ( InputManager.Instance.SquareButtonHeldDown() && !NavigatingMenu )
            {
                RaiseOnXButtonHeldDown();
            }

            // Triangle (PS3) / Y (XBOX)
            if ( InputManager.Instance.TriangleButtonDown() && !NavigatingMenu )
            {
                if ( combat != null )
                {
                    //combat.RangedAttack(null);
                }
                else if ( combat != null && tacticsManager.isActiveAndEnabled )
                {
                    bool hasAssignedOrder = tacticsManager.AssignOrder( GroupBehaviour.State.MeleeAttack );

                    if ( hasAssignedOrder )
                    {
                        newHUD.ChangeGroupState( tacticsManager.CurrentMostRepresentedGroup , 0 );
                    }
                }
            }

            // L1 (PS3) / LB (XBOX) - Down
            if ( InputManager.Instance.L1ButtonDown() && !NavigatingMenu )
            {
                if ( combat != null )
                {
                    block.StartBlock();
                }
            }

            // L1 (PS3) / LB (XOBX) - Up
            if ( InputManager.Instance.L1ButtonUp() && !NavigatingMenu )
            {
                if ( combat != null )
                {
                    block.StopBlock();
                }
            }

            // R1 (PS3) / RB (XBOX) - Down
            if ( InputManager.Instance.R1ButtonDown() && !NavigatingMenu )
            {
                if ( combat != null )
                {
                    block.StartBlock();
                }
            }

            // R1 (PS3) / RB (XOBX) - Up
            if ( InputManager.Instance.R1ButtonUp() && !NavigatingMenu )
            {
                if ( combat != null )
                {
                    block.StopBlock();
                }
            }

            //// L2 (PS3) / LT (XBOX) - Down
            ////if ( inputManager.L2Axis() )
            //if ( InputManager.Instance.L2ButtonDown() && !NavigatingMenu)
            //{
            //    if ( combat != null && tacticsManager )
            //    {
            //        tacticsManager.RotateLeftGroups();
            //    }
            //}


            //// R2 (PS3) / RT (XBOX) - Down
            ////if ( inputManager.R2Axis() )
            //if(InputManager.Instance.R2ButtonDown() && !NavigatingMenu) {
            //    if(combat != null && tacticsManager) {
            //        tacticsManager.RotateRightGroups();
            //    }
            //}

            // LT + RT - HELD Down
            if ( (InputManager.Instance.R1ButtonHeldDown() && InputManager.Instance.L1ButtonDown())
                 ||
                 (InputManager.Instance.R1ButtonDown() && InputManager.Instance.L1ButtonHeldDown())
                )
            {
                GetComponent<ImpMana>().StartActiveManaRecharge();
            }

            // LT + RT - Up
            if ( InputManager.Instance.R1ButtonUp() || InputManager.Instance.L1ButtonUp() )
            {
                GetComponent<ImpMana>().StopActiveManaRecharge();
            }

            // DPad
            if ( HasHat )
            {

                // DPad UP
                if ( InputManager.Instance.DpadVertical() > 0.7f )
                {
                    if ( !DpadInUse )
                    {

                        if ( NavigatingMenu )
                        {
                            dpadPressedInMenu = true;
                            if ( fpsCounterInMenu == 0 )
                                CurrentScreen.PreviousButton();
                        }
                        //else if(combat != null && tacticsManager.isActiveAndEnabled) {
                        //    DpadInUse = true;

                        //    //tacticsManager.AssignOrderToGroup(GroupBehaviour.State.MeleeAttack, tacticsManager.CurrentShowedGroup);
                        //    bool hasAssignedOrder = tacticsManager.AssignOrder(GroupBehaviour.State.MeleeAttack);

                        //    if (hasAssignedOrder) {
                        //        newHUD.ChangeGroupState(tacticsManager.CurrentMostRepresentedGroup, 0);
                        //    }

                        //    StartCoroutine(DpadWait(dpadWaitTime));
                        //}

                    }
                }

                //DPad HELD UP
                if ( InputManager.Instance.DpadVertical() > 0.7f )
                {
                    dpadUpOld = InputManager.Instance.DpadVertical();

                    if ( combat != null && tacticsManager.isActiveAndEnabled && dpadUpOld != 0f )
                    {
                        DpadInUse = true;

                        if ( allGroupsOrderStartTimeUp == 0f )
                            allGroupsOrderStartTimeUp = Time.time;

                        if ( (Time.time - allGroupsOrderStartTimeUp) >= heldTime )
                        {
                            newHUD.ChangeGroupState( GroupManager.Group.GroupAzure , 0 );
                            newHUD.ChangeGroupState( GroupManager.Group.GroupPink , 0 );
                            newHUD.ChangeGroupState( GroupManager.Group.GroupGreen , 0 );
                            newHUD.ChangeGroupState( GroupManager.Group.GroupYellow , 0 );
                            tacticsManager.AllGroupsOrder( GroupBehaviour.State.MeleeAttack );
                            DpadInUse = false;
                            allGroupsOrderStartTimeUp = 0f;
                            dpadUpOld = 0f;
                        }
                    }
                }
                else if ( 0f <= InputManager.Instance.DpadVertical() && InputManager.Instance.DpadVertical() < 0.7f )
                {
                    dpadUpOld = 0f;
                    allGroupsOrderStartTimeUp = 0f;
                }

                // DPad DOWN
                if ( InputManager.Instance.DpadVertical() < -0.7f )
                {
                    if ( !DpadInUse )
                    {

                        if ( NavigatingMenu )
                        {
                            dpadPressedInMenu = true;
                            if ( fpsCounterInMenu == 0 )
                                CurrentScreen.NextButton();
                        }
                        //else if(combat != null && tacticsManager.isActiveAndEnabled) {
                        //    DpadInUse = true;
                        //    //newHUD.ChangeGroupState( tacticsManager.CurrentShowedGroup, 2 );
                        //    //tacticsManager.AssignOrderToGroup(GroupBehaviour.State.RangeAttack, tacticsManager.CurrentShowedGroup);

                        //    bool hasAssignedOrder = tacticsManager.AssignOrder(GroupBehaviour.State.RangeAttack);

                        //    if (hasAssignedOrder)
                        //    {
                        //        newHUD.ChangeGroupState(tacticsManager.CurrentMostRepresentedGroup, 2);
                        //    }

                        //    StartCoroutine(DpadWait(dpadWaitTime));
                        //}

                    }
                }

                //DPad HELD DOWN
                if ( InputManager.Instance.DpadVertical() < -0.7f )
                {
                    dpadDownOld = InputManager.Instance.DpadVertical();

                    if ( combat != null && tacticsManager.isActiveAndEnabled && dpadDownOld != 0f )
                    {
                        DpadInUse = true;

                        if ( allGroupsOrderStartTimeDown == 0f )
                            allGroupsOrderStartTimeDown = Time.time;

                        if ( (Time.time - allGroupsOrderStartTimeDown) >= heldTime )
                        {
                            newHUD.ChangeGroupState( GroupManager.Group.GroupAzure , 2 );
                            newHUD.ChangeGroupState( GroupManager.Group.GroupPink , 2 );
                            newHUD.ChangeGroupState( GroupManager.Group.GroupGreen , 2 );
                            newHUD.ChangeGroupState( GroupManager.Group.GroupYellow , 2 );
                            tacticsManager.AllGroupsOrder( GroupBehaviour.State.RangeAttack );
                            DpadInUse = false;
                            allGroupsOrderStartTimeDown = 0f;
                            dpadDownOld = 0f;
                        }
                    }
                }
                else if ( -0.7f < InputManager.Instance.DpadVertical() && InputManager.Instance.DpadVertical() <= 0f )
                {
                    dpadDownOld = 0f;
                    allGroupsOrderStartTimeDown = 0f;
                }

                // DPad RIGHT
                if ( InputManager.Instance.DpadHorizontal() > 0.7f && !NavigatingMenu )
                {
                    //if(combat != null && tacticsManager.isActiveAndEnabled && !DpadInUse) {
                    //    DpadInUse = true;
                    //    //newHUD.ChangeGroupState( tacticsManager.CurrentShowedGroup, 1 );
                    //    //tacticsManager.AssignOrderToGroup(GroupBehaviour.State.Tank, tacticsManager.CurrentShowedGroup);

                    //    bool hasAssignedOrder = tacticsManager.AssignOrder(GroupBehaviour.State.Tank);

                    //    if (hasAssignedOrder)
                    //    {
                    //        newHUD.ChangeGroupState(tacticsManager.CurrentMostRepresentedGroup, 1);
                    //    }

                    //    StartCoroutine(DpadWait(dpadWaitTime));
                    //}
                }

                //DPad HELD RIGHT
                if ( InputManager.Instance.DpadHorizontal() > 0.7f )
                {
                    dpadRightOld = InputManager.Instance.DpadHorizontal();

                    if ( combat != null && tacticsManager.isActiveAndEnabled && dpadRightOld != 0f )
                    {
                        DpadInUse = true;

                        if ( allGroupsOrderStartTimeRight == 0f )
                            allGroupsOrderStartTimeRight = Time.time;

                        if ( (Time.time - allGroupsOrderStartTimeRight) >= heldTime )
                        {
                            newHUD.ChangeGroupState( GroupManager.Group.GroupAzure , 1 );
                            newHUD.ChangeGroupState( GroupManager.Group.GroupPink , 1 );
                            newHUD.ChangeGroupState( GroupManager.Group.GroupGreen , 1 );
                            newHUD.ChangeGroupState( GroupManager.Group.GroupYellow , 1 );
                            tacticsManager.AllGroupsOrder( GroupBehaviour.State.Tank );
                            DpadInUse = false;
                            allGroupsOrderStartTimeRight = 0f;
                            dpadRightOld = 0f;
                        }
                    }
                }
                else if ( 0f <= InputManager.Instance.DpadHorizontal() && InputManager.Instance.DpadHorizontal() < 0.7f )
                {
                    dpadRightOld = 0f;
                    allGroupsOrderStartTimeRight = 0f;
                }

                // DPad LEFT
                if ( InputManager.Instance.DpadHorizontal() < -0.7f && !NavigatingMenu )
                {
                    //if(combat != null && tacticsManager.isActiveAndEnabled && !DpadInUse) {
                    //    DpadInUse = true;

                    //    bool hasAssignedOrder = tacticsManager.AssignOrder(GroupBehaviour.State.Recruit);

                    //    if (hasAssignedOrder)
                    //    {
                    //        newHUD.ChangeGroupState(tacticsManager.CurrentMostRepresentedGroup, 4);
                    //    }

                    //    StartCoroutine(DpadWait(dpadWaitTime));
                    //}
                }

                //DPad HELD LEFT
                if ( InputManager.Instance.DpadHorizontal() < -0.7f )
                {
                    dpadLeftOld = InputManager.Instance.DpadHorizontal();

                    if ( combat != null && tacticsManager.isActiveAndEnabled && dpadLeftOld != 0f )
                    {
                        DpadInUse = true;

                        if ( allGroupsOrderStartTimeLeft == 0f )
                            allGroupsOrderStartTimeLeft = Time.time;

                        if ( (Time.time - allGroupsOrderStartTimeLeft) >= heldTime )
                        {
                            newHUD.ChangeGroupState( GroupManager.Group.GroupAzure , 4 );
                            newHUD.ChangeGroupState( GroupManager.Group.GroupPink , 4 );
                            newHUD.ChangeGroupState( GroupManager.Group.GroupGreen , 4 );
                            newHUD.ChangeGroupState( GroupManager.Group.GroupYellow , 4 );
                            tacticsManager.AllGroupsOrder( GroupBehaviour.State.Recruit );
                            DpadInUse = false;
                            allGroupsOrderStartTimeLeft = 0f;
                            dpadLeftOld = 0f;
                        }
                    }
                }
                else if ( -0.7f < InputManager.Instance.DpadHorizontal() && InputManager.Instance.DpadHorizontal() <= 0f )
                {
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
        //else
        //Debug.Log( name + " PlayerInput cannot find InputManager" );
    }

    private void OnDeath( Stats sender )
    {
        this.enabled = false;
    }

    private void OnBattlePreparation()
    {
        canGiveInput = false;
        playerController.PassXZValues( 0 , 0 );
    }

    private void OnBattleEnter()
    {
        canGiveInput = true;
    }

    public void DisableOrders()
    {
        HasHat = false;
    }

    public void DisableLeftStick()
    {
        playerController.XMovement = 0f;
        playerController.ZMovement = 0f;
        Attacking = true;
    }

    public void EnableLeftStick()
    {
        Attacking = false;
    }

    private void OnReincarnation( GameObject player )
    {
        DisableOrders();
    }
}
