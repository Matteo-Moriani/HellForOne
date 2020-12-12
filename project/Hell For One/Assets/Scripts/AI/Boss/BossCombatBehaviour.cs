using System;
using AI.Movement;
using CooldownSystem;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace AI.Boss
{
    public class BossCombatBehaviour : MonoBehaviour, ICooldown
    {
        [SerializeField] private float attackCooldown;

        private CombatSystem _combatSystem;
        private Cooldowns _cooldowns;
        private ContextSteering _contextSteering;

        private float _currentCooldown;

        public event Action OnStartBossAttack;
        public event Action OnStopBossAttack;

        private void Awake()
        {
            _combatSystem = GetComponentInChildren<CombatSystem>();
            _cooldowns = GetComponent<Cooldowns>();
        }

        private void OnEnable()
        {
            _combatSystem.OnStartAttack += OnStartAttack;
            _combatSystem.OnStopAttack += OnStopAttack;
        }

        private void OnDisable()
        {
            _combatSystem.OnStartAttack += OnStartAttack;
            _combatSystem.OnStopAttack += OnStopAttack;
        }

        public void Attack(Attack attack, Transform target = null)
        {
            if(!_cooldowns.TryAbility(this)) return;

            _combatSystem.StartAttack(attack, target);
        }

        private void OnStartAttack(Attack obj) => OnStartBossAttack?.Invoke();

        private void OnStopAttack() => OnStopBossAttack?.Invoke();

        public float GetCooldown() => attackCooldown;

        public void NotifyCooldownStart() { }

        public void NotifyCooldownEnd() { }
    }
}