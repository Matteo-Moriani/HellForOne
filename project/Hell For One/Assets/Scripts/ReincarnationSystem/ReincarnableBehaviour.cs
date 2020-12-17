using System;
using FactoryBasedCombatSystem.Interfaces;
using UnityEngine;

namespace ReincarnationSystem
{
    public class ReincarnableBehaviour : MonoBehaviour, IHitPointsObserver
    {
        private IReincarnationObserver[] _reincarnationObservers;

        public event Action<ReincarnableBehaviour> OnReincarnableDeath; 

        private void Awake() => _reincarnationObservers = GetComponentsInChildren<IReincarnationObserver>();

        private void Start() => ReincarnationManager.Instance.RegisterReincarnable(this);

        public void OnZeroHp() => OnReincarnableDeath?.Invoke(this);

        public void GainLeadership()
        {
            foreach (IReincarnationObserver reincarnationObserver in _reincarnationObservers)
                reincarnationObserver.StartLeader();
        }
        
        public void LoseLeadership()
        {
            foreach (IReincarnationObserver reincarnationObserver in _reincarnationObservers)
                reincarnationObserver.StopLeader();
        }
    }
}
