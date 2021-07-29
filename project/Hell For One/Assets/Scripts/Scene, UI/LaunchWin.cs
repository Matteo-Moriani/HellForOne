using FactoryBasedCombatSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using ArenaSystem;

public class LaunchWin : MonoBehaviour
{
    //public GameObject gameOverScreen;
    public GameObject musicPlayer;
    public GameObject videoPlayer;
    public GameObject videoClipScreen;
    public AudioClip winAudioClip;
    //public GameObject gameOverImage;
    public GameObject finalBoss;

    private void OnEnable()
    {
        Scene scene = SceneManager.GetActiveScene();

        if ( scene.name != "WinScene" )
        {
            finalBoss.GetComponent<ArenaBoss>().OnIgniImoragDeath += OnIgniImoragDeath;
        }
    }

    private void OnDisable()
    {
        Scene scene = SceneManager.GetActiveScene();

        if ( scene.name != "WinScene" )
        {
            finalBoss.GetComponent<ArenaBoss>().OnIgniImoragDeath -= OnIgniImoragDeath;
        }
    }

    //public IEnumerator SpriteFade( Image gameOverImage , float endValue , float duration )
    //{
    //    float elapsedTime = 0;
    //    float startValue = gameOverImage.color.a;
    //    while ( elapsedTime < duration )
    //    {
    //        elapsedTime += Time.deltaTime;
    //        float newAlpha = Mathf.Lerp( startValue , endValue , elapsedTime / duration );
    //        gameOverImage.color = new Color( gameOverImage.color.r , gameOverImage.color.g , gameOverImage.color.b , newAlpha );

    //        yield return null;
    //    }
    //}

    public void PlayWinClip()
    {
        musicPlayer.GetComponent<AudioSource>().clip = winAudioClip;
        musicPlayer.GetComponent<AudioSource>().Play();

        videoPlayer.GetComponent<VideoPlayer>().Play();
        videoClipScreen.GetComponent<RawImage>().enabled = true;

        //videoPlayer.GetComponent<VideoPlayer>().Play();

        //gameOverImage.GetComponent<Image>().enabled = true;

        //// Unity expects colors value in range [0, 1]
        //IEnumerator gameOverCR = SpriteFade( gameOverImage.GetComponent<Image>() , 1 , 0.7f );
        //StartCoroutine( gameOverCR );

        //m_sharedMaterial.SetFloat( ShaderUtilities.ID_FaceColor , 0.1f );
    }

    public void KillAllCoroutines()
    {
        MonoBehaviour[] scripts = FindObjectsOfType<MonoBehaviour>();

        foreach ( var item in scripts )
        {
            if (item.name != "LaunchWin")
                item.StopAllCoroutines();
        }
    }

    private IEnumerator LaunchWinScene()
    {
        yield return new WaitForSeconds( 5f );

        KillAllCoroutines();

        SceneManager.LoadScene( 2 );
    }

    public void OnIgniImoragDeath()
    {
        StartCoroutine( LaunchWinScene() );
    }

    //public void OnImpDeath( Transform transform )
    //{
    //    GameObject[] imps;

    //    // Se il player non si è mai reincarnato

    //    if ( GameObject.FindGameObjectWithTag( "Player" ) != null )
    //    {
    //        imps = GameObject.FindGameObjectsWithTag( "Demon" );

    //        if ( imps.Length <= 1 )
    //        {
    //            KillAllCoroutines();

    //            SceneManager.LoadScene( 1 );

    //            //PlayGameOverClip();

    //            //gameOverScreen.SetActive( true );
    //        }
    //    }

    //    // Se invece si è reincarnato almeno una volta

    //    else
    //    {
    //        imps = GameObject.FindGameObjectsWithTag( "Demon" );

    //        if ( imps.Length <= 2 )
    //        {
    //            KillAllCoroutines();

    //            SceneManager.LoadScene( 1 );

    //            //PlayGameOverClip();

    //            //gameOverScreen.SetActive( true );
    //        }
    //    }
    //}

    //private IEnumerator GameOverWin()
    //{
    //    // do stuff here, show win screen, etc.

    //    // just a simple time delay as an example
    //    //yield return new WaitForSeconds( 3.5f );

    //    //GameObject gameOverText = GameObject.Find( "GameOverText" );
    //    //gameOverText.GetComponent<TextMeshProUGUI>().enabled = true;

    //    //// wait for player to press space
    //    //yield return waitForKeyPress( KeyCode.Space ); // wait for this function to return

    //    //Debug.Log( "Ritorna" );

    //    //// do other stuff after key press
    //    //SceneManager.LoadScene( 0 );
    //}

    //private IEnumerator waitForKeyPress( KeyCode key )
    //{
    //    bool done = false;
    //    while ( !done ) // essentially a "while true", but with a bool to break out naturally
    //    {
    //        if ( Input.GetButtonDown( "XBoxA" ) )
    //        {
    //            done = true; // breaks the loop
    //        }
    //        yield return null; // wait until next frame, then continue execution from here (loop continues)
    //    }

    //    // now this function returns
    //}

    public void Start()
    {
        Scene scene = SceneManager.GetActiveScene();

        if ( scene.name == "WinScene" )
        {

            PlayWinClip();

            //gameOverScreen.SetActive( true );

            //StartCoroutine( GameOverWin() );
        }
    }
}

