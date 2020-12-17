using System;
using FactoryBasedCombatSystem.Interfaces;
using UnityEngine;

namespace ReincarnationSystem
{
    public class Reincarnation : MonoBehaviour, IHitPointsObserver
    {
        private IReincarnationObserver[] _reincarnationObservers;

        public event Action<Reincarnation> OnReincarnableDeath; 

        private void Awake() => _reincarnationObservers = GetComponentsInChildren<IReincarnationObserver>();

        private void Start() => ReincarnationManager.Instance.RegisterReincarnable(this);

        public void OnZeroHp() => OnReincarnableDeath?.Invoke(this);

        public void GainLeadership()
        {
            foreach (IReincarnationObserver reincarnationObserver in _reincarnationObservers)
                reincarnationObserver.Reincarnate();
        }
    }
}
