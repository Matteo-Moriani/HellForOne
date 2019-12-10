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
    }

    private void OnDisable()
    {
        BattleEventsManager.onBattleExit -= StartOutOfCombatMusic;
        BattleEventsManager.onBossBattleExit -= StartOutOfCombatMusic;

        BattleEventsManager.onBattleEnter -= StartCombatMusic;
        BattleEventsManager.onBossBattleEnter -= StartBossMusic;
    }

    //private void Start()
    //{
        
    //}

    // Start is called before the first frame update
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
        yield return new WaitForSeconds(AudioManager.Instance.PlayMusic(musicAudiosource,AudioManager.Music.OutOFCombat));
        AudioManager.Instance.PlayMusicLoop(musicAudiosource,AudioManager.Music.OutOFCombat);
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
        yield return new WaitForSeconds(AudioManager.Instance.PlayMusic(musicAudiosource, AudioManager.Music.Boss));
        AudioManager.Instance.PlayMusicLoop(musicAudiosource, AudioManager.Music.Boss);
    }
}
