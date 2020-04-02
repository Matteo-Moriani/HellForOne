using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInput : GeneralInput {

    public GameObject rootScreen;

    private IEnumerator DpadWait(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        DpadInUse = false;
    }

    private void Awake() {
        playerController = gameObject.GetComponent<PlayerController>();
        canGiveInput = true;
        CurrentScreen = rootScreen.GetComponent<Menu>();
    }

    private void Update() {
        if(InputManager.Instance != null) {
            if(dpadPressedInMenu ) {
                if(fpsCounterInMenu >= 8) {
                    fpsCounterInMenu = 0;
                    dpadPressedInMenu = false;
                }
                else
                    fpsCounterInMenu++;
            }

            // TODO - andare su e giu anche con le levette?
            

            // Circle (PS3) / B (XBOX) 
            if(InputManager.Instance.CircleButtonDown()) {
                CurrentScreen.Back();
            }

            // Start (PS3) / Options (PS4)
            if(InputManager.Instance.StartButtonDown() && CurrentScreen.GetComponent<PauseScreen>()) {
                CurrentScreen.GetComponent<PauseScreen>().Resume();
                GameEvents.RaiseOnResume();
            }

            // Cross (PS3) / A (XBOX)
            if(InputManager.Instance.XButtonDown()) {
                CurrentScreen.PressSelectedButton();
            }

            // DPad UP
            if(InputManager.Instance.DpadVertical() > 0.7f) {
                if(!DpadInUse) {
                    dpadPressedInMenu = true;
                    if(fpsCounterInMenu == 0)
                        CurrentScreen.PreviousButton();
                }
            }

            // DPad DOWN
            if(InputManager.Instance.DpadVertical() < -0.7f) {
                if(!DpadInUse) {
                    dpadPressedInMenu = true;
                    if(fpsCounterInMenu == 0)
                        CurrentScreen.NextButton();
                }
            }
        }
        //else {
        //    Debug.Log("Can't find an input manager");
        //}
    }
}
