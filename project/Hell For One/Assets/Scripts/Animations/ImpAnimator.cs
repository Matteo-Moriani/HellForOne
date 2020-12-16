using System;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.Interfaces;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using TacticsSystem;
using TMPro;
using UnityEngine;

namespace Animations
{
    public class ImpAnimator : MonoBehaviour, IHitPointsObserver
    {
        private Animator _animator;
        private CombatSystem _combatSystem;
        private ImpRecruitBehaviour _impRecruitBehaviour;
        private ImpCounterBehaviour _impCounterBehaviour;
        
        private Vector3 _lastFramePosition;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _combatSystem = GetComponentInChildren<CombatSystem>();
            _impRecruitBehaviour = GetComponent<ImpRecruitBehaviour>();
            _impCounterBehaviour = GetComponent<ImpCounterBehaviour>();

            _lastFramePosition = transform.position;
        }

        private void OnEnable()
        {
            _combatSystem.OnStartAttack += OnStartAttack;
            _combatSystem.OnBlockedHitReceived += OnBlockedHitReceived;

            _impRecruitBehaviour.OnStartRecruit += OnStartRecruit;
            _impRecruitBehaviour.OnStopRecruit += OnStopRecruit;

            _impCounterBehaviour.OnStartCounter += OnStartCounter;
            _impCounterBehaviour.OnStopCounter += OnStopCounter;
        }

        private void OnDisable()
        {
            _combatSystem.OnStartAttack -= OnStartAttack;
            _combatSystem.OnBlockedHitReceived -= OnBlockedHitReceived;
            
            _impRecruitBehaviour.OnStartRecruit -= OnStartRecruit;
            _impRecruitBehaviour.OnStopRecruit -= OnStopRecruit;
            
            _impCounterBehaviour.OnStartCounter -= OnStartCounter;
            _impCounterBehaviour.OnStopCounter -= OnStopCounter;
        }

        private void Update()
        {
            _animator.SetBool("isMoving",IsMoving());

            _lastFramePosition = transform.position;
        }

        private void OnBlockedHitReceived(Attack arg1, CombatSystem arg2, Vector3 arg3) =>
            _animator.SetTrigger("parry");

        private void OnStartAttack(Attack attack) => _animator.SetTrigger(attack.name);

        private void OnStopRecruit() => _animator.SetBool("isRecruiting", false);

        private void OnStartRecruit() => _animator.SetBool("isRecruiting", true);

        private void OnStartCounter() => _animator.SetBool("isBlocking", true);

        private void OnStopCounter() => _animator.SetBool("isBlocking", false);

        private bool IsMoving() => Vector3.Distance(transform.position, _lastFramePosition) >= 0.01f;
        public void OnZeroHp() => _animator.SetTrigger("death");
    }
}