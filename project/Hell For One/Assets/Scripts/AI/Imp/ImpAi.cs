using CRBT;
using GroupSystem;
using TacticsSystem.Interfaces;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace AI.Imp
{
    public class ImpAi : MonoBehaviour, IGroupObserver
    {
        private AiUtils.TargetData _currentTargetData;
        
        private TacticFactory _activeTactic = null;
        private Tactic _tacticInstance = null;

        private ITacticsObserver[] _observers;

        public AiUtils.TargetData CurrentTargetData
        {
            get => _currentTargetData;
            private set => _currentTargetData = value;
        }

        private void Awake() => _observers = GetComponentsInChildren<ITacticsObserver>();

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

        public void JoinGroup(ImpGroupAi impGroupAi) => _currentTargetData = impGroupAi.Target;
    }
}