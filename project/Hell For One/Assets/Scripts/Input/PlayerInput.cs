using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private InputManager inputManager;

    public int fpsCounterInMenu = 0;
    private bool dpadPressedInMenu = false;
    private Dash dash;
    private Combat combat;
    private TacticsManager tacticsManager;
    private PauseScript pauseScript;
    private TitleScreen titleScreen;
    private float dpadWaitTime = 0.2f;
    private bool dpadInUse = false;
    private bool gameInPause = false;
    public bool GameInPause { get => gameInPause; set => gameInPause = value; }
    private bool inTitleScreen = false;
    public bool InTitleScreen { get => inTitleScreen; set => inTitleScreen = value; }

    public bool DpadInUse { get => dpadInUse; set => dpadInUse = value; }

    private IEnumerator DpadWait( float waitTime )
    {
        yield return new WaitForSeconds( waitTime );
        DpadInUse = false;
    }

    private void Start()
    {
        dash = GetComponent<Dash>();
        combat = GetComponent<Combat>();
        inputManager = GameObject.FindGameObjectWithTag( "InputManager" ).GetComponent<InputManager>();

        titleScreen = GameObject.FindGameObjectWithTag("Canvas").GetComponent<TitleScreen>();

        if(titleScreen != null)
            InTitleScreen = true;
        else {
            tacticsManager = GetComponent<TacticsManager>();
            pauseScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PauseScript>();
        }
        
    }

    private void Update()
    {
        if ( inputManager != null )
        {
            if ( dpadPressedInMenu && (GameInPause || InTitleScreen))
            {
                if ( fpsCounterInMenu >= 8 )
                {
                    fpsCounterInMenu = 0;
                    dpadPressedInMenu = false;
                }
                else
                    fpsCounterInMenu++;
            }

            // Circle (PS3) / B (XBOX) 
            if ( inputManager.CircleButtonDown() )
            {
                if(GameInPause) {
                    if(pauseScript.CurrentMenu != MenuType.pause)
                        pauseScript.Back();
                    else
                        pauseScript.Resume();
                }
                else if(InTitleScreen) {
                    if(titleScreen.CurrentScreen != Screen.title)
                        titleScreen.Back();
                }
                else {
                    if(dash != null) {
                        dash.TryDash(inputManager.LeftStickVertical(), inputManager.LeftStickHorizontal());
                    }
                }
               
            }

            // Cross (PS3) / A (XBOX)
            if ( inputManager.XButtonDown() )
            {
                if(GameInPause) {
                    pauseScript.PressSelectedButton();
                }
                else if(InTitleScreen) {
                    titleScreen.PressSelectedButton();
                }
                else if(combat != null && tacticsManager) {
                    // TODO - dialogues
                }

            }
        

            // Square (PS3) / X (XBOX)
            if ( inputManager.SquareButtonDown() && !inTitleScreen && !GameInPause)
            {
                if ( combat != null )
                {
                    combat.PlayerAttack();
                }
            }

            // L1 (PS3) / LB (XBOX) - Down
            if ( inputManager.L1ButtonDown() && !inTitleScreen && !GameInPause)
            {
                if ( combat != null )
                {
                    combat.StartBlock();
                }
            }

            // L1 (PS3) / LB (XOBX) - Up
            if ( inputManager.L1ButtonUp() && !inTitleScreen && !GameInPause)
            {
                if ( combat != null )
                {
                    combat.StopBlock();
                }
            }

            // R1 (PS3) / RB (XBOX) - Down
            if ( inputManager.R1ButtonDown() && !inTitleScreen && !GameInPause)
            {
                if ( combat != null )
                {
                    combat.StartBlock();
                }
            }

            // R1 (PS3) / RB (XOBX) - Up
            if ( inputManager.R1ButtonUp() && !inTitleScreen && !GameInPause)
            {
                if ( combat != null )
                {
                    combat.StopBlock();
                }
            }

            // Triangle (PS3) / Y (XBOX)
            if ( inputManager.TriangleButtonDown() && !inTitleScreen && !GameInPause)
            {
                if ( combat != null )
                {
                    combat.RangedAttack( null );
                }
            }

            // L2 (PS3) / LT (XBOX) - Down
            //if ( inputManager.L2Axis() )
            if ( inputManager.L2ButtonDown() && !inTitleScreen && !GameInPause)
            {
                if ( combat != null && tacticsManager )
                {
                    tacticsManager.RotateLeftGroups();
                }
            }

            // DPad UP
            if ( inputManager.DpadVertical() > 0.7f )
            {
                if ( !DpadInUse ) {
                    
                    if(GameInPause) {
                        dpadPressedInMenu = true;
                        if (fpsCounterInMenu == 0)
                            pauseScript.PreviousButton();
                    }
                    else if(InTitleScreen) {
                        dpadPressedInMenu = true;
                        if(fpsCounterInMenu == 0)
                            titleScreen.PreviousButton();
                    }
                    else if (combat != null && tacticsManager.isActiveAndEnabled ) {
                        DpadInUse = true;
                        tacticsManager.AssignOrderToGroup(GroupBehaviour.State.MeleeAttack, tacticsManager.CurrentShowedGroup);
                        StartCoroutine(DpadWait(dpadWaitTime));
                    }

                }
            }

            // DPad DOWN
            if ( inputManager.DpadVertical() < -0.7f )
            {
                if ( !DpadInUse) {

                    if(GameInPause) {
                        dpadPressedInMenu = true;
                        if ( fpsCounterInMenu == 0 )
                            pauseScript.NextButton();
                    }
                    else if(InTitleScreen) {
                        dpadPressedInMenu = true;
                        if(fpsCounterInMenu == 0)
                            titleScreen.NextButton();
                    }
                    else if (combat != null && tacticsManager.isActiveAndEnabled ) {
                        DpadInUse = true;
                        tacticsManager.AssignOrderToGroup(GroupBehaviour.State.RangeAttack, tacticsManager.CurrentShowedGroup);
                        StartCoroutine(DpadWait(dpadWaitTime));
                    }
                    
                }
            }

            // DPad RIGHT
            if ( inputManager.DpadHorizontal() > 0.7f && !inTitleScreen && !GameInPause)
            {
                if ( combat != null && tacticsManager.isActiveAndEnabled && !DpadInUse) {
                    DpadInUse = true;
                    tacticsManager.AssignOrderToGroup( GroupBehaviour.State.Tank, tacticsManager.CurrentShowedGroup );
                    StartCoroutine( DpadWait( dpadWaitTime ) );
                }
            }

            // DPad LEFT
            if ( inputManager.DpadHorizontal() < -0.7f && !inTitleScreen && !GameInPause)
            {
                if ( combat != null && tacticsManager.isActiveAndEnabled && !DpadInUse) {
                    DpadInUse = true;
                    tacticsManager.AssignOrderToGroup( GroupBehaviour.State.Support, tacticsManager.CurrentShowedGroup );
                    StartCoroutine( DpadWait( dpadWaitTime ) );
                }
            }

            // Need in order to set an internal bool in input manager
            // Im looking for a better solution
            if ( inputManager.L2ButtonUp() && !inTitleScreen && !GameInPause) { }

            // R2 (PS3) / RT (XBOX) - Down
            //if ( inputManager.R2Axis() )
            if ( inputManager.R2ButtonDown() && !inTitleScreen && !GameInPause)
            {
                if ( combat != null && tacticsManager )
                {
                    tacticsManager.RotateRightGroups();
                }
            }

            // Need in order to set an internal bool in input manager
            // Im looking for a better solution
            if ( inputManager.R2ButtonUp() && !inTitleScreen && !GameInPause) { }

            // Start (PS3) / Options (PS4)
            if(inputManager.PauseButtonDown() && !inTitleScreen) {
                if(combat != null && pauseScript) {
                    if(GameInPause) {
                        pauseScript.Resume();
                    } else {
                        GameInPause = true;
                        pauseScript.Pause();
                    }
                }
            }
        }
        else
        {
            Debug.Log( name + " PlayerInput cannot find InputManager" );
        }
    }
}
