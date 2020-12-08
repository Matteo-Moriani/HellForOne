using System;
using System.Linq;
using Groups;
using UnityEngine;

namespace AI.Imp
{
    public class GroupMeanPosition : MonoBehaviour
    {
        private GroupManager _groupManager;

        private Vector3 _groupMeanPosition = Vector3.zero;

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
            
            foreach (Transform imp in _groupManager.Imps.Keys)
            {
                _groupMeanPosition += imp.transform.position;
            }

            _groupMeanPosition /= _groupManager.Imps.Count;
        }
    }
}