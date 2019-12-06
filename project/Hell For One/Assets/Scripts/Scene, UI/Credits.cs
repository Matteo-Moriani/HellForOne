using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : Menu
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
