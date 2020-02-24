using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    private Image image;
    public float entryTime = 0;
    public float exitTime = 2;
    public bool isLastImage = false;

    private void Awake() {
        image = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Display());
    }

    private IEnumerator Display() {
        yield return new WaitForSeconds(entryTime);
        Debug.Log(gameObject.name + "fade in");
        Color c = image.color;
        c.a = 1;
        image.color = c;
        yield return new WaitForSeconds(exitTime - entryTime);
        Debug.Log(gameObject.name + "fade out");
        c.a = 0;
        image.color = c;
        if(isLastImage)
            SceneManager.LoadScene("Title Screen");
    }
}
