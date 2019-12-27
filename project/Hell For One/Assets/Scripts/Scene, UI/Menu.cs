using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour {
    public GameObject[] elements;
    private GeneralInput input;
    public GeneralInput Input { get => input; set => input = value; }
    private int elementIndex = 0;
    public int ElementIndex { get => elementIndex; set => elementIndex = value; }
    private GameObject parentScreen;
    public GameObject ParentScreen { get => parentScreen; set => parentScreen = value; }

    private FPSLimiter FPSLimiter;

    public abstract void PressSelectedButton();

    public virtual void Back() {
        elementIndex = 0;
        foreach(GameObject button in elements) {
            button.GetComponent<Selectable>().image.color = elements[elementIndex].GetComponent<Selectable>().colors.normalColor;
        }
        TransitionTo(parentScreen);
    }

    private void Awake() {
        if(GameObject.FindGameObjectWithTag("Player"))
            input = GameObject.FindGameObjectWithTag("Player").GetComponent<GeneralInput>();
        else
            input = GameObject.FindGameObjectWithTag("MenuCanvas").GetComponent<GeneralInput>();
        elements[0].GetComponent<Selectable>().image.color = elements[0].GetComponent<Selectable>().colors.highlightedColor;
        FPSLimiter = gameObject.GetComponent<FPSLimiter>();
        if(!ParentScreen)
            ParentScreen = gameObject;
    }

    private void OnEnable() {
        if(!ParentScreen)
            ParentScreen = gameObject;
    }

    public void NextButton() {
        elements[elementIndex].GetComponent<Selectable>().image.color = elements[elementIndex].GetComponent<Selectable>().colors.normalColor;
        if(elementIndex == elements.Length - 1)
            elementIndex = 0;
        else
            elementIndex++;
        elements[elementIndex].GetComponent<Selectable>().image.color = elements[elementIndex].GetComponent<Selectable>().colors.highlightedColor;
    }

    public void PreviousButton() {
        elements[elementIndex].GetComponent<Selectable>().image.color = elements[elementIndex].GetComponent<Selectable>().colors.normalColor;
        if(elementIndex == 0)
            elementIndex = elements.Length - 1;
        else
            elementIndex--;
        elements[elementIndex].GetComponent<Selectable>().image.color = elements[elementIndex].GetComponent<Selectable>().colors.highlightedColor;
    }

    public void TransitionTo(GameObject nextMenu) {
        foreach(GameObject button in elements) {
            button.GetComponent<Selectable>().image.color = elements[elementIndex].GetComponent<Selectable>().colors.normalColor;
        }
        elementIndex = 0;
        input.CurrentScreen = nextMenu.GetComponent<Menu>();
        gameObject.SetActive(false);
        nextMenu.SetActive(true);
        nextMenu.GetComponent<Menu>().ParentScreen = gameObject;
        nextMenu.GetComponent<Menu>().elements[0].GetComponent<Selectable>().image.color = nextMenu.GetComponent<Menu>().elements[0].GetComponent<Selectable>().colors.highlightedColor;
    }
}
