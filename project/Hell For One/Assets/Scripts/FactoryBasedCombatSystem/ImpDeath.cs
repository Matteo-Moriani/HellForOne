using System;
using FactoryBasedCombatSystem.Interfaces;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class ImpDeath : MonoBehaviour, IHitPointsObserver
    {
        public static event Action<Transform> OnImpDeath;

        public void OnZeroHp() => OnImpDeath?.Invoke(transform);
    }
}