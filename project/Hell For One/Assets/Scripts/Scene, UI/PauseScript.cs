using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum MenuType {
    pause,
    options
}

public class PauseScript : MonoBehaviour
{
    private FPSLimiter FPSLimiter;
    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject optionsUI;
    public GameObject[] pauseButtons = new GameObject[3];
    public GameObject[] optionsButtons = new GameObject[1];
    public int pauseIndex = 0;
    public int optionsIndex = 0;
    private PlayerInput playerInput;
    private MenuType currentMenu = MenuType.pause;
    public MenuType CurrentMenu { get => currentMenu; set => currentMenu = value; }

    private void Awake() {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        pauseButtons[pauseIndex].GetComponent<Button>().image.color = pauseButtons[pauseIndex].GetComponent<Button>().colors.highlightedColor;
        FPSLimiter = gameObject.GetComponent<FPSLimiter>();
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        playerInput.GameInPause = false;
        FPSLimiter.IsPaused = false;
    }

    public void Freeze() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
        FPSLimiter.IsPaused = true;
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
    
    public void NextButton() {

        if(currentMenu == MenuType.pause) {
            pauseButtons[pauseIndex].GetComponent<Button>().image.color = pauseButtons[pauseIndex].GetComponent<Button>().colors.normalColor;
            if(pauseIndex == pauseButtons.Length - 1)
                pauseIndex = 0;
            else
                pauseIndex++;
            pauseButtons[pauseIndex].GetComponent<Button>().image.color = pauseButtons[pauseIndex].GetComponent<Button>().colors.highlightedColor;
        }
        else if(currentMenu == MenuType.options) {
            optionsButtons[optionsIndex].GetComponent<Button>().image.color = optionsButtons[optionsIndex].GetComponent<Button>().colors.normalColor;
            if(optionsIndex == optionsButtons.Length - 1)
                optionsIndex = 0;
            else
                optionsIndex++;
            optionsButtons[optionsIndex].GetComponent<Button>().image.color = optionsButtons[optionsIndex].GetComponent<Button>().colors.highlightedColor;
        }

    }
    
    public void PreviousButton() {
        if(currentMenu == MenuType.pause) {
            pauseButtons[pauseIndex].GetComponent<Button>().image.color = pauseButtons[pauseIndex].GetComponent<Button>().colors.normalColor;
            if(pauseIndex == 0)
                pauseIndex = pauseButtons.Length - 1;
            else
                pauseIndex--;
            pauseButtons[pauseIndex].GetComponent<Button>().image.color = pauseButtons[pauseIndex].GetComponent<Button>().colors.highlightedColor;
        }
        else if(currentMenu == MenuType.options) {
            optionsButtons[optionsIndex].GetComponent<Button>().image.color = optionsButtons[optionsIndex].GetComponent<Button>().colors.normalColor;
            if(optionsIndex == 0)
                optionsIndex = optionsButtons.Length - 1;
            else
                optionsIndex--;
            optionsButtons[optionsIndex].GetComponent<Button>().image.color = optionsButtons[optionsIndex].GetComponent<Button>().colors.highlightedColor;
        }

        
        
    }

    // TODO - must be called with X button
    public void PressSelectedButton() {

        if(currentMenu == MenuType.pause) {
            switch(pauseIndex) {
                case 0:
                    Resume();
                    break;
                case 1:
                    Options();
                    break;
                case 2:
                    TitleScreen();
                    break;
                default:
                    break;
            }
        }
        else if(currentMenu == MenuType.options) {
            switch(optionsIndex) {
                case 0:
                    Back();
                    break;
                default:
                    break;
            }
        }
        
    }

    private void Options() {
        foreach(GameObject button in pauseButtons) {
            button.GetComponent<Button>().image.color = pauseButtons[pauseIndex].GetComponent<Button>().colors.normalColor;
        }
        currentMenu = MenuType.options;
        pauseIndex = 0;
        pauseMenuUI.SetActive(false);
        optionsUI.SetActive(true);
        optionsButtons[optionsIndex].GetComponent<Button>().image.color = pauseButtons[pauseIndex].GetComponent<Button>().colors.highlightedColor;
    }

    public void Back() {
        foreach(GameObject button in optionsButtons) {
            button.GetComponent<Button>().image.color = pauseButtons[pauseIndex].GetComponent<Button>().colors.normalColor;
        }
        currentMenu = MenuType.pause;
        optionsIndex = 0;
        optionsUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        pauseButtons[0].GetComponent<Button>().image.color = pauseButtons[0].GetComponent<Button>().colors.highlightedColor;
    }
}
