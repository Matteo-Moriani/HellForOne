using System;
using CooldownSystem;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using TacticsSystem.Interfaces;
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
    public class ImpCombatBehaviour : MonoBehaviour, ICooldown, ITacticsObserver
    {
        private CombatSystem _combatSystem;
        private Cooldowns _cooldowns;

        private Attack _currentAttack;
        private float _currentCooldown;

        private void Awake()
        {
            _cooldowns = GetComponent<Cooldowns>();
            _combatSystem = GetComponentInChildren<CombatSystem>();
        }
        
        public void Attack(Transform target = null)
        {
            if(_currentAttack == null) return;
            
            if(!_cooldowns.TryAbility(this)) return;
            
            _combatSystem.StartAttack(_currentAttack,target);
        }

        public float GetCooldown() => _currentCooldown;

        public void NotifyCooldownStart() { }

        public void NotifyCooldownEnd() { }

        public void StartTactic(Tactic newTactic)
        {
            if(newTactic.GetType() != typeof(OffensiveTactic)) return;

            OffensiveTacticData data = ((OffensiveTactic) newTactic).GetOffensiveTacticData();

            _currentAttack = data.TacticAttack.GetAttack();
            _currentCooldown = data.AttackRateo;
        }

        public void EndTactic()
        {
            _currentAttack = null;
            _currentCooldown = 0f;
        }
    }
}