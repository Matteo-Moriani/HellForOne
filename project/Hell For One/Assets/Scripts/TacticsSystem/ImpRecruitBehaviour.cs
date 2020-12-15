using System;
using UnityEngine;

namespace TacticsSystem
{
    public class ImpRecruitBehaviour : MonoBehaviour
    {
        public event Action OnStartRecruit;
        public event Action OnStopRecruit;
        
        public void StartRecruit()
        {
            RecruitManager.Instance.RegisterRecruiter(this);
            
            OnStartRecruit?.Invoke();
        }

        public void StopRecruit()
        {
            RecruitManager.Instance.UnregisterRecruiter(this);
            
            OnStopRecruit?.Invoke();
        }
    }
}