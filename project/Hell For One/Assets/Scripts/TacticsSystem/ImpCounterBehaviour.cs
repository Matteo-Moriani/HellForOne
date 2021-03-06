﻿using System;
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
        
        public void StartCounter() => OnStartCounter?.Invoke();

        public void StopCounter() => OnStopCounter?.Invoke();
    }
}