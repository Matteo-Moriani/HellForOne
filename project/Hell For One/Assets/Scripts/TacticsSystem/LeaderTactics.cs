using System;
using System.Collections;
using ActionsBlockSystem;
using AggroSystem;
using CrownSystem;
using FactoryBasedCombatSystem.Interfaces;
using GroupSystem;
using Player;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace TacticsSystem
{
    public class LeaderTactics : MonoBehaviour, IActionsBlockObserver, IPlayerAggroSubject, ICrownObserver, IHitPointsObserver
    {
        #region Fields

        [SerializeField] private float singleTacticAggro;
        [SerializeField] private float globalTacticAggro;

        [SerializeField] private float heldDownTime;
        
        [SerializeField] private TacticFactory aButtonTactic;
        [SerializeField] private TacticFactory bButtonTactic;
        [SerializeField] private TacticFactory xButtonTactic;
        [SerializeField] private TacticFactory yButtonTactic;

        private GroupManager.Group _currentMostRepresentedGroup;

        private readonly ActionLock _orderAssignLock = new ActionLock();

        private Coroutine _heldDownCr = null;
        
        #endregion

        #region Delegates and events
    
        public static event Action<TacticFactory,GroupManager.Group> OnTryTacticAssign;

        #endregion

        #region Methods
        
        private void AssignOrder(TacticFactory tactic)
        {
            if (!_orderAssignLock.CanDoAction()) 
                return;

            if (GroupsInRangeDetector.MostRepresentedGroupInRange == GroupManager.Group.None ||
                GroupsInRangeDetector.MostRepresentedGroupInRange == GroupManager.Group.All) 
                return;

            OnAggroActionDone?.Invoke(singleTacticAggro);
            OnTryTacticAssign?.Invoke(tactic, GroupsInRangeDetector.MostRepresentedGroupInRange);
        }

        private void AssignGlobalOrder(TacticFactory tacticFactory)
        {
            if (!_orderAssignLock.CanDoAction()) 
                return;
            
            OnAggroActionDone?.Invoke(globalTacticAggro);
            OnTryTacticAssign?.Invoke(tacticFactory, GroupManager.Group.All);
        }

        private void StartHeldDown(TacticFactory tacticFactory)
        {
            AssignOrder(tacticFactory);
            
            if(_heldDownCr != null) return;

            _heldDownCr = StartCoroutine(HeldDownCoroutine(tacticFactory));
        }

        private void StopHeldDown()
        {
            if (_heldDownCr == null) return;
            
            StopCoroutine(_heldDownCr);
            _heldDownCr = null;
        }

        #endregion

        #region Events handler

        private void OnYButtonDown() => StartHeldDown(yButtonTactic);

        private void OnXButtonDown() => StartHeldDown(xButtonTactic);

        private void OnBButtonDown() => StartHeldDown(bButtonTactic);

        private void OnAButtonDown() => StartHeldDown(aButtonTactic);

        private void OnAButtonUp() => StopHeldDown();

        private void OnBButtonUp() => StopHeldDown();

        private void OnXButtonUp() => StopHeldDown();

        private void OnYButtonUp() => StopHeldDown();
        
        private void OnLTButtonDown() => _orderAssignLock.AddLock();

        private void OnLTButtonUp() => _orderAssignLock.RemoveLock();

        #endregion

        #region Coroutines

        private IEnumerator HeldDownCoroutine(TacticFactory tacticFactory)
        {
            yield return new WaitForSeconds(heldDownTime);
            AssignGlobalOrder(tacticFactory);
        }

        #endregion
        
        #region Interfaces

        public event Action<float> OnAggroActionDone;
        
        public void Block() => _orderAssignLock.AddLock();

        public void Unblock() => _orderAssignLock.RemoveLock();

        public UnitActionsBlockManager.UnitAction GetAction() => UnitActionsBlockManager.UnitAction.GiveOrders;

        public void OnCrownCollected()
        {
            PlayerInput.OnYButtonDown += OnYButtonDown;
            PlayerInput.OnXButtonDown += OnXButtonDown;
            PlayerInput.OnBButtonDown += OnBButtonDown;
            PlayerInput.OnAButtonDown += OnAButtonDown;
            
            PlayerInput.OnYButtonUp += OnYButtonUp;
            PlayerInput.OnXButtonUp += OnXButtonUp;
            PlayerInput.OnBButtonUp += OnBButtonUp;
            PlayerInput.OnAButtonUp += OnAButtonUp;
            
            PlayerInput.OnLTButtonDown -= OnLTButtonDown;
            PlayerInput.OnLTButtonUp -= OnLTButtonUp;
        }

        public void OnCrownLost()
        {
            PlayerInput.OnYButtonDown -= OnYButtonDown;
            PlayerInput.OnYButtonDown -= OnXButtonDown;
            PlayerInput.OnYButtonDown -= OnBButtonDown;
            PlayerInput.OnYButtonDown -= OnAButtonDown;
            
            PlayerInput.OnYButtonUp -= OnYButtonUp;
            PlayerInput.OnXButtonUp -= OnXButtonUp;
            PlayerInput.OnBButtonUp -= OnBButtonUp;
            PlayerInput.OnAButtonUp -= OnAButtonUp;

            PlayerInput.OnLTButtonDown -= OnLTButtonDown;
            PlayerInput.OnLTButtonUp -= OnLTButtonUp;
        }
        
        public void OnZeroHp() => StopAllCoroutines();
        
        #endregion
    }
}
