using UnityEngine;

namespace ReincarnationSystem
{
    public class LayerReincarnation : MonoBehaviour, IReincarnationObserver
    {
        public void StartLeader() => transform.root.gameObject.layer = LayerMask.NameToLayer("Player");

        public void StopLeader() { }
    }
}