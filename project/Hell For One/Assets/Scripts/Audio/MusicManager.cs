using System.Collections;
using System.Collections.Generic;
using ArenaSystem;
using Managers;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private bool activateSoundtrack = false;

    private AudioSource musicAudiosource;

    private bool isOutOfCOmbat = false;
    private bool isInCombat = false;
    private bool isFightingBoss = false;
    private bool isGamePaused = false;

    private Coroutine startOutOfCombatMusicCR;
    private Coroutine startCombatMusicCR;
    private Coroutine startBossMusicCR;

    // TODO - lerp volume when changing music.

    private void OnEnable()
    {
        ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
    }

    private void OnDisable()
    {
        ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;
        ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
    }

    private void Awake()
    {
        if(!activateSoundtrack)
            this.enabled = false;
    }

    void Start()
    {
        musicAudiosource = gameObject.GetComponent<AudioSource>();
        AudioManager.Instance.SetAudioAudioSource(musicAudiosource, false, 100f, 100f, false);
        musicAudiosource.outputAudioMixerGroup = AudioManager.Instance.MusicMixerGroup;
        StartOutOfCombatMusic();
    }

    private void OnGlobalEndBattle(ArenaManager arenaManager) => StartOutOfCombatMusic();

    private void OnGlobalStartBattle(ArenaManager arenaManager) => StartBossMusic();
    
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
            if (!musicAudiosource.isPlaying && !isGamePaused)
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
        // Play the main out of combat music
        AudioManager.Instance.PlayMusic(musicAudiosource, AudioManager.Music.Combat);

        //We wait the end of the main out of combat music
        bool needToPlayLoop = true;

        while (needToPlayLoop)
        {
            // If we done playing main out of combat music...
            if (!musicAudiosource.isPlaying && !isGamePaused)
            {
                // ...we play the loop out of combat music...
                AudioManager.Instance.PlayMusicLoop(musicAudiosource, AudioManager.Music.Combat);
                needToPlayLoop = false;
            }
            // ... else, we'll chek next frame
            yield return null;
        }
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
        // if (LevelManager.IsMidBossAlive) {
        //     // Play the main out of combat music
        //     AudioManager.Instance.PlayMusic(musicAudiosource, AudioManager.Music.Combat);
        //
        //     //We wait the end of the main out of combat music
        //     bool needToPlayLoop = true;
        //
        //     while (needToPlayLoop)
        //     {
        //         // If we done playing main out of combat music...
        //         if (!musicAudiosource.isPlaying && !isGamePaused)
        //         {
        //             // ...we play the loop out of combat music...
        //             AudioManager.Instance.PlayMusicLoop(musicAudiosource, AudioManager.Music.Combat);
        //             needToPlayLoop = false;
        //         }
        //         // ... else, we'll chek next frame
        //         yield return null;
        //     }
        // }
        // if (LevelManager.IsBossAlive) {
        //     // Play the main out of combat music
        //     AudioManager.Instance.PlayMusic(musicAudiosource, AudioManager.Music.Boss);
        //
        //     //We wait the end of the main out of combat music
        //     bool needToPlayLoop = true;
        //
        //     while (needToPlayLoop)
        //     {
        //         // If we done playing main out of combat music...
        //         if (!musicAudiosource.isPlaying && !isGamePaused)
        //         {
        //             // ...we play the loop out of combat music...
        //             AudioManager.Instance.PlayMusicLoop(musicAudiosource, AudioManager.Music.Boss);
        //             needToPlayLoop = false;
        //         }
        //         // ... else, we'll chek next frame
        //         yield return null;
        //     }
        // } 

        yield return null;
    }

    private void PauseMusic() {
        isGamePaused = true;
        musicAudiosource.Pause();    
    }

    private void ResumeMusic() {
        isGamePaused = false;
        musicAudiosource.UnPause();    
    }
}
