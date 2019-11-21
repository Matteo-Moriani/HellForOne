using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUtilities : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

    public void PlayRandomClip() { 
        AudioSource audioSource = GetComponent<AudioSource>();
        
        AudioClip clip = clips[Random.Range(0,clips.Length - 1)];

        if(clip != null) {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else { 
            Debug.Log(this.transform.root.name + " Tried to play a clip but didn't found it");    
        }
    }
}
