using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : Menu {

    public override void PressSelectedButton() {

        switch(ButtonIndex) {
            // RETRY
            case 0:
                SceneManager.LoadScene("Demo");
                break;
            default:
                break;
        }
    }
}
