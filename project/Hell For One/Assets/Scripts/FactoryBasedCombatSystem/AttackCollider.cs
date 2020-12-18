using System;
using FactoryBasedCombatSystem.ScriptableObjects;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    [RequireComponent(typeof(SphereCollider))]
    public class AttackCollider : MonoBehaviour
    {
        #region Fields

        private Attack _currentAttack;
        private CombatSystem _ownerCombatSystem;
        private SphereCollider _sphereCollider;
        
        private int _currentId;
        
        #endregion

        #region Properties

        public Attack CurrentAttack
        {
            get => _currentAttack;
            private set => _currentAttack = value;
        }

        public CombatSystem OwnerCombatSystem
        {
            get => _ownerCombatSystem;
            private set => _ownerCombatSystem = value;
        }

        public int CurrentId
        {
            get => _currentId;
            private set => _currentId = value;
        }

        #endregion

        #region Unity Methods

        private void Awake() => _sphereCollider = GetComponent<SphereCollider>();

        private void OnEnable()
        {
            _currentAttack = null;
            _ownerCombatSystem = null;
            _currentId = -int.MaxValue;
            _sphereCollider.radius = 0f;
            _sphereCollider.enabled = false;
        }

        private void OnDisable()
        {
            _currentAttack = null;
            _ownerCombatSystem = null;
            _currentId = -int.MaxValue;
            _sphereCollider.radius = 0f;
            _sphereCollider.enabled = false;
        }

        #endregion

        #region Methods

        public void SetRadius(float newRadius) => _sphereCollider.radius = newRadius;
        
        public void Initialize(int id, float radius, Attack attack, Transform ownerTransform, CombatSystem ownerCombatSystem)
        {
            _currentAttack = attack;
            _ownerCombatSystem = ownerCombatSystem;
            _currentId = id;
            _sphereCollider.radius = radius;
            _sphereCollider.enabled = true;
        }

        #endregion
    }
}