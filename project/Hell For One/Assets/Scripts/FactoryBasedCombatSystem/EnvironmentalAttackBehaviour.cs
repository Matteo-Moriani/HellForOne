using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalAttackBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _effectToActivate;
    private ParticleSystem[] _particlesToActivate;

    private void Awake()
    {
        _particlesToActivate = _effectToActivate.GetComponentsInChildren<ParticleSystem>();
    }

    public void Activate()
    {
        foreach(ParticleSystem p in _particlesToActivate)
        {
            p.Play();
        }
    }

    public void Deactivate()
    {
        
    }
}
