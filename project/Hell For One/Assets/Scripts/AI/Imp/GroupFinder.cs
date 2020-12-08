using System;
using System.Collections;
using Groups;
using UnityEngine;

// TODO - Merge this into GroupManager?
namespace AI.Imp
{
    public class GroupFinder : MonoBehaviour
    {
        #region Fields

        private Stats stats;

        private Reincarnation reincarnation;
    
        private GameObject groupBelongingTo;

        #endregion

        #region Properties

        public GameObject GroupBelongingTo
        {
            get => groupBelongingTo;
            private set => groupBelongingTo = value;
        }

        #endregion

        #region Delegates and Events
        
        public event Action<Transform> OnGroupFound;
        
        #endregion

        #region Unity methods

        private void Awake()
        {
            stats = GetComponent<Stats>();
            reincarnation = GetComponent<Reincarnation>();
        }

        private void Start()
        {
            FindGroup();
        }

        private void OnEnable()
        {
            stats.onDeath += OnDeath;
            reincarnation.onReincarnation += OnReincarnation;
        }

        private void OnDisable()
        {
            stats.onDeath -= OnDeath;
            reincarnation.onReincarnation -= OnReincarnation;
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
            
            groupBelongingTo = bestGroup.gameObject;
            gameObject.GetComponent<ChildrenObjectsManager>().ActivateCircle();
            OnGroupFound?.Invoke(groupBelongingTo.transform);
        }

        #endregion

        #region Event handlers

        private void OnReincarnation(GameObject newplayer)
        {
            enabled = false;
            StopAllCoroutines();
        }

        private void OnDeath(Stats sender)
        {
            enabled = false;
            StopAllCoroutines();
        }

        #endregion
    }
}
