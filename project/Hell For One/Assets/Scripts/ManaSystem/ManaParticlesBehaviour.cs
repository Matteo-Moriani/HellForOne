using ActionsBlockSystem;
using FactoryBasedCombatSystem.Interfaces;
using ReincarnationSystem;
using UnityEngine;

namespace ManaSystem
{
    public class ManaParticlesBehaviour : MonoBehaviour, IReincarnationObserver, IHitPointsObserver
    {
        private ParticleSystem[] _particleSystems;

        private readonly ActionLock _particlesLock = new ActionLock();

        private bool _particlesActivated;
        
        private void Awake()
        {
            _particlesLock.AddLock();
            
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
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
            }
            else
            {
                if(!_particlesActivated) return;

                _particlesActivated = false;
                
                foreach (ParticleSystem system in _particleSystems)
                {
                    system.Stop();    
                }   
            }
        }

        public void StartLeader()
        {
            _particlesLock.RemoveLock();
        }

        public void StopLeader()
        {
            
        }

        public void OnZeroHp()
        {
            foreach (var system in _particleSystems)
            {
                system.Stop();
            }
        }
    }
}