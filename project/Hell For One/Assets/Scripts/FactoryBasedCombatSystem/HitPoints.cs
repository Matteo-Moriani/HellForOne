using System;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class HitPoints : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float startingHp;

        private CombatSystem _combatSystem;
        
        private float _currentHp;

        public event Action OnZeroHp;
        public event Action<float> OnHpChanged;

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

        private void Awake()
        {
            _combatSystem = GetComponentInChildren<CombatSystem>();
            
            _currentHp = startingHp;
        }

        private void OnEnable() => _combatSystem.OnDamageHitReceived += OnDamageHitReceived;

        private void OnDisable() => _combatSystem.OnDamageHitReceived += OnDamageHitReceived;

        private void TakeHit(float damage)
        {
            _currentHp -= Mathf.Clamp(damage, 0f, float.MaxValue);
            
            OnHpChanged?.Invoke(Mathf.Clamp(_currentHp,0f,float.MaxValue));
            
            if(_currentHp <= 0)
                OnZeroHp?.Invoke();
        }

        private void OnDamageHitReceived(Attack attack, CombatSystem attackerCombatSystem, Vector3 contactPoint) =>
            TakeHit(attack.GetData().Damage);
    }
}