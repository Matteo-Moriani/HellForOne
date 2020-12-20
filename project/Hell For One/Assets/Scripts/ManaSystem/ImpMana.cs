using System;
using System.Collections;
using System.Collections.Generic;
using ActionsBlockSystem;
using ArenaSystem;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.Interfaces;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using GroupAbilitiesSystem;
using GroupAbilitiesSystem.ScriptableObjects;
using ReincarnationSystem;
using UnityEngine;

namespace ManaSystem
{
    public class ImpMana : MonoBehaviour, IReincarnationObserver
    {
        #region Fields

        private static int _maxSegments = 2;
        private static float _singleSegmentPool = 100f;
        private static float _manaRechargeRate = 25f;

        private static float _currentManaPool = 0f;
        private static int _currentChargedSegments;

        private Coroutine _manaRechargeCr = null;
    
        private readonly ActionLock _manaRechargeLock = new ActionLock();

        private CombatSystem _combatSystem;

        #endregion

        #region Properties

        public static int MAXSegments
        {
            get => _maxSegments;
            private set => _maxSegments = value;
        }

        public static float SingleSegmentPool
        {
            get => _singleSegmentPool;
            private set => _singleSegmentPool = value;
        }

        public static int CurrentChargedSegments
        {
            get => _currentChargedSegments;
            private set => _currentChargedSegments = value;
        }

        #endregion
    
        #region Delegates and events
        
        public static event Action<int> OnSegmentCharged;
        public static event Action<float> OnManaPoolChanged;
        public static event Action<int> OnSegmentSpent;

        #endregion

        #region Unity methods

        private void Awake()
        {
            _manaRechargeLock.AddLock();

            _combatSystem = GetComponentInChildren<CombatSystem>();
        }

        private void OnEnable()
        {
            ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;

            _combatSystem.OnDamageHitDealt += OnDamageHitDealt;
        }

        private void OnDisable()
        {
            ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;
            
            _combatSystem.OnDamageHitDealt -= OnDamageHitDealt;
        }

        #endregion
    
        #region Methods

        public bool CheckSegments(int toSpend) => toSpend <= _currentChargedSegments;

        public void SpendSegments(int toSpend)
        {
            if (toSpend > _currentChargedSegments) return;

            _currentChargedSegments -= toSpend;
            _currentManaPool -= _singleSegmentPool * toSpend;
            
            OnSegmentSpent?.Invoke(toSpend);
            OnManaPoolChanged?.Invoke(_currentManaPool);
        }

        private void AddMana(float toAdd)
        {
            if(_currentChargedSegments == _maxSegments) return;
            
            _currentManaPool = Mathf.Clamp(_currentManaPool + toAdd, 0f, (_currentChargedSegments + 1) * _singleSegmentPool);
                
            OnManaPoolChanged?.Invoke(_currentManaPool);
                
            if(_currentManaPool % _singleSegmentPool != 0) return;

            Mathf.Clamp(_currentChargedSegments++,0f,_maxSegments);
                
            OnSegmentCharged?.Invoke(_currentChargedSegments);
        }

        #endregion

        #region External events handlers

        private void OnGlobalStartBattle(ArenaManager arenaManager)
        {
            _manaRechargeLock.RemoveLock();
            
            _currentManaPool = 0f;
            _currentChargedSegments = 0;
            
            OnManaPoolChanged?.Invoke(_currentManaPool);
            OnSegmentCharged?.Invoke(0);
        }

        private void OnGlobalEndBattle(ArenaManager arenaManager)
        {
            _manaRechargeLock.AddLock();
            
            _currentManaPool = 0f;
            _currentChargedSegments = 0;
            
            OnManaPoolChanged?.Invoke(_currentManaPool);
            OnSegmentCharged?.Invoke(0);
        }

        private void OnDamageHitDealt(Attack arg1, CombatSystem arg2, Vector3 arg3) => AddMana(arg1.GetData().Damage);

        #endregion

        #region Interfaces

        public void StartLeader() => _manaRechargeCr = StartCoroutine(ManaRechargeCoroutine());

        public void StopLeader()
        {
            if(_manaRechargeCr == null) return;
            
            StopCoroutine(_manaRechargeCr);

            _manaRechargeCr = null;
        }

        #endregion
        
        #region Coroutines

        private IEnumerator ManaRechargeCoroutine()
        {
            float timer = 0f;
            
            while (true)
            {
                yield return null;
                
                if(!_manaRechargeLock.CanDoAction()) continue;
                
                if(_currentChargedSegments == _maxSegments) continue;

                timer += Time.deltaTime;
                
                if(!(timer >= 1.0f)) continue;

                timer = 0f;
                
                AddMana(_manaRechargeRate);
            }
        }

        #endregion
    }
}
