using System;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using Interfaces;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace AI.Imp
{
    /// <summary>
    /// Group behaviour should call this in order to start an attack.
    /// Attack selection should be managed here based on current order
    /// TODO :- should implement IReincarnationObserver?
    /// TODO :- should implement IActionBlockSubject/Observer?
    /// </summary>
    public class ImpCombatBehaviour : MonoBehaviour, IGroupOrdersObserver
    {
        private CombatSystem _combatSystem;

        private Attack _currentAttack;

        private void Awake() => _combatSystem = GetComponentInChildren<CombatSystem>();

        public void Attack()
        {
            if(_currentAttack != null)
                _combatSystem.StartAttack(_currentAttack);
        }

        public void OnOrderGiven(Tactic newTactic) { }

        public void OnOrderAssigned(Tactic newTactic)
        {
            if(newTactic.TacticAttack != null)
                _currentAttack = newTactic.TacticAttack.GetAttack();    
        }
    }
}