using System;
using FactoryBasedCombatSystem.Interfaces;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class ImpDeath : MonoBehaviour, IHitPointsObserver
    {
        #region Events

        public static event Action<Transform> OnImpDeath;

        #endregion

        #region Interfaces

        public void OnZeroHp() => OnImpDeath?.Invoke(transform);

        #endregion
    }
}