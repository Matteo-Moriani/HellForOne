using ActionsBlockSystem;
using FactoryBasedCombatSystem.Interfaces;
using ReincarnationSystem;
using UnityEngine;

namespace ManaSystem
{
    public class ManaParticlesBehaviour : MonoBehaviour, IReincarnationObserver
    {
        private ParticleSystem[] _particleSystems;
        private AudioSource _audioSource;

        private readonly ActionLock _particlesLock = new ActionLock();

        private bool _particlesActivated;
        
        private void Awake()
        {
            _particlesLock.AddLock();
            
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            ImpMana.OnManaPoolChanged += OnManaPoolChanged;
        }
        
        private void OnDisable()
        {
            ImpMana.OnManaPoolChanged -= OnManaPoolChanged;
        }
        
        private void Start()
        {
            foreach (var system in _particleSystems)
            {
                system.Stop();
            }
        }

        private void OnManaPoolChanged(float currentManaPool)
        {
            if(!_particlesLock.CanDoAction()) return;

            if (ImpMana.CurrentChargedSegments > 0)
            {
                if(_particlesActivated) return;

                _particlesActivated = true;
                
                foreach (ParticleSystem system in _particleSystems)
                {
                    system.Play();    
                }
                _audioSource.Play();
            }
            else
            {
                if(!_particlesActivated) return;

                _particlesActivated = false;
                
                foreach (ParticleSystem system in _particleSystems)
                {
                    system.Stop();    
                }
                _audioSource.Stop();
            }
        }

        public void StartLeader() => _particlesLock.RemoveLock();

        public void StopLeader()
        {
            foreach (var system in _particleSystems)
            {
                system.Stop();
            }
            _audioSource.Stop();
        }
    }
}