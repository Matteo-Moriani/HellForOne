using System;
using AI.Imp;
using ReincarnationSystem;
using UnityEngine;

namespace GroupSystem
{
    public class GroupCircleBehaviour : MonoBehaviour, IGroupObserver, IReincarnationObserver
    {
        private MeshRenderer[] _renderers;

        private void Awake() => _renderers = GetComponentsInChildren<MeshRenderer>();

        private void Start()
        {
            foreach (var meshRenderer in _renderers)
            {
                meshRenderer.enabled = false;
            }
        }

        public void JoinGroup(ImpGroupAi impGroupAi)
        {
            foreach (var meshRenderer in _renderers)
            {
                meshRenderer.enabled = true;
                meshRenderer.material = impGroupAi.GetComponent<GroupManager>().groupColorMat;
            }
        }

        public void StartLeader()
        {
            foreach (var meshRenderer in _renderers)
            {
                meshRenderer.enabled = false;
            }
        }

        public void StopLeader() { }
    }
}
