using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamepadMessage : MonoBehaviour
{
    private GameObject[] hudObjects;
    private Image[] myImages;
    private TextMeshProUGUI[] myTexts;
    private TutorialManager tutorialManager;
    private bool startingWithoutController = false;

    private void Awake()
    {
        myImages = GetComponentsInChildren<Image>();
        myTexts = GetComponentsInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        hudObjects = GameObject.FindGameObjectsWithTag("HUD");
        tutorialManager = GameObject.FindGameObjectWithTag("TutorialScreens").GetComponent<TutorialManager>();

        HideMessage();

        if(Input.GetJoystickNames()[Input.GetJoystickNames().Length - 1] == "")
        {
            startingWithoutController = true;
            ShowMessage();
        }
        else
            tutorialManager.StartTutorials();

        StartCoroutine(CheckController());
    }

    private void ShowMessage()
    {
        foreach(GameObject g in hudObjects) { g.GetComponent<Canvas>().enabled = false; }
        foreach(Image i in myImages) { i.enabled = true; }
        foreach(TextMeshProUGUI t in myTexts) { t.enabled = true; }
    }

    private void HideMessage()
    {
        foreach(GameObject g in hudObjects) { g.GetComponent<Canvas>().enabled = true; }
        foreach(Image i in myImages) { i.enabled = false; }
        foreach(TextMeshProUGUI t in myTexts) { t.enabled = false; }
    }

    private IEnumerator CheckController()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);

            if(Input.GetJoystickNames()[Input.GetJoystickNames().Length - 1] == "")
                ShowMessage();
            else
            {
                HideMessage();
                if(startingWithoutController)
                {
                    startingWithoutController = false;
                    tutorialManager.StartTutorials();
                }
            }
        }
    }
}
