using System;
using AI.Movement;
using CallToArmsSystem;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.Interfaces;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using Player;
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
        private LeaderCallToArms _leaderCallToArms;
        private ContextSteering _contextSteering;
        private PlayerMovement _playerMovement;
        private bool forceMoveLock = false;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _combatSystem = GetComponentInChildren<CombatSystem>();
            _impRecruitBehaviour = GetComponent<ImpRecruitBehaviour>();
            _impCounterBehaviour = GetComponent<ImpCounterBehaviour>();
            _leaderCallToArms = GetComponent<LeaderCallToArms>();
            _contextSteering = GetComponent<ContextSteering>();
            _playerMovement = GetComponent<PlayerMovement>();
        }

        private void OnEnable()
        {
            _combatSystem.OnStartAttack += OnStartAttack;
            _combatSystem.OnBlockedHitReceived += OnBlockedHitReceived;

            _impRecruitBehaviour.OnStartRecruit += OnStartRecruit;
            _impRecruitBehaviour.OnStopRecruit += OnStopRecruit;

            _impCounterBehaviour.OnStartCounter += OnStartCounter;
            _impCounterBehaviour.OnStopCounter += OnStopCounter;

            _leaderCallToArms.OnCallToArmsStart += OnCallToArmsStart;
            _leaderCallToArms.OnCallToArmsStop += OnCallToArmsStop;

            _contextSteering.OnStartMoving += OnStartMoving;
            _contextSteering.OnStopMoving += OnStopMoving;

            _playerMovement.OnStartMoving += OnStartMoving;
            _playerMovement.OnStopMoving += OnStopMoving;
        }

        private void OnDisable()
        {
            _combatSystem.OnStartAttack -= OnStartAttack;
            _combatSystem.OnBlockedHitReceived -= OnBlockedHitReceived;
            
            _impRecruitBehaviour.OnStartRecruit -= OnStartRecruit;
            _impRecruitBehaviour.OnStopRecruit -= OnStopRecruit;
            
            _impCounterBehaviour.OnStartCounter -= OnStartCounter;
            _impCounterBehaviour.OnStopCounter -= OnStopCounter;
            
            _leaderCallToArms.OnCallToArmsStart -= OnCallToArmsStart;
            _leaderCallToArms.OnCallToArmsStop -= OnCallToArmsStop;
        }

        private void OnStartMoving()
        {
            if(!forceMoveLock)
                _animator.SetBool("isMoving", true);
        }

        private void OnStopMoving() {
            if(!forceMoveLock)
                _animator.SetBool("isMoving", false);
        }
        
        private void OnBlockedHitReceived(Attack arg1, CombatSystem arg2, Vector3 arg3) =>
            _animator.SetTrigger("parry");

        private void OnStartAttack(Attack attack) => _animator.SetTrigger(attack.name);

        private void OnStopRecruit() => _animator.SetBool("isRecruiting", false);

        private void OnStartRecruit() => _animator.SetBool("isRecruiting", true);

        private void OnStartCounter() => _animator.SetBool("isBlocking", true);

        private void OnStopCounter() => _animator.SetBool("isBlocking", false);

        private void OnCallToArmsStart() => _animator.SetBool("inCallToArms", true);

        private void OnCallToArmsStop() => _animator.SetBool("inCallToArms", false);
        
        public void OnZeroHp() => _animator.SetTrigger("death");

        public void ForceMove(bool b)
        {
            forceMoveLock = b; 
            _animator.SetBool("isMoving", b);
        }
    }
}