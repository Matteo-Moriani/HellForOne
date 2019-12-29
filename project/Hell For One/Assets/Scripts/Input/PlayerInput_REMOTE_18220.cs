using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : GeneralInput {
    
    private Dash dash;
    private Combat combat;
    private TacticsManager tacticsManager;
    private bool gameInPause = false;
    public bool GameInPause { get => gameInPause; set => gameInPause = value; }
    private bool playing = true;
    public bool Playing { get => playing; set => playing = value; }
    private bool navigatingMenu = false;
    public bool NavigatingMenu { get => navigatingMenu; set => navigatingMenu = value; }
    public bool InCutscene { get => inCutscene; set => inCutscene = value; }

    private GameObject pauseScreen;
    private CombatEventsManager combatEventsManager;
    private bool inCutscene = false;
    

    private IEnumerator DpadWait(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        DpadInUse = false;
    }

    private void Awake()
    {
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        FindPauseScreen();
    }

    private void FindPauseScreen() {

        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        int childrenNum = canvas.transform.childCount;

        for(int i = 0; i < childrenNum; i++) {
            if(canvas.transform.GetChild(i).name == "PauseScreen") {
                pauseScreen = canvas.transform.GetChild(i).gameObject;
                break;
            }
        }
    }

    private void OnEnable() {
    //    Managers.Instance.onPressPlayButton += GameStart;
        if(combatEventsManager != null) { 
            combatEventsManager.onDeath += OnDeath;
        }
        BattleEventsManager.onBattlePreparation += OnBattlePreparation;
        BattleEventsManager.onBossBattleEnter += OnBossBattleEnter;
    }

    private void OnDisable() {
        //    Managers.Instance.onPressPlayButton -= GameStart;
        if (combatEventsManager != null)
        {
            combatEventsManager.onDeath -= OnDeath;
        }
        BattleEventsManager.onBattlePreparation -= OnBattlePreparation;
        BattleEventsManager.onBossBattleEnter -= OnBossBattleEnter;
    }

    public void Start() {
        controller = this.gameObject.GetComponent<Controller>();
        canGiveInput = true;
        NavigatingMenu = false;
        dash = GetComponent<Dash>();
        combat = GetComponent<Combat>();
        tacticsManager = GetComponent<TacticsManager>();
        CurrentScreen = pauseScreen.GetComponent<Menu>();
    }

    private void Update() {
        if(InputManager.Instance != null && !InCutscene) {

            if(dpadPressedInMenu && (NavigatingMenu)) {
                if(fpsCounterInMenu >= 8) {
                    fpsCounterInMenu = 0;
                    dpadPressedInMenu = false;
                }
                else
                    fpsCounterInMenu++;
            }

            // Left stick (PS3 & XBOX)
            if(controller != null) { 
                controller.PassXZValues(InputManager.Instance.LeftStickHorizontal(),InputManager.Instance.LeftStickVertical());    
            }

            // Circle (PS3) / B (XBOX) 
            if(InputManager.Instance.CircleButtonDown()) {
                if(NavigatingMenu) 
                    CurrentScreen.Back();
                else {
                    if(dash != null) {
                        dash.TryDash(InputManager.Instance.LeftStickVertical(), InputManager.Instance.LeftStickHorizontal());
                    }
                }

            }

            // Cross (PS3) / A (XBOX)
            if(InputManager.Instance.XButtonDown()) {
                if(NavigatingMenu)
                    CurrentScreen.PressSelectedButton();
                else if(combat != null && tacticsManager) {
                    // TODO - dialogues
                }

            }

            // Square (PS3) / X (XBOX)
            if(InputManager.Instance.SquareButtonDown() && !NavigatingMenu) {
                if(combat != null) {
                    combat.PlayerAttack();
                }
            }

            // L1 (PS3) / LB (XBOX) - Down
            if(InputManager.Instance.L1ButtonDown() && !NavigatingMenu) {
                if(combat != null) {
                    combat.StartBlock();
                }
            }

            // L1 (PS3) / LB (XOBX) - Up
            if(InputManager.Instance.L1ButtonUp() && !NavigatingMenu) {
                if(combat != null) {
                    combat.StopBlock();
                }
            }

            // R1 (PS3) / RB (XBOX) - Down
            if(InputManager.Instance.R1ButtonDown() && !NavigatingMenu) {
                if(combat != null) {
                    combat.StartBlock();
                }
            }

            // R1 (PS3) / RB (XOBX) - Up
            if(InputManager.Instance.R1ButtonUp() && !NavigatingMenu) {
                if(combat != null) {
                    combat.StopBlock();
                }
            }

            // Triangle (PS3) / Y (XBOX)
            if(InputManager.Instance.TriangleButtonDown() && !NavigatingMenu) {
                if(combat != null) {
                    combat.RangedAttack(null);
                }
            }

            // L2 (PS3) / LT (XBOX) - Down
            //if ( inputManager.L2Axis() )
            if(InputManager.Instance.L2ButtonDown() && !NavigatingMenu) {
                if(combat != null && tacticsManager) {
                    tacticsManager.RotateLeftGroups();
                }
            }

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
                        tacticsManager.AssignOrderToGroup(GroupBehaviour.State.MeleeAttack, tacticsManager.CurrentShowedGroup);
                        StartCoroutine(DpadWait(dpadWaitTime));
                    }

                }
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
                        tacticsManager.AssignOrderToGroup(GroupBehaviour.State.RangeAttack, tacticsManager.CurrentShowedGroup);
                        StartCoroutine(DpadWait(dpadWaitTime));
                    }

                }
            }

            // DPad RIGHT
            if(InputManager.Instance.DpadHorizontal() > 0.7f && !NavigatingMenu) {
                if(combat != null && tacticsManager.isActiveAndEnabled && !DpadInUse) {
                    DpadInUse = true;
                    tacticsManager.AssignOrderToGroup(GroupBehaviour.State.Tank, tacticsManager.CurrentShowedGroup);
                    StartCoroutine(DpadWait(dpadWaitTime));
                }
            }

            // DPad LEFT
            if(InputManager.Instance.DpadHorizontal() < -0.7f && !NavigatingMenu) {
                if(combat != null && tacticsManager.isActiveAndEnabled && !DpadInUse) {
                    DpadInUse = true;
                    tacticsManager.AssignOrderToGroup(GroupBehaviour.State.Support, tacticsManager.CurrentShowedGroup);
                    StartCoroutine(DpadWait(dpadWaitTime));
                }
            }

            // Need in order to set an internal bool in input manager
            // Im looking for a better solution
            if(InputManager.Instance.L2ButtonUp() && !NavigatingMenu) { }

            // R2 (PS3) / RT (XBOX) - Down
            //if ( inputManager.R2Axis() )
            if(InputManager.Instance.R2ButtonDown() && !NavigatingMenu) {
                if(combat != null && tacticsManager) {
                    tacticsManager.RotateRightGroups();
                }
            }

            // Need in order to set an internal bool in input manager
            // Im looking for a better solution
            if(InputManager.Instance.R2ButtonUp() && !NavigatingMenu) { }

            // Start (PS3) / Options (PS4)
            if(InputManager.Instance.PauseButtonDown() ) {
                if(combat != null) {
                    if(GameInPause && NavigatingMenu) {
                        CurrentScreen.GetComponent<PauseScreen>().Resume();
                    }
                    else {
                        CurrentScreen.gameObject.SetActive(true);
                        NavigatingMenu = true;
                        CurrentScreen.GetComponent<PauseScreen>().Pause();
                    }
                }
            }
        }
    }

    private void OnDeath() { 
        this.enabled = false;    
    }

    private void OnBattlePreparation() { 
        canGiveInput = false;
        controller.PassXZValues(0,0);
    }

    private void OnBossBattleEnter() { 
        canGiveInput = true;    
    }
}
