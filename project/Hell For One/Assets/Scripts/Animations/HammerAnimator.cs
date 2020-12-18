using System;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace Animations
{
    public class HammerAnimator : MonoBehaviour
    {
        private CombatSystem _combatSystem;
        private Animator _animator;

        private void Awake()
        {
            _combatSystem = GetComponentInChildren<CombatSystem>();
            _animator = GetComponent<Animator>();
        }

        private void OnEnable() => _combatSystem.OnStartAttack += OnStartAttack;
        private void OnDisable() => _combatSystem.OnStartAttack -= OnStartAttack;

        private void OnStartAttack(Attack obj) => _animator.SetTrigger(obj.name);
    }
}