using FactoryBasedCombatSystem.ScriptableObjects;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class AttackCollider : MonoBehaviour
    {
        private Attack _currentAttack;
        private Transform _currentOwner;
        private CombatSystem _ownerCombatSystem;
        
        private bool _hasHit = false;

        //public event Action<Attack,AttackDamage,HitboxCollider> OnHit;
        
        public bool HasHit
        {
            get => _hasHit;
            private set => _hasHit = value;
        }

        private void OnDisable()
        {
            transform.localScale = Vector3.zero;
            
            _currentAttack = null;
            _currentOwner = null;
            _ownerCombatSystem = null;
            
            _hasHit = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.transform.root == _currentOwner)
                return;
            
            if(other.gameObject.layer == LayerMask.NameToLayer("CombatSystem"))
            {
                _ownerCombatSystem.OnHit(_currentAttack);
            
                _hasHit = true;
            }
            // else if(other.gameObject.layer == LayerMask.NameToLayer("Environment") || other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            // {
            //     _currentAttack.NotifyEnvironmentHit(transform, _currentOwner, _identity);
            //     
            //     _hasHit = true;
            // }
        }

        public void SetSize(float size) => transform.localScale = Vector3.one * size;

        /// <summary>
        /// Initialize the projectile
        /// This is called before this behaviour's Start method
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="ownerTransform"></param>
        /// <param name="ownerCombatSystem"></param>
        public void Initialize(Attack attack, Transform ownerTransform, CombatSystem ownerCombatSystem)
        {
            _currentAttack = attack;

            _currentOwner = ownerTransform;

            _ownerCombatSystem = ownerCombatSystem;
            
            _hasHit = false;
        }
    }
}