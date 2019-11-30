using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource musicAudiosource;

    private bool isOutOfCOmbat = false;
    private bool isInCombat = false;
    private bool isFightingBoss = false;

    // Start is called before the first frame update
    void Start()
    {
        musicAudiosource = this.gameObject.AddComponent<AudioSource>();
        AudioManager.Instance.SetAudioAudioSource(musicAudiosource,false,100f,100f,false);

        // Used for testing
        isFightingBoss = true;
    }

    private void Update()
    {
        if (isFightingBoss) { 
            AudioManager.Instance.PlayMusic(musicAudiosource,AudioManager.Music.Boss);        
        }
    }


}
