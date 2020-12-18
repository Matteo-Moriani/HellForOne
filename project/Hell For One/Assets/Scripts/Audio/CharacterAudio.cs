using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using System;
using UnityEngine;

public abstract class CharacterAudio : MonoBehaviour
{
    public float rollOffMaxDistance = 50f;
    public Sound[] sounds;
    private AudioSource _attacksSource;

    private void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            if(s.random == true)
            {
                s.source.volume = s.volume * UnityEngine.Random.Range(0.8f, 1.2f);
                s.source.pitch = s.pitch * UnityEngine.Random.Range(0.8f, 1.2f);
                s.source.dopplerLevel += UnityEngine.Random.Range(0f, 1f);
            }
            else
            {
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.dopplerLevel = 0f;
            }
            
            s.source.clip = s.clip;
            s.source.spatialBlend = s.spatialBlend;
            s.source.loop = s.loop;
            s.source.rolloffMode = AudioRolloffMode.Linear;
            s.source.maxDistance = rollOffMaxDistance;
            s.source.playOnAwake = false;
        }
        
        _attacksSource = gameObject.AddComponent<AudioSource>();
        _attacksSource.dopplerLevel = 0f;
        _attacksSource.spatialBlend = 0.9f;
        _attacksSource.loop = false;
        _attacksSource.rolloffMode = AudioRolloffMode.Linear;
        _attacksSource.maxDistance = rollOffMaxDistance;
        _attacksSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        GetComponentInChildren<CombatSystem>().OnStartAttack += OnStartAttack;

        SubscribeToOtherEvents();
    }

    private void OnDisable()
    {
        GetComponentInChildren<CombatSystem>().OnStartAttack -= OnStartAttack;

        UnsubscribeToOtherEvents();
    }

    public abstract void SubscribeToOtherEvents();

    public abstract void UnsubscribeToOtherEvents();

    protected void Play(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if(!s.source.isPlaying)
            s.source.Play();
    }

    protected void Stop(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if(s.source.loop == true)
            s.source.Stop();
    }

    private void OnStartAttack(Attack attack)
    {
        if(attack.GetData().AttackCry != null)
        {
            _attacksSource.clip = attack.GetData().AttackCry;
            _attacksSource.volume = attack.GetData().Volume;
            _attacksSource.pitch = attack.GetData().Pitch;
            
            _attacksSource.Play();
        }
    }
}

