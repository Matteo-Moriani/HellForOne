using System;
using ArenaSystem;
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
        public Tactic TacticInstance { get => _tacticInstance; set => _tacticInstance = value; }

        #endregion

        #region Unity Methods

        private void Awake() => _observers = GetComponentsInChildren<ITacticsObserver>();

        private void OnEnable() => ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;

        #endregion

        #region Methods

        public void ExecuteTactic(TacticFactory tactic)
        {
            if (_activeTactic == null || !_activeTactic.Equals(tactic))
            {
                _activeTactic = tactic;

                StopCurrentTactic();

                TacticInstance = tactic.GetTactic();
                
                foreach (ITacticsObserver tacticsObserver in _observers)
                {
                    tacticsObserver.StartTactic(TacticInstance);
                }
            }
            
            TacticInstance.ExecuteTactic(this);
        }
        
        private void StopCurrentTactic()
        {
            if(TacticInstance == null) return;
            
            TacticInstance.TerminateTactic(this);

            foreach (ITacticsObserver tacticsObserver in _observers)
            {
                tacticsObserver.EndTactic(TacticInstance);
            }
        }

        #endregion

        #region Event Handlers

        private void OnGlobalEndBattle(ArenaManager obj) => StopCurrentTactic();

        #endregion
        
        #region Interfaces

        public void JoinGroup(GroupManager groupManager) => _currentTargetData = groupManager.GetComponent<ImpGroupAi>().Target;

        public void LeaveGroup(GroupManager groupManager)
        {
            StopCurrentTactic();
            
            _currentTargetData = null;
            _activeTactic = null;
            TacticInstance = null;
        }

        public void OnZeroHp() => StopCurrentTactic();

        public void StartLeader() => StopCurrentTactic();

        public void StopLeader() { }

        #endregion
    }
}