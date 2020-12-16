using System;
using System.Collections;
using ActionsBlockSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using TacticsSystem.Interfaces;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class Knockback : MonoBehaviour, IActionsBlockSubject, ITacticsObserver
    {
        #region Fields

        [SerializeField] private UnitActionsBlockManager.UnitAction[] actionBlocks;
        
        public event Action OnStartKnockback;
        public event Action OnStopKnockback;

        private CombatSystem _combatSystem;
        private Rigidbody _rigidbody;

        private Coroutine _knockbackCr = null;

        private readonly ActionLock _knockbackLock = new ActionLock();

        #endregion

        #region Unity methods

        private void Awake()
        {
            _combatSystem = GetComponent<CombatSystem>();
            _rigidbody = transform.root.GetComponent<Rigidbody>();
        }
        
        private void OnEnable()
        {
            _combatSystem.OnBlockedHitReceived += OnBlockedHitReceived;
            _combatSystem.OnDamageHitReceived += OnDamageHitReceived;
        }

        private void OnDisable()
        {
            _combatSystem.OnBlockedHitReceived -= OnBlockedHitReceived;
            _combatSystem.OnDamageHitReceived -= OnDamageHitReceived;
        }

        #endregion

        #region Methods

        private void StartKnockback(Attack attack, CombatSystem attackerCombatSystem)
        {
            if(_knockbackCr != null) return;

            _knockbackCr = StartCoroutine(KnockbackCoroutine(attack,attackerCombatSystem));
            
            OnStartKnockback?.Invoke();
            OnBlockEvent?.Invoke(actionBlocks);
        }

        private void StopKnockback()
        {
            if(_knockbackCr == null) return;
            
            StopCoroutine(_knockbackCr);
            _knockbackCr = null;
            
            OnStopKnockback?.Invoke();
            OnUnblockEvent?.Invoke(actionBlocks);
        }

        #endregion

        #region Event handlers

        private void OnBlockedHitReceived(Attack attack, CombatSystem attackerCombatSystem, Vector3 contactPoint)
        {
            if(!_knockbackLock.CanDoAction()) return;
            
            if(attack.GetData().DealKnockbackWhenBlocked) StartKnockback(attack,attackerCombatSystem);
        }

        private void OnDamageHitReceived(Attack attack, CombatSystem attackerCombatSystem, Vector3 contactPoint)
        {
            if(!_knockbackLock.CanDoAction()) return;
            
            if(attack.GetData().DealsKnockback) StartKnockback(attack,attackerCombatSystem);
        }

        #endregion

        #region Coroutines

        private IEnumerator KnockbackCoroutine(Attack attack, CombatSystem attackerCombatSystem)
        {
            float timer = 0.0f;
            float a = attack.GetData().KnockbackSize /
                      (0.5f * attack.GetData().KnockbackDuration * attack.GetData().KnockbackDuration);
            
            _rigidbody.velocity = Vector3.zero;
            Vector3 knockbackDirection = Vector3
                .ProjectOnPlane((_combatSystem.transform.position - attackerCombatSystem.transform.position),
                    new Vector3(0f, 1f, 0f)).normalized;

            while (timer < attack.GetData().KnockbackDuration)
            {
                _rigidbody.velocity = knockbackDirection * (a * timer);
                
                timer += Time.fixedDeltaTime;
            
                yield return new WaitForFixedUpdate();
            }

            _rigidbody.velocity = Vector3.zero;
            
            StopKnockback();
        }

        #endregion
        
        #region Interfaces

        public event Action<UnitActionsBlockManager.UnitAction[]> OnBlockEvent;
        public event Action<UnitActionsBlockManager.UnitAction[]> OnUnblockEvent;

        public void StartTactic(Tactic newTactic)
        {
            if(!newTactic.GetData().ImmuneToKnockback) return;
            
            _knockbackLock.AddLock();
        }

        public void EndTactic(Tactic oldTactic)
        {
            if(!oldTactic.GetData().ImmuneToKnockback) return;
            
            _knockbackLock.RemoveLock();
        }
        
        #endregion
    }
}