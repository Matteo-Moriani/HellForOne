using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : Menu {

    public GameObject optionsUI;
    public GameObject creditsUI;

    public override void PressSelectedButton() {
        
        switch(ButtonIndex) {
            // PLAY
            case 0:
                // TODO - cambiare in index, rivedi tutorial
                SceneManager.LoadScene("Demo");
                // TODO - capire se funziona
                Managers.Instance.RaiseOnPressPlayButton();
                break;
            // OPTIONS
            case 1:
                TransitionTo(optionsUI);
                break;
            // CREDITS
            case 2:
                TransitionTo(creditsUI);
                break;
            // QUIT
            case 3:
                Debug.Log("Quit!");
                break;
            default:
                break;
        }
    }

    public override void Back() {
        // you can't go back from a title screen
    }



}
