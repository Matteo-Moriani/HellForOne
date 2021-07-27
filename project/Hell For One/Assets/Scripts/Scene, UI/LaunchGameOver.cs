using FactoryBasedCombatSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LaunchGameOver : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject musicPlayer;
    public GameObject videoPlayer;
    public GameObject videoClipScreen;
    public AudioClip gameOverAudioClip;
    public GameObject gameOverImage;

    private void OnEnable()
    {
        ImpDeath.OnImpDeath += OnImpDeath;
    }

    private void OnDisable()
    {
        ImpDeath.OnImpDeath -= OnImpDeath;
    }

    public IEnumerator SpriteFade( Image gameOverImage, float endValue, float duration )
    {
        float elapsedTime = 0;
        float startValue = gameOverImage.color.a;
        while ( elapsedTime < duration )
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp( startValue , endValue , elapsedTime / duration );
            gameOverImage.color = new Color( gameOverImage.color.r , gameOverImage.color.g , gameOverImage.color.b , newAlpha );

            yield return null;
        }
    }

    public void PlayGameOverClip()
    {
        musicPlayer.GetComponent<AudioSource>().clip = gameOverAudioClip;
        musicPlayer.GetComponent<AudioSource>().Play();

        videoPlayer.GetComponent<VideoPlayer>().Play();
        videoClipScreen.GetComponent<RawImage>().enabled = true;

        //videoPlayer.GetComponent<VideoPlayer>().Play();

        gameOverImage.GetComponent<Image>().enabled = true;

        // Unity expects colors value in range [0, 1]
        IEnumerator gameOverCR = SpriteFade( gameOverImage.GetComponent<Image>() , 1 , 0.7f );
        StartCoroutine( gameOverCR );

        //m_sharedMaterial.SetFloat( ShaderUtilities.ID_FaceColor , 0.1f );
    }

    public void OnImpDeath( Transform transform )
    {
        GameObject[] imps;

        // Se il player non si è mai reincarnato

        if ( GameObject.FindGameObjectWithTag( "Player" ) != null )
        {
            imps = GameObject.FindGameObjectsWithTag( "Demon" );

            if ( imps.Length <= 1 )
            {
                PlayGameOverClip();

                gameOverScreen.SetActive( true );
            }
        }

        // Se invece si è reincarnato almeno una volta

        else
        {
            imps = GameObject.FindGameObjectsWithTag( "Demon" );

            if ( imps.Length <= 2 )
            {
                PlayGameOverClip();

                gameOverScreen.SetActive( true );
            }
        }
    }
}
