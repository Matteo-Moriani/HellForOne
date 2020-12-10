using ArenaSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaWallBehaviour : MonoBehaviour, IArenaObserver
{
    private Collider _collider;
    private ParticleSystem[] _particles;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _particles = GetComponentsInChildren<ParticleSystem>();
    }

    public void EnterBattle(ArenaManager subject)
    {
        _collider.enabled = true;
        foreach(ParticleSystem p in _particles)
        {
            p.Play();
        }
    }

    public void ExitBattle(ArenaManager subject)
    {
        _collider.enabled = false;
        foreach(ParticleSystem p in _particles)
        {
            p.Stop();
        }
    }

    public void PrepareBattle(ArenaManager subject)
    {
        subject.NotifyBattlePrepared(this);
    }
}
