using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalAttackBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _effectToActivate;
    private ParticleSystem[] _particlesToActivate;
    private AudioSource _soundToPlay;

    private void Awake()
    {
        _particlesToActivate = _effectToActivate.GetComponentsInChildren<ParticleSystem>();
        _soundToPlay = _effectToActivate.GetComponentInChildren<AudioSource>();
    }

    public void Activate()
    {
        foreach(ParticleSystem p in _particlesToActivate)
        {
            p.Play();
        }
        _soundToPlay.Play();
    }

    public void Deactivate()
    {
        
    }
}
