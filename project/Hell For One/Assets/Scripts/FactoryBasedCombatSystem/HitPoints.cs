using System;
using ActionsBlockSystem;
using FactoryBasedCombatSystem.Interfaces;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class HitPoints : MonoBehaviour, IActionsBlockSubject
    {
        #region Fields

        [SerializeField, Min(0f)] private float startingHp;

        [SerializeField] private UnitActionsBlockManager.UnitAction[] deathActionBlocks;
        
        private CombatSystem _combatSystem;
        
        [SerializeField] private float _currentHp;

        private IHitPointsObserver[] _observers;
        
        #endregion

        #region Events
        
        public event Action<float> OnHpChanged;
        
        #endregion

        #region Properties

        public float StartingHp
        {
            get => startingHp;
            private set => startingHp = value;
        }

        public float CurrentHp
        {
            get => _currentHp;
            private set => _currentHp = value;
        }

        #endregion

        #region Unity methods

        private void Awake()
        {
            _combatSystem = GetComponentInChildren<CombatSystem>();
            
            _currentHp = startingHp;

            _observers = GetComponentsInChildren<IHitPointsObserver>(true);
        }

        private void OnEnable() => _combatSystem.OnDamageHitReceived += OnDamageHitReceived;

        private void OnDisable() => _combatSystem.OnDamageHitReceived += OnDamageHitReceived;
        
        #endregion

        #region Methods

        private void TakeHit(float damage)
        {
            _currentHp -= Mathf.Clamp(damage, 0f, float.MaxValue);
            
            OnHpChanged?.Invoke(Mathf.Clamp(_currentHp,float.Epsilon,float.MaxValue));

            if (!(_currentHp <= 0f)) return;
            
            foreach (IHitPointsObserver observer in _observers)
            {
                observer.OnZeroHp();
                OnBlockEvent?.Invoke(deathActionBlocks);
            }
        }

        #endregion

        #region Events handlers

        private void OnDamageHitReceived(Attack attack, CombatSystem attackerCombatSystem, Vector3 contactPoint) =>
            TakeHit(attack.GetData().Damage);

        #endregion

        #region Interfaces

        public event Action<UnitActionsBlockManager.UnitAction[]> OnBlockEvent;
        public event Action<UnitActionsBlockManager.UnitAction[]> OnUnblockEvent;

        #endregion
    }
}