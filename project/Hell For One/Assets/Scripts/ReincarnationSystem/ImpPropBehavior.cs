using UnityEngine;

namespace ReincarnationSystem
{
    public class ImpPropBehavior : MonoBehaviour, IReincarnationObserver
    {
        private MeshRenderer[] _renderers;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<MeshRenderer>();
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