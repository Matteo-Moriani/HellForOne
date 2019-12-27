using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : Menu
{
    public GameObject controlsUI;
    public GameObject optionsUI;

    public override void PressSelectedButton() {

        switch(ElementIndex) {
            // RESUME
            case 0:
                Resume();
                break;
            // CONTROLS
            case 1:
                TransitionTo(controlsUI);
                break;
            // OPTIONS
            case 2:
                TransitionTo(optionsUI);
                break;
            // TITLE SCREEN
            case 3:
                Resume();
                SceneManager.LoadScene("Title Screen");
                break;
            default:
                break;
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        Input.gameObject.GetComponent<PlayerInput>().GameInPause = true;
        GameObject.FindGameObjectWithTag("Canvas").GetComponent<FPSLimiter>().IsPaused = true;
    }
    
    public void Resume()
    {
        Time.timeScale = 1f;
        Input.gameObject.GetComponent<PlayerInput>().GameInPause = false;
        Input.gameObject.GetComponent<PlayerInput>().NavigatingMenu = false;
        GameObject.FindGameObjectWithTag("Canvas").GetComponent<FPSLimiter>().IsPaused = false;
        gameObject.SetActive(false);
    }

    public override void Back() {
        Resume();
    }
}
