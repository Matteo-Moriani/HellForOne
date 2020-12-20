using System;
using TacticsSystem;
using UnityEngine;

namespace ReincarnationSystem
{
    public class ImpPropBehavior : MonoBehaviour, IReincarnationObserver
    {
        private MeshRenderer[] _renderers;
        private ImpRecruitBehaviour _impRecruitBehaviour;
        
        private void Awake()
        {
            _renderers = GetComponentsInChildren<MeshRenderer>();
            _impRecruitBehaviour = transform.root.GetComponent<ImpRecruitBehaviour>();
        }

        private void OnEnable()
        {
            _impRecruitBehaviour.OnStartRecruit += OnStartRecruit;
            _impRecruitBehaviour.OnStopRecruit += OnStopRecruit;
        }
        
        private void OnDisable()
        {
            _impRecruitBehaviour.OnStartRecruit -= OnStartRecruit;
            _impRecruitBehaviour.OnStopRecruit -= OnStopRecruit;
        }

        private void OnStartRecruit()
        {
            foreach (MeshRenderer meshRenderer in _renderers)
            {
                meshRenderer.enabled = false;
            }
        }

        private void OnStopRecruit()
        {
            foreach (MeshRenderer meshRenderer in _renderers)
            {
                meshRenderer.enabled = true;
            }
        }

        public void StartLeader()
        {
            foreach (MeshRenderer meshRenderer in _renderers)
            {
                meshRenderer.enabled = false;
            }
        }

        public void StopLeader()
        {
        }    
    }
}