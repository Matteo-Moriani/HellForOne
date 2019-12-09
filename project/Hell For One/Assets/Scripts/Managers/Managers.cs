using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    public delegate void OnPressPlayButton();
    public event OnPressPlayButton onPressPlayButton;

    public void RaiseOnPressPlayButton() {
        if(onPressPlayButton != null)
            onPressPlayButton();
    }

    private static Managers instance;

    public static Managers Instance {
        get {
            return instance;
        }
    }
}
