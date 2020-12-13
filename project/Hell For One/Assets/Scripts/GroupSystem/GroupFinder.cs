using System;
using FactoryBasedCombatSystem.Interfaces;
using UnityEngine;

namespace GroupSystem
{
    public class GroupFinder : MonoBehaviour, IHitPointsObserver
    {
        #region Fields

        private GroupManager _impGroup;

        #endregion

        #region Properties

        public GroupManager ImpGroup
        {
            get => _impGroup;
            private set => _impGroup = value;
        }

        #endregion

        #region Delegates and Events
        
        public event Action<Transform> OnGroupFound;
        
        #endregion

        #region Unity methods

        private void Start()
        {
            FindGroup();
        }

        #endregion

        #region Methods

        // Balances group entering too
        private void FindGroup()
        {
            int lowest = int.MaxValue;
            GroupManager bestGroup = null;
                
            foreach ( GameObject group in GroupsManager.Instance.Groups )
            {
                GroupManager groupManager = group.GetComponent<GroupManager>();

                if (groupManager.Imps.Keys.Count >= lowest) continue;
                
                bestGroup = groupManager;
                lowest = groupManager.Imps.Keys.Count;
            }

            if (bestGroup == null) return;
                
            bestGroup.AddDemonToGroup(transform);
            
            _impGroup = bestGroup;
            gameObject.GetComponent<ChildrenObjectsManager>().ActivateCircle();
            OnGroupFound?.Invoke(_impGroup.transform);
        }

        #endregion

        #region Interfaces

        public void OnZeroHp() => _impGroup.RemoveImp(transform);

        #endregion
    }
}
