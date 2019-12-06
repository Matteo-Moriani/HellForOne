using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    private InputManager inputManager;

    public int fpsCounterInMenu = 0;
    private bool dpadPressedInMenu = false;
    private Dash dash;
    private Combat combat;
    private TacticsManager tacticsManager;
    private float dpadWaitTime = 0.2f;
    private bool dpadInUse = false;
    public bool DpadInUse { get => dpadInUse; set => dpadInUse = value; }
    private bool gameInPause = false;
    public bool GameInPause { get => gameInPause; set => gameInPause = value; }
    private bool navigatingMenu = false;
    public bool NavigatingMenu { get => navigatingMenu; set => navigatingMenu = value; }
    private GameObject currentScreen;
    public GameObject CurrentScreen { get => currentScreen; set => currentScreen = value; }
    // is initially inactive so I must put it manually from the inspector
    public GameObject pauseScreen;


    private IEnumerator DpadWait(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        DpadInUse = false;
    }

    private void Start() {
        dash = GetComponent<Dash>();
        combat = GetComponent<Combat>();
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();

        if(GameObject.Find("TitleScreen")) {
            NavigatingMenu = true;
            CurrentScreen = GameObject.Find("TitleScreen");
        }
        else {
            tacticsManager = GetComponent<TacticsManager>();
            CurrentScreen = pauseScreen;
        }

    }

    private void Update() {
        if(inputManager != null) {
            if(dpadPressedInMenu && (NavigatingMenu)) {
                if(fpsCounterInMenu >= 8) {
                    fpsCounterInMenu = 0;
                    dpadPressedInMenu = false;
                }
                else
                    fpsCounterInMenu++;
            }

            // Circle (PS3) / B (XBOX) 
            if(inputManager.CircleButtonDown()) {
                if(NavigatingMenu) 
                    currentScreen.GetComponent<Screen>().Back();
                else {
                    if(dash != null) {
                        dash.TryDash(inputManager.LeftStickVertical(), inputManager.LeftStickHorizontal());
                    }
                }

            }

            // Cross (PS3) / A (XBOX)
            if(inputManager.XButtonDown()) {
                if(NavigatingMenu)
                    currentScreen.GetComponent<Screen>().PressSelectedButton();
                else if(combat != null && tacticsManager) {
                    // TODO - dialogues
                }

            }


            // Square (PS3) / X (XBOX)
            if(inputManager.SquareButtonDown() && !NavigatingMenu) {
                if(combat != null) {
                    combat.PlayerAttack();
                }
            }

            // L1 (PS3) / LB (XBOX) - Down
            if(inputManager.L1ButtonDown() && !NavigatingMenu) {
                if(combat != null) {
                    combat.StartBlock();
                }
            }

            // L1 (PS3) / LB (XOBX) - Up
            if(inputManager.L1ButtonUp() && !NavigatingMenu) {
                if(combat != null) {
                    combat.StopBlock();
                }
            }

            // R1 (PS3) / RB (XBOX) - Down
            if(inputManager.R1ButtonDown() && !NavigatingMenu) {
                if(combat != null) {
                    combat.StartBlock();
                }
            }

            // R1 (PS3) / RB (XOBX) - Up
            if(inputManager.R1ButtonUp() && !NavigatingMenu) {
                if(combat != null) {
                    combat.StopBlock();
                }
            }

            // Triangle (PS3) / Y (XBOX)
            if(inputManager.TriangleButtonDown() && !NavigatingMenu) {
                if(combat != null) {
                    combat.RangedAttack(null);
                }
            }

            // L2 (PS3) / LT (XBOX) - Down
            //if ( inputManager.L2Axis() )
            if(inputManager.L2ButtonDown() && !NavigatingMenu) {
                if(combat != null && tacticsManager) {
                    tacticsManager.RotateLeftGroups();
                }
            }

            // DPad UP
            if(inputManager.DpadVertical() > 0.7f) {
                if(!DpadInUse) {

                    if(NavigatingMenu) {
                        dpadPressedInMenu = true;
                        if(fpsCounterInMenu == 0)
                            currentScreen.GetComponent<Screen>().PreviousButton();
                    }
                    else if(combat != null && tacticsManager.isActiveAndEnabled) {
                        DpadInUse = true;
                        tacticsManager.AssignOrderToGroup(GroupBehaviour.State.MeleeAttack, tacticsManager.CurrentShowedGroup);
                        StartCoroutine(DpadWait(dpadWaitTime));
                    }

                }
            }

            // DPad DOWN
            if(inputManager.DpadVertical() < -0.7f) {
                if(!DpadInUse) {

                    if(NavigatingMenu) {
                        dpadPressedInMenu = true;
                        if(fpsCounterInMenu == 0)
                            currentScreen.GetComponent<Screen>().NextButton();
                    }
                    else if(combat != null && tacticsManager.isActiveAndEnabled) {
                        DpadInUse = true;
                        tacticsManager.AssignOrderToGroup(GroupBehaviour.State.RangeAttack, tacticsManager.CurrentShowedGroup);
                        StartCoroutine(DpadWait(dpadWaitTime));
                    }

                }
            }

            // DPad RIGHT
            if(inputManager.DpadHorizontal() > 0.7f && !NavigatingMenu) {
                if(combat != null && tacticsManager.isActiveAndEnabled && !DpadInUse) {
                    DpadInUse = true;
                    tacticsManager.AssignOrderToGroup(GroupBehaviour.State.Tank, tacticsManager.CurrentShowedGroup);
                    StartCoroutine(DpadWait(dpadWaitTime));
                }
            }

            // DPad LEFT
            if(inputManager.DpadHorizontal() < -0.7f && !NavigatingMenu) {
                if(combat != null && tacticsManager.isActiveAndEnabled && !DpadInUse) {
                    DpadInUse = true;
                    tacticsManager.AssignOrderToGroup(GroupBehaviour.State.Support, tacticsManager.CurrentShowedGroup);
                    StartCoroutine(DpadWait(dpadWaitTime));
                }
            }

            // Need in order to set an internal bool in input manager
            // Im looking for a better solution
            if(inputManager.L2ButtonUp() && !NavigatingMenu) { }

            // R2 (PS3) / RT (XBOX) - Down
            //if ( inputManager.R2Axis() )
            if(inputManager.R2ButtonDown() && !NavigatingMenu) {
                if(combat != null && tacticsManager) {
                    tacticsManager.RotateRightGroups();
                }
            }

            // Need in order to set an internal bool in input manager
            // Im looking for a better solution
            if(inputManager.R2ButtonUp() && !NavigatingMenu) { }

            // Start (PS3) / Options (PS4)
            if(inputManager.PauseButtonDown() ) {
                if(combat != null) {
                    if(GameInPause && NavigatingMenu) {
                        currentScreen.GetComponent<PauseScreen>().Resume();
                    }
                    else {
                        currentScreen.SetActive(true);
                        NavigatingMenu = true;
                        currentScreen.GetComponent<PauseScreen>().Pause();
                    }
                }
            }
        }
        else {
            Debug.Log(name + " PlayerInput cannot find InputManager");
        }
    }
}
