using System;
using ActionsBlockSystem;
using Groups;
using Player;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace TacticsSystem
{
    public class TacticsManager : MonoBehaviour, IActionsBlockObserver
    {
        #region Fields

        [SerializeField] private Tactic aButtonTactic;
        [SerializeField] private Tactic bButtonTactic;
        [SerializeField] private Tactic xButtonTactic;
        [SerializeField] private Tactic yButtonTactic;

        private GroupManager.Group _currentMostRepresentedGroup;

        private readonly ActionLock _orderAssignLock = new ActionLock();
    
        #endregion

        #region Delegates and events
    
        public static event Action<Tactic,GroupManager.Group> OnTryOrderAssign;

        #endregion
        
        #region Unity methods

        private void OnEnable()
        {
            PlayerInput.OnYButtonDown += OnYButtonDown;
            PlayerInput.OnXButtonDown += OnXButtonDown;
            PlayerInput.OnBButtonDown += OnBButtonDown;
            PlayerInput.OnAButtonDown += OnAButtonDown;
        }

        private void OnDisable()
        {
            PlayerInput.OnYButtonDown -= OnYButtonDown;
            PlayerInput.OnYButtonDown -= OnXButtonDown;
            PlayerInput.OnYButtonDown -= OnBButtonDown;
            PlayerInput.OnYButtonDown -= OnAButtonDown;
        }

        #endregion
        
        #region Methods
        
        private bool AssignOrder(Tactic tactic)
        {
            if (!_orderAssignLock.CanDoAction()) 
                return false;

            if (GroupsInRangeDetector.MostRappresentedGroupInRange == GroupManager.Group.None ||
                GroupsInRangeDetector.MostRappresentedGroupInRange == GroupManager.Group.All) 
                return false;

            OnTryOrderAssign?.Invoke(tactic, GroupsInRangeDetector.MostRappresentedGroupInRange);

            return true;
        }

        #endregion

        #region Events handler

        private void OnYButtonDown() => AssignOrder(yButtonTactic);

        private void OnXButtonDown() => AssignOrder(xButtonTactic);

        private void OnBButtonDown() => AssignOrder(bButtonTactic);

        private void OnAButtonDown() => AssignOrder(aButtonTactic);
        
        #endregion

        #region Interfaces

        public void Block() => _orderAssignLock.AddLock();

        public void Unblock() => _orderAssignLock.RemoveLock();

        public UnitActionsBlockManager.UnitAction GetAction() => UnitActionsBlockManager.UnitAction.GiveOrders;

        #endregion
    }
}
