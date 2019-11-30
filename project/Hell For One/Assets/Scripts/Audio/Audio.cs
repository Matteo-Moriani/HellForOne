using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
    private AudioSource combatAudioSource;
    private AudioSource walkAudioSource;

    private Stats stats;

    private CombatEventsManager combatEventsManager;

    private void Awake()
    {
        combatEventsManager = GetComponent<CombatEventsManager>();    
    }

    private void OnEnable()
    {
        combatEventsManager.onSuccessfulHit += PlayHitClip;
        combatEventsManager.onBlockedHit += PlayBlockClip;
    }

    private void OnDisable()
    {
        combatEventsManager.onSuccessfulHit -= PlayHitClip;
        combatEventsManager.onBlockedHit -= PlayBlockClip;
    }

    private void Start()
    {
        stats = GetComponent<Stats>();

        combatAudioSource = gameObject.AddComponent<AudioSource>();
        AudioManager.Instance.SetAudioAudioSource(combatAudioSource, true, 5f, 500f, false);
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        AudioManager.Instance.SetAudioAudioSource(walkAudioSource, true, 5f, 500f, false);

        walkAudioSource.outputAudioMixerGroup = AudioManager.Instance.WalkAudioMixerGroup;
        combatAudioSource.outputAudioMixerGroup = AudioManager.Instance.CombatAudioMixerGroup;
    }

    private void Update()
    {
        AudioCycle();    
    }

    private void PlayHitClip() { 
        AudioManager.Instance.PlayRandomCombatAudioClip(AudioManager.CombatAudio.Hit,combatAudioSource);    
    }

    private void PlayBlockClip() { 
        AudioManager.Instance.PlayRandomCombatAudioClip(AudioManager.CombatAudio.Block,combatAudioSource);    
    }

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
}
