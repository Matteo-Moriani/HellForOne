using GroupAbilitiesSystem;
using GroupAbilitiesSystem.ScriptableObjects;
using GroupSystem;
using UnityEngine;

public class SpecialAbilityParticles : MonoBehaviour
{
    private GroupAbilities _groupAbilities;
    private ParticleSystem[] _particles;
    //private AudioSource _audioSource;

    private void Awake()
    {
        _groupAbilities = transform.root.GetComponentInChildren<GroupAbilities>();
        _particles = GetComponentsInChildren<ParticleSystem>();
        //_audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _groupAbilities.OnStartGroupAbility += OnStartGroupAbility;
    }

    private void OnDisable()
    {
        _groupAbilities.OnStartGroupAbility -= OnStartGroupAbility;
    }

    private void OnStartGroupAbility(GroupAbilities g, GroupAbility ga)
    {
        foreach(ParticleSystem p in _particles)
        {
            p.Play();
        }
        //_audioSource.Play();
    }
}