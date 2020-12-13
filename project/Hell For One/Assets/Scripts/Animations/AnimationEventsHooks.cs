using System;
using UnityEngine;

namespace Animations
{
    public class AnimationEventsHooks : MonoBehaviour
    {
        public event Action OnAttackAnimationStart;
        public event Action OnAttackAnimationEnd;
        public event Action OnDeathAnimationEnd;

        public void RaiseOnAttackAnimationStart() => OnAttackAnimationStart?.Invoke();
        public void RaiseOnAttackAnimationEnd() => OnAttackAnimationEnd?.Invoke();
        public void RaiseOnDeathAnimationEnd() => OnDeathAnimationEnd?.Invoke();
    }
}
