using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource musicAudiosource;

    private bool isOutOfCOmbat = false;
    private bool isInCombat = false;
    private bool isFightingBoss = false;

    private Coroutine startOutOfCombatMusicCR;
    private Coroutine startCombatMusicCR;
    private Coroutine startBossMusicCR;

    // TODO - lerp volume when changing music.

    private void OnEnable()
    {
        BattleEventsManager.onBattleExit += StartOutOfCombatMusic;
        BattleEventsManager.onBossBattleExit += StartOutOfCombatMusic;

        BattleEventsManager.onBattleEnter += StartCombatMusic;
        BattleEventsManager.onBossBattleEnter += StartBossMusic;

        GameEvents.onPause += PauseMusic;
        GameEvents.onResume += ResumeMusic;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBattleExit -= StartOutOfCombatMusic;
        BattleEventsManager.onBossBattleExit -= StartOutOfCombatMusic;

        BattleEventsManager.onBattleEnter -= StartCombatMusic;
        BattleEventsManager.onBossBattleEnter -= StartBossMusic;

        GameEvents.onPause -= PauseMusic;
        GameEvents.onResume -= ResumeMusic;
    }

    void Start()
    {
        musicAudiosource = this.gameObject.GetComponent<AudioSource>();
        AudioManager.Instance.SetAudioAudioSource(musicAudiosource, false, 100f, 100f, false);
        musicAudiosource.outputAudioMixerGroup = AudioManager.Instance.MusicMixerGroup;
        StartOutOfCombatMusic();
    }

    private void StartOutOfCombatMusic() {
        if (!isOutOfCOmbat) {
            if (startOutOfCombatMusicCR == null)
            {
                StopAllCoroutines();
                startCombatMusicCR = null;
                startBossMusicCR = null;

                //musicAudiosource.Stop();
                startOutOfCombatMusicCR = StartCoroutine(StartOutOFCombatMusicCoroutine());
                isOutOfCOmbat = true;
                isInCombat = false;
                isFightingBoss = false;
            }
        }  
    }

    private IEnumerator StartOutOFCombatMusicCoroutine() {
        // Play the main out of combat music
        AudioManager.Instance.PlayMusic(musicAudiosource, AudioManager.Music.OutOFCombat);
        
        //We wait the end of the main out of combat music
        bool needToPlayLoop = true;
        
        while (needToPlayLoop) {
            // If we done playing main out of combat music...
            if (!musicAudiosource.isPlaying)
            {
                // ...we play the loop out of combat music...
                AudioManager.Instance.PlayMusicLoop(musicAudiosource, AudioManager.Music.OutOFCombat);
                needToPlayLoop = false;
            }
            // ... else, we'll chek next frame
            yield return null;
        }
    }

    private void StartCombatMusic() {
        if (!isInCombat)
        {
            if (startCombatMusicCR == null)
            {
                StopAllCoroutines();
                startOutOfCombatMusicCR = null;
                startBossMusicCR = null;
                    
                //musicAudiosource.Stop();
                startCombatMusicCR = StartCoroutine(StartCombatMusicCoroutine());
                isOutOfCOmbat = false;
                isInCombat = true;
                isFightingBoss = false;
            }
        }
    }

    private IEnumerator StartCombatMusicCoroutine() {
        yield return new WaitForSeconds(AudioManager.Instance.PlayMusic(musicAudiosource, AudioManager.Music.Combat));
        AudioManager.Instance.PlayMusicLoop(musicAudiosource, AudioManager.Music.Combat);
    }

    private void StartBossMusic() {
        if (!isFightingBoss)
        {
            if (startBossMusicCR == null)
            {
                StopAllCoroutines();
                startOutOfCombatMusicCR = null;
                startCombatMusicCR = null;

                //musicAudiosource.Stop();
                startBossMusicCR = StartCoroutine(StartBossMusicCoroutine());
                isOutOfCOmbat = false;
                isInCombat = false;
                isFightingBoss = true;
            }
        }
    }

    private IEnumerator StartBossMusicCoroutine() {
        if (LevelManager.IsMidBossAlive) {
            yield return new WaitForSeconds(AudioManager.Instance.PlayMusic(musicAudiosource, AudioManager.Music.Combat));
            AudioManager.Instance.PlayMusicLoop(musicAudiosource, AudioManager.Music.Combat);
        }
        if (LevelManager.IsBossAlive) {
            yield return new WaitForSeconds(AudioManager.Instance.PlayMusic(musicAudiosource, AudioManager.Music.Boss));
            AudioManager.Instance.PlayMusicLoop(musicAudiosource, AudioManager.Music.Boss);
        } 
    }

    private void PauseMusic() { 
        musicAudiosource.Pause();    
    }

    private void ResumeMusic() { 
        musicAudiosource.UnPause();    
    }
}
