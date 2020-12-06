using System;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace Animations
{
    public class RefactorImpAnimator : MonoBehaviour
    {
        private Animator _animator;
        private CombatSystem _combatSystem;
        
        private Vector3 _lastFramePosition;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _combatSystem = GetComponentInChildren<CombatSystem>();

            _lastFramePosition = transform.position;
        }

        private void OnEnable()
        {
            _combatSystem.OnStartAttack += OnStartAttack;
        }

        private void OnDisable()
        {
            _combatSystem.OnStartAttack -= OnStartAttack;
        }

        private void Update()
        {
            _animator.SetBool("isMoving",IsMoving());

            _lastFramePosition = transform.position;
        }

        private void OnStartAttack(Attack attack) => _animator.SetTrigger(attack.name);

        private bool IsMoving() => Vector3.Distance(transform.position, _lastFramePosition) >= 0.01f;
    }
}