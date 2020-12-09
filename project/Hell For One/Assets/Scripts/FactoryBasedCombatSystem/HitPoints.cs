using System;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class HitPoints : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float startingHp;

        private float _currentHp;

        public event Action OnDeath;
        
        private void Awake()
        {
            _currentHp = startingHp;
        }
    }
}