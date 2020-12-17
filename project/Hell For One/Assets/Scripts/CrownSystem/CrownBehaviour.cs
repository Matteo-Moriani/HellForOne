using System;
using FactoryBasedCombatSystem.Interfaces;
using UnityEngine;

namespace CrownSystem
{
    public class CrownBehaviour : MonoBehaviour, IHitPointsObserver
    {
        private ICrownObserver[] _crownObservers;

        private void Awake() => _crownObservers = transform.root.GetComponentsInChildren<ICrownObserver>();

        private void OnEnable()
        {
            foreach (ICrownObserver crownObserver in _crownObservers)
            {
                crownObserver.OnCrownCollected();
            }
        }

        private void OnDisable()
        {
            foreach (ICrownObserver crownObserver in _crownObservers)
            {
                crownObserver.OnCrownLost();
            }
        }

        public void OnZeroHp() => gameObject.SetActive(false);
    }
}