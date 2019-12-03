using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private InputManager inputManager;

    private Dash dash;
    private Combat combat;
    private TacticsManager tacticsManager;
    private PauseScript pauseScript;
    private float dpadWaitTime = 0.2f;
    private bool dpadInUse = false;
    private bool gameIsPaused = false;
    public bool GameIsPaused { get => gameIsPaused; set => gameIsPaused = value; }

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
        tacticsManager = GetComponent<TacticsManager>();
        pauseScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PauseScript>();
    }

    private void Update()
    {
        if ( inputManager != null )
        {

            // Circle (PS3) / B (XBOX) 
            if ( inputManager.CircleButtonDown() )
            {
                if(GameIsPaused) {
                    if(pauseScript.CurrentMenu != MenuType.pause)
                        pauseScript.Back();
                    else
                        pauseScript.Resume();
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
                if ( combat != null && tacticsManager )
                {
                    if(GameIsPaused) {
                        pauseScript.PressSelectedButton();
                    }

                    // TODO - dialogues
                }
            }

            // Square (PS3) / X (XBOX)
            if ( inputManager.SquareButtonDown() )
            {
                if ( combat != null )
                {
                    combat.SingleAttack();
                }
            }

            // L1 (PS3) / LB (XBOX) - Down
            if ( inputManager.L1ButtonDown() )
            {
                if ( combat != null )
                {
                    combat.StartBlock();
                }
            }

            // L1 (PS3) / LB (XOBX) - Up
            if ( inputManager.L1ButtonUp() )
            {
                if ( combat != null )
                {
                    combat.StopBlock();
                }
            }

            // Triangle (PS3) / Y (XBOX)
            if ( inputManager.TriangleButtonDown() )
            {
                if ( combat != null )
                {
                    combat.RangedAttack( null );
                }
            }

            // L2 (PS3) / LT (XBOX) - Down
            //if ( inputManager.L2Axis() )
            if ( inputManager.L2ButtonDown() )
            {
                if ( combat != null && tacticsManager )
                {
                    tacticsManager.RotateLeftGroups();
                }
            }

            // DPad UP
            if ( inputManager.DpadVertical() > 0.7f )
            {
                if ( combat != null && tacticsManager && !DpadInUse ) {
                    
                    if(GameIsPaused) {
                        pauseScript.PreviousButton();
                    }
                    else {
                        DpadInUse = true;
                        tacticsManager.AssignOrderToGroup(GroupBehaviour.State.MeleeAttack, tacticsManager.CurrentShowedGroup);
                        StartCoroutine(DpadWait(dpadWaitTime));
                    }

                }
            }

            // DPad DOWN
            if ( inputManager.DpadVertical() < -0.7f )
            {
                if ( combat != null && tacticsManager && !DpadInUse) {

                    if(GameIsPaused) {
                        pauseScript.NextButton();
                    }
                    else {
                        DpadInUse = true;
                        tacticsManager.AssignOrderToGroup(GroupBehaviour.State.RangeAttack, tacticsManager.CurrentShowedGroup);
                        StartCoroutine(DpadWait(dpadWaitTime));
                    }
                    
                }
            }

            // DPad RIGHT
            if ( inputManager.DpadHorizontal() > 0.7f )
            {
                if ( combat != null && tacticsManager && !DpadInUse) {
                    DpadInUse = true;
                    tacticsManager.AssignOrderToGroup( GroupBehaviour.State.Tank, tacticsManager.CurrentShowedGroup );
                    StartCoroutine( DpadWait( dpadWaitTime ) );
                }
            }

            // DPad LEFT
            if ( inputManager.DpadHorizontal() < -0.7f )
            {
                if ( combat != null && tacticsManager && !DpadInUse) {
                    DpadInUse = true;
                    tacticsManager.AssignOrderToGroup( GroupBehaviour.State.Support, tacticsManager.CurrentShowedGroup );
                    StartCoroutine( DpadWait( dpadWaitTime ) );
                }
            }

            // Need in order to set an internal bool in input manager
            // Im looking for a better solution
            if ( inputManager.L2ButtonUp() ) { }

            // R2 (PS3) / RT (XBOX) - Down
            //if ( inputManager.R2Axis() )
            if ( inputManager.R2ButtonDown() )
            {
                if ( combat != null && tacticsManager )
                {
                    tacticsManager.RotateRightGroups();
                }
            }

            // Need in order to set an internal bool in input manager
            // Im looking for a better solution
            if ( inputManager.R2ButtonUp() ) { }

            // Start (PS3) / Options (PS4)
            if(inputManager.PauseButtonDown()) {
                if(combat != null && pauseScript) {
                    gameIsPaused = true;
                    pauseScript.Pause();
                }
            }
        }
        else
        {
            Debug.Log( name + " PlayerInput cannot find InputManager" );
        }
    }
}
