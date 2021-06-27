using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : Menu {

    public override void PressSelectedButton() {

        //switch(ElementIndex) {
        //    // RETRY
        //    case 0:
        //        SceneManager.LoadScene("Demo");
        //        break;
        //    // TITLE SCREEN
        //    case 1:
        //        SceneManager.LoadScene("Title Screen");
        //        break;
        //    default:
        //        break;
        //}
    }

    public void Retry()
    {
        StartCoroutine(Reload());
    }

    private void Update()
    {
        if(UnityEngine.Input.GetButtonDown("XBoxA"))
            Retry();
    }
    private IEnumerator Reload()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Resources.UnloadUnusedAssets();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
