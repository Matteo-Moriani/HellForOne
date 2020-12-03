using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticChangeParticles : MonoBehaviour
{
    private GroupBehaviour _groupBehaviour;
    private GroupManager _groupManager;
    private ParticleSystem[] _particles;

    private void Awake()
    {
        _groupBehaviour = transform.root.GetComponent<GroupBehaviour>();
        _groupManager = transform.root.GetComponent<GroupManager>();
        _particles = transform.root.GetComponentsInChildren<ParticleSystem>();

        foreach(ParticleSystem p in _particles)
        {
            ParticleSystem.MainModule mainModule = p.main;
            mainModule.startColor = _groupManager.GroupColor;
        }
    }

    private void OnEnable()
    {
        _groupBehaviour.onOrderChanged += PlayParticles;
    }

    private void OnDisable()
    {
        _groupBehaviour.onOrderChanged -= PlayParticles;
    }

    private void PlayParticles(GroupBehaviour sender, GroupBehaviour.State newState)
    {
        foreach(ParticleSystem p in _particles)
        {
            p.Play();
        }
    }
}
