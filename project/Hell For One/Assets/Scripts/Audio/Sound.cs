using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.5f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool loop;

    public bool random;

    [HideInInspector]
    public AudioSource source;

    [HideInInspector]
    public float spatialBlend;

    [HideInInspector]
    public float defaultVolume;
}