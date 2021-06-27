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
    public GameObject gameOverText;

    private void OnEnable()
    {
        ImpDeath.OnImpDeath += OnImpDeath;
    }

    private void OnDisable()
    {
        ImpDeath.OnImpDeath -= OnImpDeath;
    }

    public void PlayGameOverClip()
    {
        musicPlayer.GetComponent<AudioSource>().clip = gameOverAudioClip;
        musicPlayer.GetComponent<AudioSource>().Play();

        videoClipScreen.GetComponent<RawImage>().enabled = true;

        videoPlayer.GetComponent<VideoPlayer>().Play();

        gameOverText.SetActive( true );

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
