using System;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;
using Utils;

namespace FactoryBasedCombatSystem
{
    public class HitboxCollider : MonoBehaviour
    {
        #region Delegates and events

        internal event Action<Attack,CombatSystem> OnBeingHit;
    
        #endregion

        #region Unity methods

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("CombatSystem")) return;

            AttackCollider attackerAttackCollider = other.GetComponent<AttackCollider>();
            
            if(attackerAttackCollider) return;

            if(attackerAttackCollider.CurrentOwner == transform.root) return;
            
            OnBeingHit?.Invoke(attackerAttackCollider.CurrentAttack,attackerAttackCollider.OwnerCombatSystem);
        }

        #endregion
    }
}