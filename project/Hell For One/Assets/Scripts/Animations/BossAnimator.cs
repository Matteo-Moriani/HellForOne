using System;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace Animations
{
    public class BossAnimator : MonoBehaviour
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
            _animator.SetBool("isMoving", IsMoving());
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

        // vecchio script estendeve enemyAnimator

        // public void PlayGlobalAttackAnimation()
        // {
        //     SetAllBoolsToFalse();
        //     animator.SetTrigger("globalAttack");
        // }
        //
        // private protected override void OnStartAttack(NormalCombat sender, GenericAttack attack)
        // {
        //     switch(attack.name)
        //     {
        //         case "BossNormalAttackSwipe":
        //             PlaySingleAttackAnimation();
        //             break;
        //         case "FlameCircle":
        //             PlayGroupAttackAnimation();
        //             break;
        //         case "FlameExplosion":
        //             PlayGlobalAttackAnimation();
        //             break;
        //         default:
        //             break;
        //     }        
        // }
    }
}
