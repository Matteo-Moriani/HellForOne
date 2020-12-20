using System;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.Interfaces;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace Animations
{
    public class MidBossAnimator : MonoBehaviour, IHitPointsObserver
    {
        private CombatSystem _combatSystem;
        private Animator _animator;
        private Rigidbody _rigidbody;
        private Stun _stun;

        private void Awake()
        {
            _combatSystem = GetComponentInChildren<CombatSystem>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _stun = GetComponentInChildren<Stun>();
        }

        private void OnEnable()
        {
            _combatSystem.OnStartAttack += OnStartAttack;
            _stun.OnStartStun += OnStartStun;
            _stun.OnStopStun += OnStopStun;
        }
        
        private void OnDisable()
        {
            _combatSystem.OnStartAttack -= OnStartAttack;
            _stun.OnStartStun += OnStartStun;
            _stun.OnStopStun += OnStopStun;
        }

        private void Update() => _animator.SetBool("isMoving",IsMoving());

        private bool IsMoving() => _rigidbody.velocity.magnitude >= 0.2f;
        
        private void OnStartAttack(Attack obj) => _animator.SetTrigger(obj.name);

        private void OnStartStun() => _animator.SetBool("isStunned", true);

        private void OnStopStun() => _animator.SetBool("isStunned", false);
        public void OnZeroHp() => _animator.SetTrigger("death");
    }
}
