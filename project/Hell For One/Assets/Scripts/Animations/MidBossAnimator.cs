using System;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace Animations
{
    public class MidBossAnimator : MonoBehaviour
    {
        private CombatSystem _combatSystem;
        private Animator _animator;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _combatSystem = GetComponentInChildren<CombatSystem>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
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
        }

        private bool IsMoving() => _rigidbody.velocity.magnitude >= 0.2f;
        
        private void OnStartAttack(Attack obj)
        {
            _animator.SetTrigger(obj.name);
        }


        // private protected override void OnStartAttack(NormalCombat sender, GenericAttack attack)
        // {
        //     if (attack.CanHitMultipleTargets)
        //     {
        //         PlayGroupAttackAnimation();
        //     }
        // }
    }
}
