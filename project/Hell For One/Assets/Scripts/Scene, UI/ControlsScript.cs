using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsScript : Menu {

    public override void PressSelectedButton() {

        switch(ElementIndex) {
            // BACK
            case 0:
                TransitionTo(ParentScreen);
                break;
            default:
                break;
        }
    }
}
