using System;
using FactoryBasedCombatSystem.Interfaces;
using UnityEngine;

namespace GroupSystem
{
    public class GroupFinder : MonoBehaviour
    {
        #region Fields

        private GroupManager _group;

        #endregion

        #region Properties

        public GroupManager Group
        {
            get => _group;
            private set => _group = value;
        }

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
            
            _group = bestGroup;
        }

        #endregion
    }
}
