using System;
using System.Collections.Generic;
using AI.Movement;
using GroupSystem;
using UnityEngine;

namespace AI.Imp
{
    public class InPositionCondition : MonoBehaviour
    {
        private GroupManager _groupManager;

        private readonly List<ContextGroupFormation> _contextGroupFormations = new List<ContextGroupFormation>();

        public event Action OnImpsInPosition; 
        
        private void Awake()
        {
            _groupManager = transform.root.GetComponent<GroupManager>();
        }

        private void OnEnable()
        {
            _groupManager.OnImpJoined += OnImpJoined;
            _groupManager.OnImpRemoved += OnImpRemoved;
        }
        
        private void OnDisable()
        {
            _groupManager.OnImpJoined -= OnImpJoined;
            _groupManager.OnImpRemoved -= OnImpRemoved;
        }

        private void Update()
        {
            if (_contextGroupFormations.TrueForAll(item => item.InPosition()))
                OnImpsInPosition?.Invoke();
        }

        private void OnImpJoined(GroupManager arg1, GameObject impJoined) => _contextGroupFormations.Add(impJoined.GetComponent<ContextGroupFormation>());

        private void OnImpRemoved(GameObject impRemoved) => _contextGroupFormations.Remove(impRemoved.GetComponent<ContextGroupFormation>());
    }
}