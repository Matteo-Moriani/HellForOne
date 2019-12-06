using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : Menu {

    public override void PressSelectedButton() {

        switch(ButtonIndex) {
            // BACK
            case 0:
                TransitionTo(ParentScreen);
                break;
            default:
                break;
        }
    }
}
