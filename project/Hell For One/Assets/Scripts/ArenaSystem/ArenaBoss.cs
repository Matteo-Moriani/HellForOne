using System;
using FactoryBasedCombatSystem.Interfaces;
using UnityEngine;

namespace ArenaSystem
{
    public class ArenaBoss : MonoBehaviour, IDeathObserver
    {
        [SerializeField] private ArenaManager arenaManager;
        
        public event Action OnBossDeath;

        public ArenaManager Arena
        {
            get => arenaManager;
            private set => arenaManager = value;
        }

        void IDeathObserver.OnDeath() => OnBossDeath?.Invoke();
    }
}