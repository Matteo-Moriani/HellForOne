using System;
using FactoryBasedCombatSystem.Interfaces;
using UnityEngine;

namespace ArenaSystem
{
    public class ArenaBoss : MonoBehaviour, IHitPointsObserver
    {
        [SerializeField] private ArenaManager arenaManager;
        
        public event Action OnBossDeath;
        public event Action OnIgniImoragDeath;

        public ArenaManager Arena
        {
            get => arenaManager;
            private set => arenaManager = value;
        }

        void IHitPointsObserver.OnZeroHp()
        {
            OnBossDeath?.Invoke();
            OnIgniImoragDeath?.Invoke();
        }
    }
}