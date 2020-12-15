using System;
using System.Collections;
using System.Collections.Generic;
using ActionsBlockSystem;
using ArenaSystem;
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
        private static float _singleSegmentPool = 50f;
        private static float _manaRechargeRate = 10f;

        private static float _currentManaPool = 45f;
        private static int _currentChargedSegments;

        private Coroutine _manaRechargeCr = null;
    
        private readonly ActionLock _manaRechargeLock = new ActionLock();
        
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
        }

        private void OnEnable()
        {
            ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        }

        private void OnDisable()
        {
            ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;
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

            return;
        }

        #endregion

        #region External events handlers

        private void OnGlobalStartBattle(ArenaManager arenaManager) => _manaRechargeLock.RemoveLock();

        private void OnGlobalEndBattle(ArenaManager arenaManager) => _manaRechargeLock.AddLock();
    
        #endregion

        #region Interfaces

        public void StartLeader() => _manaRechargeCr = StartCoroutine(ManaRechargeCoroutine());

        public void StopLeader() => StopCoroutine(_manaRechargeCr);

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
                
                _currentManaPool = Mathf.Clamp(_currentManaPool + _manaRechargeRate, 0f, (_currentChargedSegments + 1) * _singleSegmentPool);
                
                OnManaPoolChanged?.Invoke(_currentManaPool);
                
                if(_currentManaPool % _singleSegmentPool != 0) continue;

                Mathf.Clamp(_currentChargedSegments++,0f,_maxSegments);
                
                OnSegmentCharged?.Invoke(_currentChargedSegments);
            }
        }

        #endregion
    }
}
