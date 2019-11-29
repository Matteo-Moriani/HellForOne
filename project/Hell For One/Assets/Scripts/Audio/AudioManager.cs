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

    [SerializeField]
    private static AudioMixerGroup walkAudioMixedGroup;
    [SerializeField]
    private static AudioMixerGroup combatAudioMixerGroup;

    [SerializeField]
    [Tooltip("Clips that will be played when an unit is hit")]
    private static AudioClip[] hitClips;

    [SerializeField]
    [Tooltip("Clips that will be played when an unit blocks")]
    private static AudioClip[] blockClips;

    [SerializeField]
    [Tooltip("Clips that will be played when an unit dies")]
    private static AudioClip[] deathClips;

    [SerializeField]
    [Tooltip("Clip that will be played when an unit walks")]
    private static AudioClip[] walkClips;

    /// <summary>
    /// Mixer group for base audio (Stuff like walking)
    /// </summary>
    public static AudioMixerGroup WalkAudioMixerGroup { get => walkAudioMixedGroup; set => walkAudioMixedGroup = value; }
    /// <summary>
    /// Mixer group for combat audio
    /// </summary>
    public static AudioMixerGroup CombatAudioMixerGroup { get => combatAudioMixerGroup; set => combatAudioMixerGroup = value; }

    private void Start()
    {
        /*
        stats = GetComponent<Stats>();

        combatAudioSource = gameObject.AddComponent<AudioSource>();
        SetAudioAudioSource(combatAudioSource, true, 5f, 500f, false);
        baseAudioSource = gameObject.AddComponent<AudioSource>();
        SetAudioAudioSource(baseAudioSource, true, 5f, 500f, false);
        if (baseAudioMixerGroup != null)
        {
            baseAudioSource.outputAudioMixerGroup = baseAudioMixerGroup;
        }
        if (combatAudioMixerGroup != null)
        {
            combatAudioSource.outputAudioMixerGroup = combatAudioMixerGroup;
        }
        */
    }

    /*
    private void Update()
    {
        AudioCycle();
    }
    */

    public static void PlayRandomCombatAudioClip(CombatAudio type, AudioSource combatAudioSource)
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
                break;
            case CombatAudio.Death:
                Debug.Log("TODO - Implement death audio");
                break;
        }
    }

    public static void PlayRandomWalkClip(Size size, AudioSource walkAudioSource) {
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
    
    /*
    public static void PlayBaseAudioClip(BaseAudio type, AudioSource baseAudioSource)
    {
        switch (type)
        {
            case BaseAudio.Walk:
                baseAudioSource.clip = walkClip;
                baseAudioSource.loop = true;
                baseAudioSource.pitch = Random.Range(0.9f, 1f);
                //baseAudioSource.PlayDelayed(Random.Range(0, 44100 / 1000));
                baseAudioSource.Play();
                break;
        }
    }
    */

    public static void SetAudioAudioSource(AudioSource audioSource, bool hasToBeSpatial, float minDistance, float maxDistance, bool hasToBePlayedOnAwake)
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

    //public static void ManagePlayerBaseAudio(AudioSource baseAudioSource) { 
    //    
    //}

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
            if (baseAudioSource.clip != walkClip)
                PlayBaseAudioClip(BaseAudio.Walk);
            if (baseAudioSource.clip == walkClip && !baseAudioSource.isPlaying)
            {
                baseAudioSource.Play();
            }
        }
        else
        {
            if (baseAudioSource.clip == walkClip)
            {
                baseAudioSource.Stop();
            }
        }
    }

    private void ManageAllyBaseAudio()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            if (agent.velocity != Vector3.zero)
            {
                if (baseAudioSource.clip != walkClip)
                {
                    PlayBaseAudioClip(BaseAudio.Walk);
                }
                if (baseAudioSource.clip == walkClip && !baseAudioSource.isPlaying)
                {
                    PlayBaseAudioClip(BaseAudio.Walk);
                }
            }
            else
            {
                if (baseAudioSource.clip == walkClip)
                {
                    baseAudioSource.Stop();
                }
            }

        }
        else
        {
            Debug.Log(this.transform.root.gameObject.name + " is an ally but has no NavMeshAgent attached, cannot play Base Audio");
        }
    }
    */
}
