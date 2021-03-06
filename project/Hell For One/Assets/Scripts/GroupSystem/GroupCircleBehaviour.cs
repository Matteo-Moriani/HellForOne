﻿using System;
using AI.Imp;
using ReincarnationSystem;
using UnityEngine;

namespace GroupSystem
{
    public class GroupCircleBehaviour : MonoBehaviour, IGroupObserver
    {
        private MeshRenderer[] _renderers;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<MeshRenderer>();   
        }

        public void JoinGroup(GroupManager groupManager)
        {
            foreach (var meshRenderer in _renderers)
            {
                meshRenderer.material = groupManager.groupColorMat;
            }
        }

        public void LeaveGroup(GroupManager groupManager)
        {
            foreach (var meshRenderer in _renderers)
            {
                meshRenderer.enabled = false;
            }
        }
    }
}
