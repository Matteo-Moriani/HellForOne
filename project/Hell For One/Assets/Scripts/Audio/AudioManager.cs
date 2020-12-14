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
    private AudioMixerGroup deathMixerGroup;
    [SerializeField]
    private AudioMixerGroup dashMixerGroup;
    [SerializeField]
    private AudioMixerGroup roarMixerGroup;

    [SerializeField]
    [Tooltip("Clips that will be played when an unit is hit")]
    private AudioClip[] hitClips;

    [SerializeField]
    [Tooltip("Clips that will be played when an unit blocks")]
    private AudioClip[] blockClips;

    [SerializeField]
    [Tooltip("Clips that will be played when a big unit dies")]
    private AudioClip[] bigDeathClips;

    [SerializeField]
    [Tooltip("Clips that will be played when a small unit dies")]
    private AudioClip[] smallDeathClips;

    [SerializeField]
    [Tooltip("Clip that will be played when an unit walks")]
    private AudioClip[] walkClips;

    [SerializeField]
    [Tooltip("Clips that will be played during a dash")]
    private AudioClip[] dashClips;

    [SerializeField]
    [Tooltip("Clips that will be played during global attack")]
    private AudioClip[] roarClips;

    [SerializeField]
    private AudioClip outOfCombatMusicMain;

    [SerializeField]
    private AudioClip outOfCombatMusicLoop;

    [SerializeField]
    private AudioClip combatMusicMain;

    [SerializeField]
    private AudioClip combatMusicLoop;

    [SerializeField]
    private AudioClip bossFightMusicMain;

    [SerializeField]
    private AudioClip bossFightMusicLoop;

    private bool canPlayFootSteps = true;

    /// <summary>
    /// Mixer group for base audio (Stuff like walking)
    /// </summary>
    public AudioMixerGroup WalkAudioMixerGroup { get => walkAudioMixerGroup; private set => walkAudioMixerGroup = value; }
    /// <summary>
    /// Mixer group for combat audio
    /// </summary>
    public AudioMixerGroup CombatAudioMixerGroup { get => combatAudioMixerGroup; private set => combatAudioMixerGroup = value; }
    /// <summary>
    /// Mixer group for death audio
    /// </summary>
    public AudioMixerGroup DeathMixerGroup { get => deathMixerGroup; private set => deathMixerGroup = value; }
    /// <summary>
    /// Mixer group for music audio
    /// </summary>
    public AudioMixerGroup MusicMixerGroup { get => musicMixerGroup; private set => musicMixerGroup = value; }
    public AudioMixerGroup DashMixerGroup { get => dashMixerGroup; private set => dashMixerGroup = value; }
    public AudioMixerGroup RoarMixerGroup { get => roarMixerGroup; private set => roarMixerGroup = value; }

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
                    AudioClip clipToPlay = hitClips[(Random.Range(0, hitClips.Length))];
                    if (clipToPlay != null)
                    {
                        combatAudioSource.clip = clipToPlay;
                        //combatAudioSource.Play();
                        combatAudioSource.PlayOneShot(clipToPlay);
                    }
                    //else
                    //{
                    //    Debug.Log(" Input manager is trying to play a clip but it is null");
                    //}
                }
                break;
            case CombatAudio.Block:
                if (blockClips.Length > 0)
                {
                    AudioClip clipToPlay = blockClips[(Random.Range(0, blockClips.Length))];
                    if (clipToPlay != null)
                    {
                        combatAudioSource.clip = clipToPlay;
                        //combatAudioSource.Play();
                        combatAudioSource.PlayOneShot(clipToPlay);
                    }
                    //else
                    //{
                    //    Debug.Log(" Input manager is trying to play a clip but it is null");
                    //}
                }
                break;
        }
    }

    // public void PlayDeathSound(Stats.Type type, AudioSource deathAudioSource) {
    //     switch (type) { 
    //         case Stats.Type.Ally:
    //             if(smallDeathClips.Length > 0) {
    //                 AudioClip clipToPlay = smallDeathClips[Random.Range(0, smallDeathClips.Length - 1)];
    //                 if (clipToPlay != null)
    //                 {
    //                     deathAudioSource.clip = clipToPlay;
    //                     deathAudioSource.PlayOneShot(clipToPlay);
    //                 }
    //             }
    //             break;
    //         case Stats.Type.Boss:
    //             if(bigDeathClips.Length > 0) {
    //                 AudioClip clipToPlay = bigDeathClips[Random.Range(0, smallDeathClips.Length - 1)];
    //                 if (clipToPlay != null)
    //                 {
    //                     deathAudioSource.clip = clipToPlay;
    //                     deathAudioSource.PlayOneShot(clipToPlay);
    //                 }
    //             }
    //             break;
    //         case Stats.Type.Enemy:
    //             if (smallDeathClips.Length > 0)
    //             {
    //                 AudioClip clipToPlay = smallDeathClips[Random.Range(0, smallDeathClips.Length - 1)];
    //                 if (clipToPlay != null)
    //                 {
    //                     deathAudioSource.clip = clipToPlay;
    //                     deathAudioSource.PlayOneShot(clipToPlay);
    //                 }
    //             }
    //             break;
    //         case Stats.Type.Player:
    //             if (smallDeathClips.Length > 0)
    //             {
    //                 AudioClip clipToPlay = smallDeathClips[Random.Range(0, smallDeathClips.Length - 1)];
    //                 if (clipToPlay != null)
    //                 {
    //                     deathAudioSource.clip = clipToPlay;
    //                     deathAudioSource.PlayOneShot(clipToPlay);
    //                 }
    //             }
    //             break;
    //         case Stats.Type.None:
    //             //Debug.Log("You are trying to play a death sound but the type of this unit is Stats.Type.None");
    //             break;
    //     }
    // }

    public float PlayRandomWalkClip(Size size, AudioSource walkAudioSource) {
        if (canPlayFootSteps) {
            switch (size)
            {
                case Size.Small:
                    if (walkClips.Length > 0)
                    {
                        if (!walkAudioSource.isPlaying)
                        {
                            AudioClip clipToPlay = walkClips[Random.Range(0, walkClips.Length - 1)];

                            if (clipToPlay != null)
                            {
                                walkAudioSource.clip = clipToPlay;
                                //walkAudioSource.pitch = Random.Range(0.9f,1.1f);
                                //walkAudioSource.PlayOneShot(clipToPlay);
                                walkAudioSource.Play();
                                return clipToPlay.length;
                            }
                        }
                    }
                    break;
                case Size.Medium:
                    //Debug.Log("TODO - Implement medium size walk audio");
                    break;
                case Size.Big:
                    //Debug.Log("TODO - Implement big size walk audio");
                    break;
            }
        }
        return 0;
    }
    
    public void PlayRandomRoarClip(AudioSource audioSource) {
        if (roarClips.Length > 0)
        {
            AudioClip clipToPlay = roarClips[(Random.Range(0, blockClips.Length))];
            if (clipToPlay != null)
            {
                audioSource.clip = clipToPlay;
                audioSource.Play();
            }
        }
    }

    public float PlayRandomDashClip(AudioSource audioSource) { 

        if(dashClips.Length > 0) {
            AudioClip clipToPlay = dashClips[Random.Range(0,dashClips.Length)];
            
            audioSource.clip = clipToPlay;
            audioSource.Play();
            return clipToPlay.length;
        }
        
        return 0;
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

    public float PlayMusic(AudioSource musicAudioSource, Music type) {
        switch (type) { 
            case Music.OutOFCombat:
                musicAudioSource.clip = outOfCombatMusicMain;
                musicAudioSource.loop = false;
                musicAudioSource.Play();
                return outOfCombatMusicMain.length;
            case Music.Combat:
                musicAudioSource.clip = combatMusicMain;
                musicAudioSource.loop = false;
                musicAudioSource.Play();
                return combatMusicMain.length;
            case Music.Boss:
                musicAudioSource.clip = bossFightMusicMain;
                musicAudioSource.loop = false;
                musicAudioSource.Play();
                return bossFightMusicMain.length;
            case Music.BossHalfLife:
                break;
        }
        return 0;
    }

    public void PlayMusicLoop(AudioSource musicAudioSource, Music type)
    {
        switch (type)
        {
            case Music.OutOFCombat:
                musicAudioSource.clip = outOfCombatMusicLoop;
                musicAudioSource.loop = true;
                musicAudioSource.Play();
                break;
            case Music.Combat:
                musicAudioSource.clip = combatMusicLoop;
                musicAudioSource.loop = true;
                musicAudioSource.Play();
                break;
            case Music.Boss:
                musicAudioSource.clip = bossFightMusicLoop;
                musicAudioSource.loop = true;
                musicAudioSource.Play();
                break;
            case Music.BossHalfLife:
                break;
        }
    }

    public void LockWalkAudio() {
        canPlayFootSteps = false;    
    }

    public void UnlockWalkAudio() { 
        canPlayFootSteps = true;    
    }
}
