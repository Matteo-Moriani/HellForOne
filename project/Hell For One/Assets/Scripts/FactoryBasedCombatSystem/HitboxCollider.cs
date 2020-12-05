using System;
using ActionsBlockSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class HitboxCollider : MonoBehaviour, IActionsBlockObserver
    {
        #region Fields

        private readonly ActionLock _hitboxLock = new ActionLock();
        private Collider _collider;

        #endregion
        
        #region Delegates and events

        internal event Action<int,Attack,CombatSystem,Vector3> OnHitboxColliderHit;

        #endregion

        #region Unity methods

        private void Awake() => _collider = GetComponent<Collider>();

        private void OnTriggerEnter(Collider other)
        {
            if(!_hitboxLock.CanDoAction()) return;

            AttackCollider attackerAttackCollider = other.GetComponent<AttackCollider>();
            
            if(attackerAttackCollider) return;

            if(attackerAttackCollider.OwnerCombatSystem.transform.root == transform.root) return;
            
            OnHitboxColliderHit?.Invoke(attackerAttackCollider.CurrentId,attackerAttackCollider.CurrentAttack,attackerAttackCollider.OwnerCombatSystem,_collider.ClosestPoint(other.transform.position));
        }

        #endregion

        #region Interfaces

        public void Block() => _hitboxLock.AddLock();

        public void Unblock() => _hitboxLock.RemoveLock();

        public UnitActionsBlockManager.UnitAction GetAction() => UnitActionsBlockManager.UnitAction.ReceiveDamage;

        #endregion
    }
}