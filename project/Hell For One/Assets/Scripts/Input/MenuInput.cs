using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInput : GeneralInput {

    private IEnumerator DpadWait(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        DpadInUse = false;
    }

    private void Awake() {
        controller = gameObject.GetComponent<Controller>();
        canGiveInput = true;
        CurrentScreen = GameObject.FindGameObjectWithTag("RootScreen").GetComponent<Menu>();
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

            // Left stick (PS3 & XBOX)
            //if(controller != null) {
            //    controller.PassXZValues(InputManager.Instance.LeftStickHorizontal(), InputManager.Instance.LeftStickVertical());
            //}



            // tutti metodi abstract con il nome del tasto da premere

            // Circle (PS3) / B (XBOX) 
            if(InputManager.Instance.CircleButtonDown()) {
                CurrentScreen.Back();
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
        else {
            Debug.Log("Can't find an input manager");
        }
    }
}
