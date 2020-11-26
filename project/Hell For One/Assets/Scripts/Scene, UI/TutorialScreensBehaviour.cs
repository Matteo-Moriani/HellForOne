using System.Collections;
using UI;
using UnityEngine;

public class TutorialScreensBehaviour : MonoBehaviour
{
    public float fadeDurations = 0.5f;

    private CanvasGroup[] _tutorialScreens;
    private CanvasGroup _currentScreen;
    private bool _showingScreen = false;

    private void Awake()
    {
        _tutorialScreens = gameObject.GetComponentsInChildren<CanvasGroup>();

        foreach(CanvasGroup screen in _tutorialScreens)
        {
            screen.alpha = 0f;
        }
    }

    private void Update()
    {
        if(_showingScreen && InputManager.Instance.XButtonDown())
            HideCurrentScreen();
    }

    public void ShowScreen(string screenName)
    {
        // pause - to do after pause refactor

        CanvasGroup screenToShow = FindScreen(screenName);
        UiUtils.FadeIn(this, screenToShow, fadeDurations);
        _currentScreen = screenToShow;
        _showingScreen = true;
    }

    public void HideScreen(string screen)
    {
        if(screen == _currentScreen.gameObject.name)
        {
            UiUtils.FadeOut(this, _currentScreen, fadeDurations);
            _showingScreen = false;
        }

        // resume - to do after pause refactor
    }

    public void HideCurrentScreen()
    {
        UiUtils.FadeOut(this, _currentScreen, fadeDurations);
        _showingScreen = false;

        // resume - to do after pause refactor
    }

    private CanvasGroup FindScreen(string screenName)
    {
        foreach(CanvasGroup tutorialScreen in _tutorialScreens)
        {
            if(screenName == tutorialScreen.gameObject.name)
                return tutorialScreen;
        }

        Debug.Log("DIDN'T FIND ANY SCREEN NAMED " + screenName);
        return null;
    }
}
