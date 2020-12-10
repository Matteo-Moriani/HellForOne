using System;
using UnityEngine;

namespace Animations
{
    public class AnimationEventsHooks : MonoBehaviour
    {
        public event Action OnAttackAnimationActivateAttack;
        public event Action OnAttackAnimationDeactivateAttack;

        public void OnAttackAnimationStart() => OnAttackAnimationActivateAttack?.Invoke();
        public void OnAttackAnimationEnd() => OnAttackAnimationDeactivateAttack?.Invoke();
    }
}
