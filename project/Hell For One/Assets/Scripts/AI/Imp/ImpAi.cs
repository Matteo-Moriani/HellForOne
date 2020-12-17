using CRBT;
using FactoryBasedCombatSystem.Interfaces;
using GroupSystem;
using ReincarnationSystem;
using TacticsSystem.Interfaces;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace AI.Imp
{
    public class ImpAi : MonoBehaviour, IGroupObserver, IHitPointsObserver, IReincarnationObserver
    {
        #region Fields

        private AiUtils.TargetData _currentTargetData;
        
        private TacticFactory _activeTactic = null;
        private Tactic _tacticInstance = null;

        private ITacticsObserver[] _observers;

        #endregion

        #region Properties

        public AiUtils.TargetData CurrentTargetData
        {
            get => _currentTargetData;
            private set => _currentTargetData = value;
        }

        #endregion

        #region Unity Methods

        private void Awake() => _observers = GetComponentsInChildren<ITacticsObserver>();

        #endregion

        #region Methods

        public void ExecuteTactic(TacticFactory tactic)
        {
            if (_activeTactic == null || !_activeTactic.Equals(tactic))
            {
                _activeTactic = tactic;

                StopCurrentTactic();

                _tacticInstance = tactic.GetTactic();
                
                foreach (ITacticsObserver tacticsObserver in _observers)
                {
                    tacticsObserver.StartTactic(_tacticInstance);
                }
            }
            
            _tacticInstance.ExecuteTactic(this);
        }
        
        private void StopCurrentTactic()
        {
            if(_tacticInstance == null) return;
            
            _tacticInstance.TerminateTactic(this);

            foreach (ITacticsObserver tacticsObserver in _observers)
            {
                tacticsObserver.EndTactic(_tacticInstance);
            }
        }

        #endregion

        #region Interfaces

        public void JoinGroup(GroupManager groupManager) => _currentTargetData = groupManager.GetComponent<ImpGroupAi>().Target;

        public void LeaveGroup(GroupManager groupManager)
        {
            StopCurrentTactic();
            
            _currentTargetData = null;
            _activeTactic = null;
            _tacticInstance = null;
        }

        public void OnZeroHp() => StopCurrentTactic();

        public void StartLeader() => StopCurrentTactic();

        public void StopLeader() { }

        #endregion
    }
}