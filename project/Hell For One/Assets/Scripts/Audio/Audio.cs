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

    private void Start()
    {
        stats = GetComponent<Stats>();

        combatAudioSource = gameObject.AddComponent<AudioSource>();
        AudioManager.SetAudioAudioSource(combatAudioSource, true, 5f, 500f, false);
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        AudioManager.SetAudioAudioSource(walkAudioSource, true, 5f, 500f, false);

        walkAudioSource.outputAudioMixerGroup = AudioManager.WalkAudioMixerGroup;
        combatAudioSource.outputAudioMixerGroup = AudioManager.CombatAudioMixerGroup;
    }

    private void Update()
    {
        AudioCycle();    
    }

    public void PlayRandomCombatAudioClip(AudioManager.CombatAudio type) { 
        AudioManager.PlayRandomCombatAudioClip(type, combatAudioSource);    
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
            AudioManager.PlayRandomWalkClip(AudioManager.Size.Small,walkAudioSource);
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
                AudioManager.PlayRandomWalkClip(AudioManager.Size.Small, walkAudioSource);
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
