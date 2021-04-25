using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.Interfaces;
using UnityEngine;

public class StunParticlesBehaviour : MonoBehaviour, IHitPointsObserver
{
    private Stun _stun;
    private ParticleSystem[] _particles;
    //private AudioSource _audioSource;

    private void Awake()
    {
        _stun = transform.root.GetComponentInChildren<Stun>();
        _particles = GetComponentsInChildren<ParticleSystem>();
        //_audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _stun.OnStartStun += OnStartStun;
        _stun.OnStopStun += OnStopStun;
    }

    private void OnDisable()
    {
        _stun.OnStartStun -= OnStartStun;
        _stun.OnStopStun -= OnStopStun;
    }

    private void OnStartStun()
    {
        foreach(ParticleSystem p in _particles)
        {
            p.Play();
        }
        //_audioSource.Play();
    }

    private void OnStopStun()
    {
        foreach(ParticleSystem p in _particles)
        {
            p.Stop();
        }
        //_audioSource.Stop();
    }

    public void OnZeroHp()
    {
        OnStopStun();
    }
}