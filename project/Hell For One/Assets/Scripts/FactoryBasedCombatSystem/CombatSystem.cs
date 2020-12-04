using System;
using System.Collections.Generic;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using FactoryBasedCombatSystem.ScriptableObjects.Units;
using Interfaces;
using OrdersSystem.ScriptableObjects;
using UnityEngine;

using Utils;
using Random = UnityEngine.Random;

namespace FactoryBasedCombatSystem
{
    [RequireComponent(typeof(Block))]
    public class CombatSystem : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Unit unitType;
        [SerializeField] private Transform projectileAnchor;

        private readonly Dictionary<int, Coroutine> _activeAttacks = new Dictionary<int, Coroutine>();

        private HitboxCollider[] _hitboxColliders;
        private Block _block;

        #endregion

        #region Properties

        public Transform ProjectileAnchor
        {
            get => projectileAnchor;
            private set => projectileAnchor = value;
        }

        #endregion

        #region Events

        public event Action OnActivateAttack;
        public event Action OnDeactivateAttack;

        public event Action OnStartAttack;
        public event Action OnStopAttack;

        public event Action OnDamageHitDealt;
        public event Action OnDamageHitReceived;

        public event Action OnBlockedHitDealt;
        public event Action OnBlockedHitReceived;
        
        #endregion

        #region Unity methods

        private void Awake()
        {
            _hitboxColliders = GetComponentsInChildren<HitboxCollider>();
            _block = GetComponent<Block>();
        }

        private void OnEnable()
        {
            foreach (HitboxCollider hitboxCollider in _hitboxColliders)
            {
                hitboxCollider.OnHitboxColliderHit += OnHitboxColliderHit;
            }
        }

        private void OnDisable()
        {
            foreach (HitboxCollider hitboxCollider in _hitboxColliders)
            {
                hitboxCollider.OnHitboxColliderHit -= OnHitboxColliderHit;
            }
        }

        #endregion

        #region Methods

        // TODO :- Pass AttackCollider directly in order to avoid multiple GetComponentInChildren?
        public void StartAttack(Attack attack, Transform target = null)
        {
            if (_activeAttacks.Keys.Count > 0 && !attack.GetData().CanBeMultiple) return;

            int id = IdManager.Instance.GetId();

            _activeAttacks.Add(id, StartCoroutine(attack.DoAttack(id, this, StopAttack, target)));

            OnStartAttack?.Invoke();
        }

        private void StopAttack(int id)
        {
            StopCoroutine(_activeAttacks[id]);

            _activeAttacks.Remove(id);

            IdManager.Instance.FreeId(id);

            OnStopAttack?.Invoke();
        }

        #endregion

        #region Event handlers

        // TODO :- Animation events
        public void OnAttackAnimationActivateAttack() => OnActivateAttack?.Invoke();

        // TODO :- Animation events
        public void OnAttackAnimationDeactivateAttack() => OnDeactivateAttack?.Invoke();

        private void OnHitboxColliderHit(Attack attackerAttack, CombatSystem attackerCombatSystem, Vector3 contactPoint)
        {
            if (!unitType.CanBeHit(attackerCombatSystem.unitType)) return;

            if (attackerAttack.GetData().Blockable && _block.TryBlock())
            {
                OnBlockedHitReceived?.Invoke();
                attackerCombatSystem.OnBlockedHitDealt?.Invoke();
            }
            else
            {
                OnDamageHitReceived?.Invoke();
                attackerCombatSystem.OnDamageHitDealt?.Invoke();   
            }
        }

        #endregion
    }
}