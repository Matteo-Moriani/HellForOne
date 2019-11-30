using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public enum CombatAudio
    {
        Hit,
        Block,
        Death
    }

    public enum Size { 
        Small,
        Medium,
        Big
    }

    public enum Music { 
        OutOFCombat,
        Combat,
        Boss,
        BossHalfLife
    }

    [SerializeField]
    private  AudioMixerGroup walkAudioMixerGroup;
    [SerializeField]
    private  AudioMixerGroup combatAudioMixerGroup;
    [SerializeField]
    private AudioMixerGroup musicMixerGroup;

    [SerializeField]
    [Tooltip("Clips that will be played when an unit is hit")]
    private AudioClip[] hitClips;

    [SerializeField]
    [Tooltip("Clips that will be played when an unit blocks")]
    private AudioClip[] blockClips;

    [SerializeField]
    [Tooltip("Clips that will be played when an unit dies")]
    private AudioClip[] deathClips;

    [SerializeField]
    [Tooltip("Clip that will be played when an unit walks")]
    private AudioClip[] walkClips;

    [SerializeField]
    private AudioClip BossFightmusic;

    /// <summary>
    /// Mixer group for base audio (Stuff like walking)
    /// </summary>
    public AudioMixerGroup WalkAudioMixerGroup { get => walkAudioMixerGroup; private set => walkAudioMixerGroup = value; }
    /// <summary>
    /// Mixer group for combat audio
    /// </summary>
    public AudioMixerGroup CombatAudioMixerGroup { get => combatAudioMixerGroup; private set => combatAudioMixerGroup = value; }

    private static AudioManager _instance;

    public static AudioManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void PlayRandomCombatAudioClip(CombatAudio type, AudioSource combatAudioSource)
    {
        switch (type)
        {
            case CombatAudio.Hit:
                if (hitClips.Length > 0)
                {
                    AudioClip clipToPlay = hitClips[(Random.Range(0, hitClips.Length - 1))];
                    if (clipToPlay != null)
                    {
                        combatAudioSource.clip = clipToPlay;
                        combatAudioSource.Play();
                    }
                    else
                    {
                        Debug.Log(" Input manager is trying to play a clip but it is null");
                    }
                }
                break;
            case CombatAudio.Block:
                Debug.Log("TODO - Implement block audio");
                if (blockClips.Length > 0)
                {
                    AudioClip clipToPlay = blockClips[(Random.Range(0, blockClips.Length - 1))];
                    if (clipToPlay != null)
                    {
                        combatAudioSource.clip = clipToPlay;
                        combatAudioSource.Play();
                    }
                    else
                    {
                        Debug.Log(" Input manager is trying to play a clip but it is null");
                    }
                }
                break;
            case CombatAudio.Death:
                Debug.Log("TODO - Implement death audio");
                break;
        }
    }

    public void PlayRandomWalkClip(Size size, AudioSource walkAudioSource) {
        switch (size) { 
            case Size.Small:
                if(walkClips.Length > 0) {
                    if (!walkAudioSource.isPlaying) { 
                        AudioClip clipToPlay = walkClips[Random.Range(0,walkClips.Length - 1)];
                        
                        if(clipToPlay != null) { 
                            walkAudioSource.clip = clipToPlay;
                            walkAudioSource.Play();
                         }
                    }    
                }
                break;
            case Size.Medium:
                Debug.Log("TODO - Implement medium size walk audio");
                break;
            case Size.Big:
                Debug.Log("TODO - Implement big size walk audio");
                break;
        }       
    }
    
    public void SetAudioAudioSource(AudioSource audioSource, bool hasToBeSpatial, float minDistance, float maxDistance, bool hasToBePlayedOnAwake)
    {
        audioSource.playOnAwake = hasToBePlayedOnAwake;
        if (hasToBeSpatial)
        {
            audioSource.spatialBlend = 1.0f;
        }
        else
        {
            audioSource.spatialBlend = 0.0f;
        }
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
    }

    public void PlayMusic(AudioSource musicAudioSource, Music type) {
        switch (type) { 
            case Music.OutOFCombat:
                break;
            case Music.Combat:
                break;
            case Music.Boss:
                if(musicAudioSource.clip != BossFightmusic) {
                    musicAudioSource.loop = true;
                    musicAudioSource.clip = BossFightmusic;
                    musicAudioSource.Play();
                }
                break;
            case Music.BossHalfLife:
                break;
        }
    }
}
