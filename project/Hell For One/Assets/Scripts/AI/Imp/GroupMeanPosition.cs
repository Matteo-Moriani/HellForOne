﻿using System;
using System.Linq;
using GroupSystem;
using UnityEngine;

namespace AI.Imp
{
    public class GroupMeanPosition : MonoBehaviour
    {
        private GroupManager _groupManager;

        private Vector3 _groupMeanPosition = Vector3.zero;
        private Vector3 _groupMeanDirection = Vector3.zero;

        public Vector3 MeanPosition
        {
            get => _groupMeanPosition;
            private set => _groupMeanPosition = value;
        }

        private void Awake()
        {
            _groupManager = GetComponent<GroupManager>();
        }

        private void Update()
        {
            _groupMeanPosition = Vector3.zero;
            _groupMeanDirection = Vector3.zero;
         
            if(_groupManager.Imps.Count == 0) return;
            
            foreach (Transform imp in _groupManager.Imps.Keys)
            {
                _groupMeanPosition += imp.transform.position;
                _groupMeanDirection += imp.forward;
            }

            _groupMeanPosition /= _groupManager.Imps.Count;
            _groupMeanDirection /= _groupManager.Imps.Count;

            transform.position = _groupMeanPosition;
            transform.rotation = Quaternion.LookRotation(_groupMeanDirection);
        }
    }
}