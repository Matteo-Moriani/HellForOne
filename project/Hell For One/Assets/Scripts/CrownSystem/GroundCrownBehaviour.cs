using System;
using ReincarnationSystem;
using UnityEngine;

namespace CrownSystem
{
    public class GroundCrownBehaviour : MonoBehaviour
    { 
        private void OnTriggerEnter(Collider other)
        {
            ReincarnableBehaviour reincarnableBehaviour = other.GetComponent<ReincarnableBehaviour>();
            
            if(reincarnableBehaviour == null) return;
            
            if(ReincarnationManager.Instance.CurrentLeader != null && ReincarnationManager.Instance.CurrentLeader != reincarnableBehaviour) return;
            
            reincarnableBehaviour.GetComponentInChildren<CrownBehaviour>(true).gameObject.SetActive(true);
            
            gameObject.SetActive(false);
        }
    }
}