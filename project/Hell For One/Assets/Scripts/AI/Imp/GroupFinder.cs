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
        private bool groupFound = false;

        #endregion

        #region Properties

        public GameObject GroupBelongingTo
        {
            get => groupBelongingTo;
            private set => groupBelongingTo = value;
        }
    
        public bool GroupFound
        {
            get => groupFound;
            private set => groupFound = value;
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
            StartCoroutine(FindGroupCoroutine());
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
            foreach ( GameObject group in GroupsManager.Instance.Groups )
            {
                GroupManager groupManager = group.GetComponent<GroupManager>();

                if (!groupManager.AddDemonToGroup(transform)) continue;
                
                groupBelongingTo = group;
                gameObject.GetComponent<ChildrenObjectsManager>().ActivateCircle();
                OnGroupFound?.Invoke(groupBelongingTo.transform);
                    
                break;
            }
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

        #region Coroutines

        private IEnumerator FindGroupCoroutine()
        {
            while (!groupFound)
            {
                FindGroup();
                yield return null;
            }
        }

        #endregion
    }
}
