using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CombatAudio : MonoBehaviour
{
    public enum Type { 
        Hit,
        Block,
        Death
    }
    
    private AudioSource audioSource;

    [SerializeField]
    [Tooltip("Clips that will be played when this unit is hit")]
    private AudioClip[] hitClips;

    [SerializeField]
    [Tooltip("Clips that will be played when this unit blocks")]
    private AudioClip[] blockClips;

    [SerializeField]
    [Tooltip("Clips that will be played when this unit dies")]
    private AudioClip[] deathClips;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomClip(Type type) {
        switch (type) { 
            case Type.Hit:
                    AudioClip clipToPlay = hitClips[(Random.Range(0,hitClips.Length - 1))];
                    if(clipToPlay != null && audioSource != null) { 
                        audioSource.clip = clipToPlay;
                        audioSource.Play();
                }
                else { 
                    Debug.Log(name + " CombatAudio: AudioSource is null or ClipToPlay is null");    
                }
                break;
            case Type.Block:
                Debug.Log("TODO - Implement block audio");
                break;
            case Type.Death:
                Debug.Log("TODO - Implement death audio");
                break;
        }    
    } 
}
