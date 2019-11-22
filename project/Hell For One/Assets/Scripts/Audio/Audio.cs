using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
    public enum CombatAudio { 
        Hit,
        Block,
        Death
    }

    public enum BaseAudio { 
        Walk    
    }

    [SerializeField]
    private AudioMixerGroup baseAudioMixerGroup;
    [SerializeField]
    private AudioMixerGroup combatAudioMixerGroup;

    private AudioSource combatAudioSource;
    private AudioSource baseAudioSource;

    private Stats stats;

    [SerializeField]
    [Tooltip("Clips that will be played when this unit is hit")]
    private AudioClip[] hitClips;

    [SerializeField]
    [Tooltip("Clips that will be played when this unit blocks")]
    private AudioClip[] blockClips;

    [SerializeField]
    [Tooltip("Clips that will be played when this unit dies")]
    private AudioClip[] deathClips;

    [SerializeField]
    [Tooltip("Clip that will be played when this unit walks")]
    private AudioClip walkClip;

    private void Start()
    {
        stats = GetComponent<Stats>();
        
        combatAudioSource = gameObject.AddComponent<AudioSource>();
        SetAudioAudioSource(combatAudioSource,true,5f,500f,false);
        baseAudioSource = gameObject.AddComponent<AudioSource>();
        SetAudioAudioSource(baseAudioSource,true,5f,500f,false);
        if(baseAudioMixerGroup != null) {
            baseAudioSource.outputAudioMixerGroup = baseAudioMixerGroup;
        }
        if(combatAudioMixerGroup != null) { 
            combatAudioSource.outputAudioMixerGroup = combatAudioMixerGroup;    
        }
    }

    private void Update()
    {
        AudioCycle();    
    }

    public void PlayRandomCombatAudioClip(CombatAudio type) {
        switch (type) { 
            case CombatAudio.Hit:
                    AudioClip clipToPlay = hitClips[(Random.Range(0,hitClips.Length - 1))];
                    if(clipToPlay != null && combatAudioSource != null) { 
                        combatAudioSource.clip = clipToPlay;
                        combatAudioSource.Play();
                }
                else { 
                    Debug.Log(name + " CombatAudio: AudioSource is null or ClipToPlay is null");    
                }
                break;
            case CombatAudio.Block:
                Debug.Log("TODO - Implement block audio");
                break;
            case CombatAudio.Death:
                Debug.Log("TODO - Implement death audio");
                break;
        }    
    } 

    private void PlayBaseAudioClip(BaseAudio type) {
        switch (type) { 
            case BaseAudio.Walk:
                baseAudioSource.clip = walkClip;
                baseAudioSource.loop = true;
                baseAudioSource.pitch = Random.Range(0.9f,1f);
                baseAudioSource.Play((ulong)Random.Range(0,44100/1000));
                break;
        }    
    }

    private void SetAudioAudioSource(AudioSource audioSource, bool hasToBeSpatial, float minDistance, float maxDistance, bool hasToBePlayedOnAwake) {
        audioSource.playOnAwake = hasToBePlayedOnAwake;
        if (hasToBeSpatial) { 
            audioSource.spatialBlend = 1.0f;   
        }
        else { 
            audioSource.spatialBlend = 0.0f;    
        }
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
    }

    private void AudioCycle() { 
        if(stats != null) { 
            if(stats.type == Stats.Type.Player) { 
                ManagePlayerBaseAudio();          
            }
            if(stats.type == Stats.Type.Ally) { 
                ManageAllyBaseAudio();    
            }
        }
        else { 
            stats = GetComponent<Stats>();    
        }
    }

    private void ManagePlayerBaseAudio() { 
        if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) { 
            if(baseAudioSource.clip != walkClip)
                PlayBaseAudioClip(BaseAudio.Walk);
            if(baseAudioSource.clip == walkClip && !baseAudioSource.isPlaying) { 
                baseAudioSource.Play();    
            }
        }
        else { 
           if(baseAudioSource.clip == walkClip) { 
                baseAudioSource.Stop();     
           }     
        }
    }

    private void ManageAllyBaseAudio() { 
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if(agent != null) { 
            if(agent.velocity != Vector3.zero) { 
                if(baseAudioSource.clip != walkClip) {
                    PlayBaseAudioClip(BaseAudio.Walk);
                }
                if(baseAudioSource.clip == walkClip && !baseAudioSource.isPlaying) { 
                    PlayBaseAudioClip(BaseAudio.Walk);           
                }
            }
            else { 
                if(baseAudioSource.clip == walkClip) { 
                    baseAudioSource.Stop();    
                }    
            } 
        
        }
        else { 
            Debug.Log(this.transform.root.gameObject.name + " is an ally but has no NavMeshAgent attached, cannot play Base Audio");        
        }
    }
}
