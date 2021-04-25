using System;
using System.Collections;
using ActionsBlockSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class Stun : MonoBehaviour, IActionsBlockSubject
    {
        [SerializeField] private UnitActionsBlockManager.UnitAction[] actionBlocks;

        private CombatSystem _combatSystem;

        private Coroutine _stunCoroutine;

        public event Action OnStartStun;
        public event Action OnStopStun;
        
        private void Awake() => _combatSystem = GetComponent<CombatSystem>();

        private void OnEnable()
        {
            _combatSystem.OnBlockedHitReceived += OnBlockedHitReceived;
            _combatSystem.OnDamageHitReceived += OnDamageHitReceived;
        }

        private void OnDisable()
        {
            _combatSystem.OnBlockedHitReceived += OnBlockedHitReceived;
            _combatSystem.OnDamageHitReceived += OnDamageHitReceived;
        }

        private void StartStun(float stunTime)
        {
            if(_stunCoroutine != null) return;

            _stunCoroutine = StartCoroutine(StunCoroutine(stunTime));
            
            OnStartStun?.Invoke();
            OnBlockEvent?.Invoke(actionBlocks);
            
            //Debug.Log("Start stun " + transform.root.name);
        }

        private void StopStun()
        {
            if(_stunCoroutine == null) return;
            
            StopCoroutine(_stunCoroutine);
            _stunCoroutine = null;
            
            OnStopStun?.Invoke();
            OnUnblockEvent?.Invoke(actionBlocks);
            
            Debug.Log("Stop stun " + transform.root.name);
        }

        private void OnBlockedHitReceived(Attack arg1, CombatSystem arg2, Vector3 arg3)
        {
            if(!arg1.GetData().DealsStunWhenBlocked) return;
            
            StartStun(arg1.GetData().StunTime);
        }

        private void OnDamageHitReceived(Attack arg1, CombatSystem arg2, Vector3 arg3)
        {
            if(!arg1.GetData().DealsStun) return;
            
            StartStun(arg1.GetData().StunTime);
        }

        private IEnumerator StunCoroutine(float stunTime)
        {
            yield return new WaitForSeconds(stunTime);
            
            StopStun();
        }

        public event Action<UnitActionsBlockManager.UnitAction[]> OnBlockEvent;
        public event Action<UnitActionsBlockManager.UnitAction[]> OnUnblockEvent;
    }
}