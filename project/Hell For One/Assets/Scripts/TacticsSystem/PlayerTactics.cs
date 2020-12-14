using System;
using System.Collections;
using ActionsBlockSystem;
using AggroSystem;
using GroupSystem;
using Player;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace TacticsSystem
{
    // TODO :- ICrownObserver
    public class PlayerTactics : MonoBehaviour, IActionsBlockObserver, IPlayerAggroSubject
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
    
        public static event Action<TacticFactory,GroupManager.Group> OnTryOrderAssign;

        #endregion
        
        #region Unity methods

        private void OnEnable()
        {
            PlayerInput.OnYButtonDown += OnYButtonDown;
            PlayerInput.OnXButtonDown += OnXButtonDown;
            PlayerInput.OnBButtonDown += OnBButtonDown;
            PlayerInput.OnAButtonDown += OnAButtonDown;
            
            PlayerInput.OnYButtonUp += OnYButtonUp;
            PlayerInput.OnXButtonUp += OnXButtonUp;
            PlayerInput.OnBButtonUp += OnBButtonUp;
            PlayerInput.OnAButtonUp += OnAButtonUp;
        }

        private void OnDisable()
        {
            PlayerInput.OnYButtonDown -= OnYButtonDown;
            PlayerInput.OnYButtonDown -= OnXButtonDown;
            PlayerInput.OnYButtonDown -= OnBButtonDown;
            PlayerInput.OnYButtonDown -= OnAButtonDown;
            
            PlayerInput.OnYButtonUp -= OnYButtonUp;
            PlayerInput.OnXButtonUp -= OnXButtonUp;
            PlayerInput.OnBButtonUp -= OnBButtonUp;
            PlayerInput.OnAButtonUp -= OnAButtonUp;
        }

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
            OnTryOrderAssign?.Invoke(tactic, GroupsInRangeDetector.MostRepresentedGroupInRange);
        }

        private void AssignGlobalOrder(TacticFactory tacticFactory)
        {
            if (!_orderAssignLock.CanDoAction()) 
                return;
            
            OnAggroActionDone?.Invoke(globalTacticAggro);
            OnTryOrderAssign?.Invoke(tacticFactory, GroupManager.Group.All);
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

        #endregion
    }
}
