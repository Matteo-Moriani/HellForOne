using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : Screen
{
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
