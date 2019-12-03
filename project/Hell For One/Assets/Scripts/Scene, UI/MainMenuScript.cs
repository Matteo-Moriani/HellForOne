using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void Play() {
        // better
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("Complete");
    }

    public void Quit() {
        Application.Quit();
        Debug.Log("IN A BUILD THIS WOULD HAVE QUIT THE GAME");
    }
}
