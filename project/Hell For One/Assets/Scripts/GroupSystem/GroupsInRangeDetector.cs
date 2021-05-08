﻿using System;
using System.Collections.Generic;
using System.Linq;
using ActionsBlockSystem;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.Interfaces;
using ReincarnationSystem;
using UnityEngine;

namespace GroupSystem
{ 
    public class GroupsInRangeDetector : MonoBehaviour, IReincarnationObserver, IHitPointsObserver
    {
        #region Fields

        [SerializeField] private float orderRadius = 1.0f;

        private SphereCollider _sphereCollider;
        
        private readonly Dictionary<GroupManager.Group, List<Transform>> _impsInRange = new Dictionary<GroupManager.Group, List<Transform>>();

        private static GroupManager.Group _mostRepresentedGroupInRange = GroupManager.Group.None;

        private readonly ActionLock _detectionLock = new ActionLock();
        
        #endregion

        #region Event

        public static event Action<GroupManager.Group> OnMostRepresentedGroupChanged;

        #endregion
        
        #region Properties

        public static GroupManager.Group MostRepresentedGroupInRange
        {
            get => _mostRepresentedGroupInRange; 
            private set => _mostRepresentedGroupInRange = value;
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _sphereCollider = GetComponent<SphereCollider>();
            
            _detectionLock.AddLock();

            _mostRepresentedGroupInRange = GroupManager.Group.None;
        }

        private void OnEnable()
        {
            ImpDeath.OnImpDeath += OnImpDeath;
            UnitDestroyer.OnPreImpDestroyed += OnPreImpDestroyed;
        }

        private void OnDisable()
        {
            ImpDeath.OnImpDeath -= OnImpDeath;
            UnitDestroyer.OnPreImpDestroyed -= OnPreImpDestroyed;
        }

        private void OnTriggerEnter(Collider other) => AddImp(other.transform.root);

        private void OnTriggerExit(Collider other) => RemoveImp(other.transform.root);

        #endregion

        #region Methods

        private void UpdateMostRepresentedGroup()
        {
            // Only leader will update represented groups
            if(!_detectionLock.CanDoAction()) return;

            if (_impsInRange.All(item => item.Value.Count == 0))
            {
                _mostRepresentedGroupInRange = GroupManager.Group.None;
                OnMostRepresentedGroupChanged?.Invoke(_mostRepresentedGroupInRange);
                
                return;
            }

            GroupManager.Group newGroup =
                _impsInRange.OrderByDescending(item => item.Value.Count).FirstOrDefault().Key;
            
            if(newGroup == _mostRepresentedGroupInRange) return;

            _mostRepresentedGroupInRange = newGroup;
            
            OnMostRepresentedGroupChanged?.Invoke(_mostRepresentedGroupInRange);
        }

        private void AddImp(Transform imp)
        {
            GroupManager impGroup = imp.GetComponent<GroupFinder>().Group;
            
            if(impGroup == null) return;
            
            if(!_impsInRange.ContainsKey(impGroup.ThisGroupName))
                _impsInRange.Add(impGroup.ThisGroupName, new List<Transform>());

            if(!_impsInRange[impGroup.ThisGroupName].Contains(imp))
                _impsInRange[impGroup.ThisGroupName].Add(imp);
            
            UpdateMostRepresentedGroup();
        }

        private void RemoveImp(Transform imp)
        {
            EnsureCleanData();
            
            GroupManager impGroup = imp.GetComponent<GroupFinder>().Group;
            
            if(impGroup == null) return;
            
            if(!_impsInRange.ContainsKey(impGroup.ThisGroupName)) return;

            if(!_impsInRange[impGroup.ThisGroupName].Contains(imp)) return;
                
            _impsInRange[impGroup.ThisGroupName].Remove(imp);
            
            UpdateMostRepresentedGroup();
        }

        private void EnsureCleanData()
        {
            foreach (var keyValuePair in _impsInRange)
            {
                foreach (var impTransform in keyValuePair.Value.Where(t => t == null))
                {
                    keyValuePair.Value.Remove(impTransform);
                }
            }
            
            UpdateMostRepresentedGroup();
        }

        #endregion

        #region Event Handlers

        private void OnImpDeath(Transform obj) => RemoveImp(obj.transform.root);

        private void OnPreImpDestroyed(Transform obj) => RemoveImp(obj.transform.root);

        #endregion
        
        #region Interfaces
        
        public void StartLeader()
        {
            //_sphereCollider.radius = orderRadius;
            
            transform.localScale = Vector3.one * orderRadius; 
            
            _detectionLock.RemoveLock();
        }

        public void StopLeader() => transform.localScale = Vector3.zero;//_sphereCollider.radius = 0f;

        public void OnZeroHp() => transform.localScale = Vector3.zero; //_sphereCollider.radius = 0f;

        #endregion
    }
}
