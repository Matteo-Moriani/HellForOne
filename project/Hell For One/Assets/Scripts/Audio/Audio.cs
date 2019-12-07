using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
    private AudioSource combatAudioSource;
    private AudioSource walkAudioSource;
    private AudioSource deathAudioSource;
    private AudioSource dashAudioSource;

    private Stats stats;

    private CombatEventsManager combatEventsManager;

    private Coroutine walkCR;

    private Coroutine dashCr;

    private void Awake()
    {
        combatEventsManager = GetComponent<CombatEventsManager>();    
    }

    private void OnEnable()
    {
        combatEventsManager.onBeenHit += PlayHitClip;
        combatEventsManager.onBlockedHit += PlayBlockClip;
        combatEventsManager.onStartMoving += PlayFootStep;
        combatEventsManager.onStartIdle += StopFootStep;
        combatEventsManager.onDeath += PlayDeathSound;
        combatEventsManager.onStartDash += PlayDashClip; 
    }

    private void OnDisable()
    {
        combatEventsManager.onBeenHit -= PlayHitClip;
        combatEventsManager.onBlockedHit -= PlayBlockClip;
        combatEventsManager.onStartMoving -= PlayFootStep;
        combatEventsManager.onStartIdle -= StopFootStep;
        combatEventsManager.onDeath -= PlayDeathSound;
        combatEventsManager.onStartDash -= PlayDashClip;
    }

    private void Start()
    {
        stats = GetComponent<Stats>();

        combatAudioSource = gameObject.AddComponent<AudioSource>();
        AudioManager.Instance.SetAudioAudioSource(combatAudioSource, true, 5f, 500f, false);
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        AudioManager.Instance.SetAudioAudioSource(walkAudioSource, true, 5f, 500f, false);
        deathAudioSource = gameObject.AddComponent<AudioSource>();
        AudioManager.Instance.SetAudioAudioSource(deathAudioSource,true,5f,500f,false);
        dashAudioSource = gameObject.AddComponent<AudioSource>();
        AudioManager.Instance.SetAudioAudioSource(dashAudioSource,true,5f,500f,false);

        walkAudioSource.outputAudioMixerGroup = AudioManager.Instance.WalkAudioMixerGroup;
        combatAudioSource.outputAudioMixerGroup = AudioManager.Instance.CombatAudioMixerGroup;
        deathAudioSource.outputAudioMixerGroup = AudioManager.Instance.DeathMixerGroup;
        dashAudioSource.outputAudioMixerGroup = AudioManager.Instance.DashMixerGroup;
    }

    private void PlayDashClip() { 
        if(dashCr == null) { 
            dashCr = StartCoroutine(dashCoroutine());        
        }    
    }

    private void StopDashClip() { 
        if(dashCr != null){ 
            StopCoroutine(dashCr);
            dashCr = null;
        }    
    }

    private void PlayHitClip() { 
        AudioManager.Instance.PlayRandomCombatAudioClip(AudioManager.CombatAudio.Hit,combatAudioSource);    
    }

    private void PlayBlockClip() { 
        AudioManager.Instance.PlayRandomCombatAudioClip(AudioManager.CombatAudio.Block,combatAudioSource);    
    }

    private void PlayDeathSound() { 
        AudioManager.Instance.PlayDeathSound(stats.type,deathAudioSource);    
    }

    private void PlayFootStep() { 
        if(walkCR == null) { 
            walkCR = StartCoroutine(walkCoroutine());    
        }   
    }

    private void StopFootStep() { 
        if(walkCR != null) { 
            StopCoroutine(walkCR);
            walkCR = null;
            walkAudioSource.Stop();
        }    
    }

    private IEnumerator walkCoroutine() {
        while (true) {
            yield return new WaitForSeconds(AudioManager.Instance.PlayRandomWalkClip(AudioManager.Size.Small, walkAudioSource));// + Random.Range(0,0.01f));
        }    
    }

    private IEnumerator dashCoroutine() { 
        AudioManager.Instance.LockWalkAudio();
        yield return new WaitForSeconds(AudioManager.Instance.PlayRandomDashClip(dashAudioSource));
        AudioManager.Instance.UnlockWalkAudio();
        StopDashClip();
    }
}
