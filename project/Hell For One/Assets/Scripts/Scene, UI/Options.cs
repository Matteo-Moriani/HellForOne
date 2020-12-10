using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : Menu {

    public override void PressSelectedButton() {

        switch(ElementIndex) {
            // SHOW/HIDE ORDERS CROSS
            case 0:
                SwitchToggle();
                break;
            // INVERT/NOT X CAMERA AXIS
            case 1:
                SwitchToggle();
                break;
            // BACK
            case 2:
                TransitionTo(ParentScreen);
                break;
            default:
                break;
        }
    }

    public void ShowOrdersCross(bool b) {
        //GameObject.FindGameObjectWithTag("HUD").GetComponent<NewHUD>().OrdersCross.SetActive(b);
    }

    public void InvertXCameraAxis() {
        GameObject.FindGameObjectWithTag("ThirdPersonCamera").GetComponent<CameraInputAssign>().InvertAxis_X();
    }

    private void SwitchToggle() {
        if(elements[ElementIndex].GetComponent<Toggle>().isOn)
            elements[ElementIndex].GetComponent<Toggle>().isOn = false;
        else
            elements[ElementIndex].GetComponent<Toggle>().isOn = true;
    }
}
