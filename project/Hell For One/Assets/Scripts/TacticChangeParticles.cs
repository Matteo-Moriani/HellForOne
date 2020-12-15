﻿using System.Collections;
using System.Collections.Generic;
using AI;
using AI.Imp;
using GroupSystem;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

public class TacticChangeParticles : MonoBehaviour
{
    private ImpGroupAi _impGroupAi;
    private GroupManager _groupManager;
    private ParticleSystem[] _particles;
    
    private void Awake()
    {
        _impGroupAi = transform.root.GetComponent<ImpGroupAi>();
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
        _impGroupAi.OnTacticChanged += PlayParticles;
    }

    private void OnDisable()
    {
        _impGroupAi.OnTacticChanged -= PlayParticles;
    }
    
    private void PlayParticles(TacticFactory obj)
    {
        foreach(ParticleSystem p in _particles)
        {
            p.Play();
        }
    }
}
