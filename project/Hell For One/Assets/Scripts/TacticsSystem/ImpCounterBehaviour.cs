using System;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace TacticsSystem
{
    public class ImpCounterBehaviour : MonoBehaviour
    {
        private CombatSystem _combatSystem;
        private Attack _counterAttack;

        public event Action OnStartCounter;
        public event Action OnStopCounter;
        
        private void Awake() => _combatSystem = GetComponentInChildren<CombatSystem>();
        
        public void StartCounter(Attack counterAttack)
        {
            _counterAttack = counterAttack;
            _combatSystem.OnBlockedHitReceived += OnBlockedHitReceived;
            
            OnStartCounter?.Invoke();
        }

        public void StopCounter()
        {
            _counterAttack = null;
            _combatSystem.OnBlockedHitReceived -= OnBlockedHitReceived;
            
            OnStopCounter?.Invoke();
        }

        private void OnBlockedHitReceived(Attack arg1, CombatSystem arg2, Vector3 arg3)
        {
            _combatSystem.StartAttack(_counterAttack);   
        }
    }
}