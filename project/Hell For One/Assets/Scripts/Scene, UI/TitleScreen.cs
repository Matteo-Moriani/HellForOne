using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Screen {
    title,
    options,
    credits
}

public class TitleScreen : MonoBehaviour {
    private FPSLimiter FPSLimiter;
    public GameObject titleScreenUI;
    public GameObject optionsUI;
    public GameObject creditsUI;
    public GameObject[] titleButtons = new GameObject[4];
    public GameObject[] optionsButtons = new GameObject[1];
    public GameObject[] creditsButtons = new GameObject[1];
    public int titleIndex = 0;
    public int optionsIndex = 0;
    public int creditsIndex = 0;
    private PlayerInput playerInput;
    private Screen currentScreen = Screen.title;
    public Screen CurrentScreen { get => currentScreen; set => currentScreen = value; }

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        titleButtons[titleIndex].GetComponent<Button>().image.color = titleButtons[titleIndex].GetComponent<Button>().colors.highlightedColor;
        FPSLimiter = gameObject.GetComponent<FPSLimiter>();
    }

    public void NextButton() {

        if(currentScreen == Screen.title) {
            titleButtons[titleIndex].GetComponent<Button>().image.color = titleButtons[titleIndex].GetComponent<Button>().colors.normalColor;
            if(titleIndex == titleButtons.Length - 1)
                titleIndex = 0;
            else
                titleIndex++;
            titleButtons[titleIndex].GetComponent<Button>().image.color = titleButtons[titleIndex].GetComponent<Button>().colors.highlightedColor;
        }
        else if(currentScreen == Screen.options) {
            optionsButtons[optionsIndex].GetComponent<Button>().image.color = optionsButtons[optionsIndex].GetComponent<Button>().colors.normalColor;
            if(optionsIndex == optionsButtons.Length - 1)
                optionsIndex = 0;
            else
                optionsIndex++;
            optionsButtons[optionsIndex].GetComponent<Button>().image.color = optionsButtons[optionsIndex].GetComponent<Button>().colors.highlightedColor;
        }
        else if(currentScreen == Screen.credits) {
            creditsButtons[creditsIndex].GetComponent<Button>().image.color = creditsButtons[creditsIndex].GetComponent<Button>().colors.normalColor;
            if(creditsIndex == creditsButtons.Length - 1)
                creditsIndex = 0;
            else
                creditsIndex++;
            creditsButtons[creditsIndex].GetComponent<Button>().image.color = creditsButtons[creditsIndex].GetComponent<Button>().colors.highlightedColor;
        }

    }

    public void PreviousButton() {
        if(currentScreen == Screen.title) {
            titleButtons[titleIndex].GetComponent<Button>().image.color = titleButtons[titleIndex].GetComponent<Button>().colors.normalColor;
            if(titleIndex == 0)
                titleIndex = titleButtons.Length - 1;
            else
                titleIndex--;
            titleButtons[titleIndex].GetComponent<Button>().image.color = titleButtons[titleIndex].GetComponent<Button>().colors.highlightedColor;
        }
        else if(currentScreen == Screen.options) {
            optionsButtons[optionsIndex].GetComponent<Button>().image.color = optionsButtons[optionsIndex].GetComponent<Button>().colors.normalColor;
            if(optionsIndex == 0)
                optionsIndex = optionsButtons.Length - 1;
            else
                optionsIndex--;
            optionsButtons[optionsIndex].GetComponent<Button>().image.color = optionsButtons[optionsIndex].GetComponent<Button>().colors.highlightedColor;
        }
        else if(currentScreen == Screen.credits) {
            creditsButtons[creditsIndex].GetComponent<Button>().image.color = creditsButtons[creditsIndex].GetComponent<Button>().colors.normalColor;
            if(creditsIndex == 0)
                creditsIndex = creditsButtons.Length - 1;
            else
                creditsIndex--;
            creditsButtons[creditsIndex].GetComponent<Button>().image.color = creditsButtons[creditsIndex].GetComponent<Button>().colors.highlightedColor;
        }
        
    }
    
    public void PressSelectedButton() {

        if(currentScreen == Screen.title) {
            switch(titleIndex) {
                case 0:
                    playerInput.InTitleScreen = false;
                    SceneManager.LoadScene("Game");
                    break;
                case 1:
                    Options();
                    break;
                case 2:
                    Credits();
                    break;
                case 3:
                    // TODO - quit
                    Debug.Log("Quit!");
                    break;
                default:
                    break;
            }
        }
        else if(currentScreen == Screen.options) {
            switch(optionsIndex) {
                case 0:
                    Back();
                    break;
                default:
                    break;
            }
        }
        else if(currentScreen == Screen.credits) {
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
        foreach(GameObject button in titleButtons) {
            button.GetComponent<Button>().image.color = titleButtons[titleIndex].GetComponent<Button>().colors.normalColor;
        }
        currentScreen = Screen.options;
        titleIndex = 0;
        titleScreenUI.SetActive(false);
        optionsUI.SetActive(true);
        optionsButtons[optionsIndex].GetComponent<Button>().image.color = optionsButtons[optionsIndex].GetComponent<Button>().colors.highlightedColor;
    }

    private void Credits() {
        foreach(GameObject button in titleButtons) {
            button.GetComponent<Button>().image.color = titleButtons[titleIndex].GetComponent<Button>().colors.normalColor;
        }
        currentScreen = Screen.credits;
        titleIndex = 0;
        titleScreenUI.SetActive(false);
        creditsUI.SetActive(true);
        creditsButtons[creditsIndex].GetComponent<Button>().image.color = creditsButtons[creditsIndex].GetComponent<Button>().colors.highlightedColor;
    }

    public void Back() {
        switch(currentScreen) {
            case Screen.options:
                optionsIndex = 0;
                foreach(GameObject button in optionsButtons) {
                    button.GetComponent<Button>().image.color = optionsButtons[optionsIndex].GetComponent<Button>().colors.normalColor;
                }
                break;
            case Screen.credits:
                creditsIndex = 0;
                foreach(GameObject button in creditsButtons) {
                    button.GetComponent<Button>().image.color = creditsButtons[creditsIndex].GetComponent<Button>().colors.normalColor;
                }
                break;
        }
        currentScreen = Screen.title;
        optionsUI.SetActive(false);
        creditsUI.SetActive(false);
        titleScreenUI.SetActive(true);
        titleButtons[0].GetComponent<Button>().image.color = titleButtons[0].GetComponent<Button>().colors.highlightedColor;
    }
}
