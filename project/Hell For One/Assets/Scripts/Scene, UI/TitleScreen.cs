using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : Menu {

    public GameObject creditsUI;

    public override void PressSelectedButton() {
        
        switch(ElementIndex) {
            // PLAY
            case 0:
                // TODO - cambiare tramite index, rivedi il tutorial
                SceneManager.LoadScene("Demo");
                // TODO - capire se funziona
                Managers.Instance.RaiseOnPressPlayButton();
                break;
            // CREDITS
            case 1:
                TransitionTo(creditsUI);
                break;
            // QUIT
            case 2:
                Application.Quit();
                break;
            default:
                break;
        }
    }

    public override void Back() {
        // you can't go back from a title screen
    }



}
