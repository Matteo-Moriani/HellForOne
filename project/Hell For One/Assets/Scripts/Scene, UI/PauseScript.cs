using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Freeze() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void TitleScreen() {
        Resume();
        SceneManager.LoadScene("Title Screen");
    }

    public void OnPause() {
        
        // TODO - manage pause with events

    }

    public void Pause() {
        if(gameIsPaused)
            Resume();
        else
            Freeze();
    }

    private void Update() {

        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(gameIsPaused)
                Resume();
            else
                Freeze();
        }

    }
}
