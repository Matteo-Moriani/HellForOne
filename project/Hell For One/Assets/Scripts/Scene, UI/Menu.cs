﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour {
    public GameObject[] buttons;
    private PlayerInput playerInput;
    public PlayerInput PlayerInput { get => playerInput; set => playerInput = value; }
    private int buttonIndex = 0;
    public int ButtonIndex { get => buttonIndex; set => buttonIndex = value; }
    private GameObject parentScreen;
    public GameObject ParentScreen { get => parentScreen; set => parentScreen = value; }

    private FPSLimiter FPSLimiter;

    public abstract void PressSelectedButton();

    public virtual void Back() {
        buttonIndex = 0;
        foreach(GameObject button in buttons) {
            button.GetComponent<Button>().image.color = buttons[buttonIndex].GetComponent<Button>().colors.normalColor;
        }
        TransitionTo(parentScreen);
    }

    private void Awake() {
        playerInput = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PlayerInput>();
        if(!playerInput)
            playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        buttons[0].GetComponent<Button>().image.color = buttons[0].GetComponent<Button>().colors.highlightedColor;
        FPSLimiter = gameObject.GetComponent<FPSLimiter>();
        if(!ParentScreen)
            ParentScreen = GameObject.FindGameObjectWithTag("RootScreen");
    }

    public void NextButton() {
        buttons[buttonIndex].GetComponent<Button>().image.color = buttons[buttonIndex].GetComponent<Button>().colors.normalColor;
        if(buttonIndex == buttons.Length - 1)
            buttonIndex = 0;
        else
            buttonIndex++;
        buttons[buttonIndex].GetComponent<Button>().image.color = buttons[buttonIndex].GetComponent<Button>().colors.highlightedColor;
    }

    public void PreviousButton() {
        buttons[buttonIndex].GetComponent<Button>().image.color = buttons[buttonIndex].GetComponent<Button>().colors.normalColor;
        if(buttonIndex == 0)
            buttonIndex = buttons.Length - 1;
        else
            buttonIndex--;
        buttons[buttonIndex].GetComponent<Button>().image.color = buttons[buttonIndex].GetComponent<Button>().colors.highlightedColor;
    }

    public void TransitionTo(GameObject nextMenu) {
        foreach(GameObject button in buttons) {
            button.GetComponent<Button>().image.color = buttons[buttonIndex].GetComponent<Button>().colors.normalColor;
        }
        buttonIndex = 0;
        playerInput.CurrentScreen = nextMenu;
        gameObject.SetActive(false);
        nextMenu.SetActive(true);
        nextMenu.GetComponent<Menu>().ParentScreen = gameObject;
        nextMenu.GetComponent<Menu>().buttons[0].GetComponent<Button>().image.color = nextMenu.GetComponent<Menu>().buttons[0].GetComponent<Button>().colors.highlightedColor;
    }
}