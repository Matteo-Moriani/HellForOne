using FactoryBasedCombatSystem.ScriptableObjects;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class AttackCollider : MonoBehaviour
    {
        #region Fields

        private Attack _currentAttack;
        private Transform _currentOwner;
        private CombatSystem _ownerCombatSystem;
        
        #endregion

        #region Properties

        public Attack CurrentAttack
        {
            get => _currentAttack;
            set => _currentAttack = value;
        }

        public Transform CurrentOwner
        {
            get => _currentOwner;
            set => _currentOwner = value;
        }

        public CombatSystem OwnerCombatSystem
        {
            get => _ownerCombatSystem;
            set => _ownerCombatSystem = value;
        }

        #endregion

        #region Unity Methods

        private void OnDisable()
        {
            transform.localScale = Vector3.zero;
            
            _currentAttack = null;
            _currentOwner = null;
            _ownerCombatSystem = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root == _currentOwner) return;
            
            if (other.gameObject.layer != LayerMask.NameToLayer("CombatSystem")) return;

            HitboxCollider targetHitboxCollider = other.GetComponent<HitboxCollider>();
            
            if(targetHitboxCollider == null) return;

            // TODO :- Do something
        }

        #endregion
        
        public void SetSize(float size) => transform.localScale = Vector3.one * size;
        
        public void Initialize(Attack attack, Transform ownerTransform, CombatSystem ownerCombatSystem)
        {
            _currentAttack = attack;
            _currentOwner = ownerTransform;
            _ownerCombatSystem = ownerCombatSystem;
        }
    }
}