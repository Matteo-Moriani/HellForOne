using System;
using ActionsBlockSystem;
using OrdersSystem.ScriptableObjects;
using Player;
using UnityEngine;

namespace OrdersSystem
{
    public class TacticsManager : MonoBehaviour, IActionsBlockObserver
    {
        #region Fields

        [SerializeField] private Order aButtonOrder;
        [SerializeField] private Order bButtonOrder;
        [SerializeField] private Order xButtonOrder;
        [SerializeField] private Order yButtonOrder;

        private GroupManager.Group _currentMostRepresentedGroup;

        private readonly ActionLock _orderAssignLock = new ActionLock();
    
        #endregion

        #region Delegates and events
    
        public static event Action<Order,GroupManager.Group> OnTryOrderAssign;

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
        
        private bool AssignOrder(Order order)
        {
            if (!_orderAssignLock.CanDoAction()) 
                return false;

            if (GroupsInRangeDetector.MostRappresentedGroupInRange == GroupManager.Group.None ||
                GroupsInRangeDetector.MostRappresentedGroupInRange == GroupManager.Group.All) 
                return false;

            OnTryOrderAssign?.Invoke(order, GroupsInRangeDetector.MostRappresentedGroupInRange);

            return true;
        }

        #endregion

        #region Events handler

        private void OnYButtonDown() => AssignOrder(yButtonOrder);

        private void OnXButtonDown() => AssignOrder(xButtonOrder);

        private void OnBButtonDown() => AssignOrder(bButtonOrder);

        private void OnAButtonDown() => AssignOrder(aButtonOrder);
        
        #endregion

        public void Block() => _orderAssignLock.AddLock();

        public void Unblock() => _orderAssignLock.RemoveLock();

        public UnitActionsBlockManager.UnitAction GetAction() => UnitActionsBlockManager.UnitAction.GiveOrders;
    }
}
