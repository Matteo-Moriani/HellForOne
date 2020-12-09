using System;
using TacticsSystem.Interfaces;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace AI.Imp
{
    public class ImpAi : MonoBehaviour
    {
        private TacticFactory _activeTactic = null;
        private Tactic _tacticInstance = null;

        private ITacticsObserver[] _observers;

        private void Awake()
        {
            _observers = GetComponentsInChildren<ITacticsObserver>();
        }

        public void ExecuteTactic(TacticFactory tactic)
        {
            if (_activeTactic == null || !_activeTactic.Equals(tactic))
            {
                _activeTactic = tactic;
                _tacticInstance = tactic.GetTactic();
                
                foreach (ITacticsObserver tacticsObserver in _observers)
                {
                    tacticsObserver.StartTactic(_tacticInstance);
                }
            }
            
            _tacticInstance.ExecuteTactic(this);
        }
    }
}