using FactoryBasedCombatSystem;
using UnityEngine;

public class DashParticlesBehaviour : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private Dash _dash;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _dash = transform.root.GetComponentInChildren<Dash>();
    }

    private void OnEnable()
    {
        _dash.OnStartDash += OnStartDash;
    }

    private void OnDisable()
    {
        _dash.OnStartDash -= OnStartDash;
    }

    private void OnStartDash()
    {
        _particleSystem.Play();
    }
}
