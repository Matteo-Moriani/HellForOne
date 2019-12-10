using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreensManager : MonoBehaviour
{
    public GameObject gameOverScreen;

    public void ActivateGameOver() {
        gameObject.SetActive(true);
    }
}
