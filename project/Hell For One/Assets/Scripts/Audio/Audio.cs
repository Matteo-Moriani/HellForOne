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

    private Stats stats;

    private CombatEventsManager combatEventsManager;

    private Coroutine walkCR;

    private void Awake()
    {
        combatEventsManager = GetComponent<CombatEventsManager>();    
    }

    private void OnEnable()
    {
        combatEventsManager.onBeenHit += PlayHitClip;
        combatEventsManager.onBlockedHit += PlayBlockClip;
        combatEventsManager.onStartRunning += PlayFootStep;
        combatEventsManager.onStartIdle += StopFootStep;
        combatEventsManager.onDeath += PlayDeathSound;
    }

    private void OnDisable()
    {
        combatEventsManager.onBeenHit -= PlayHitClip;
        combatEventsManager.onBlockedHit -= PlayBlockClip;
        combatEventsManager.onStartRunning -= PlayFootStep;
        combatEventsManager.onStartIdle -= StopFootStep;
        combatEventsManager.onDeath -= PlayDeathSound;
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

        walkAudioSource.outputAudioMixerGroup = AudioManager.Instance.WalkAudioMixerGroup;
        combatAudioSource.outputAudioMixerGroup = AudioManager.Instance.CombatAudioMixerGroup;
        deathAudioSource.outputAudioMixerGroup = AudioManager.Instance.DeathMixedGroup;
    }

    private void Update()
    {
        //AudioCycle();    
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
        yield return new WaitForSeconds(AudioManager.Instance.PlayRandomWalkClip(AudioManager.Size.Small, walkAudioSource));    
    }

    /*
    private void AudioCycle()
    {
        if (stats != null)
        {
            if (stats.type == Stats.Type.Player)
            {
                ManagePlayerBaseAudio();
            }
            if (stats.type == Stats.Type.Ally)
            {
                ManageAllyBaseAudio();
            }
        }
        else
        {
            stats = GetComponent<Stats>();
        }
    }

    private void ManagePlayerBaseAudio()
    {
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            // TODO - Implement right size
            AudioManager.Instance.PlayRandomWalkClip(AudioManager.Size.Small,walkAudioSource);
        }
        else
        {
            walkAudioSource.Stop();
        }
    }

    private void ManageAllyBaseAudio()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            if (agent.velocity != Vector3.zero)
            {
                AudioManager.Instance.PlayRandomWalkClip(AudioManager.Size.Small, walkAudioSource);
            }
            else
            {
                walkAudioSource.Stop();
            }

        }
        else
        {
            Debug.Log(this.transform.root.gameObject.name + " is an ally but has no NavMeshAgent attached, cannot play Base Audio");
        }
    }
    */
}
