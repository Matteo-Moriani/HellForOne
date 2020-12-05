using System;
using System.Collections.Generic;
using ActionsBlockSystem;
using Animations;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using FactoryBasedCombatSystem.ScriptableObjects.Units;
using UnityEngine;
using Utils;

namespace FactoryBasedCombatSystem
{
    [RequireComponent(typeof(Block))]
    public class CombatSystem : MonoBehaviour, IActionsBlockObserver
    {
        #region Fields

        [SerializeField] private Unit unitType;
        [SerializeField] private Transform projectileAnchor;

        private readonly Dictionary<int, Tuple<Attack,Coroutine>> _activeAttacks = new Dictionary<int, Tuple<Attack,Coroutine>>();
        private readonly Queue<int> _toActivate = new Queue<int>();
        private readonly Queue<int> _toDeactivate = new Queue<int>();
        
        private HitboxCollider[] _hitboxColliders;
        private Block _block;
        private AnimationEventsHooks _animationEventsHooks;
        
        private readonly ActionLock _combatSystemLock = new ActionLock();

        #endregion

        #region Properties

        public Transform ProjectileAnchor
        {
            get => projectileAnchor;
            private set => projectileAnchor = value;
        }

        #endregion

        #region Events

        public event Action OnStartAttack;
        public event Action OnStopAttack;
        
        public event Action<Attack,CombatSystem,Vector3> OnDamageHitDealt;
        public event Action<Attack,CombatSystem,Vector3> OnDamageHitReceived;

        public event Action<Attack,CombatSystem,Vector3> OnBlockedHitDealt;
        public event Action<Attack,CombatSystem,Vector3> OnBlockedHitReceived;
        
        #endregion

        #region Unity methods

        private void Awake()
        {
            _hitboxColliders = GetComponentsInChildren<HitboxCollider>();
            _block = GetComponent<Block>();
            _animationEventsHooks = transform.root.GetComponent<AnimationEventsHooks>();
        }

        private void OnEnable()
        {
            foreach (HitboxCollider hitboxCollider in _hitboxColliders)
            {
                hitboxCollider.OnHitboxColliderHit += OnHitboxColliderHit;
            }

            _animationEventsHooks.OnAttackAnimationActivateAttack += OnAttackAnimationActivateAttack;
            _animationEventsHooks.OnAttackAnimationDeactivateAttack += OnAttackAnimationDeactivateAttack;
        }

        private void OnDisable()
        {
            foreach (HitboxCollider hitboxCollider in _hitboxColliders)
            {
                hitboxCollider.OnHitboxColliderHit -= OnHitboxColliderHit;
            }
            
            _animationEventsHooks.OnAttackAnimationActivateAttack -= OnAttackAnimationActivateAttack;
            _animationEventsHooks.OnAttackAnimationDeactivateAttack -= OnAttackAnimationDeactivateAttack;
        }

        #endregion

        #region Methods
        
        public void StartAttack(Attack attack, Transform target = null)
        {
            if (_activeAttacks.Keys.Count > 0 && !attack.GetData().CanBeMultiple) return;

            int id = IdManager.Instance.GetId();

            _activeAttacks.Add(id, new Tuple<Attack, Coroutine>(attack,StartCoroutine(attack.DoAttack(id, this, StopAttack, target))));
            _toActivate.Enqueue(id);
            _toDeactivate.Enqueue(id);
            
            OnStartAttack?.Invoke();
        }

        private void StopAttack(int id)
        {
            StopCoroutine(_activeAttacks[id].Item2);

            _activeAttacks.Remove(id);

            IdManager.Instance.FreeId(id);

            OnStopAttack?.Invoke();
        }

        #endregion

        #region Event handlers

        private void OnAttackAnimationActivateAttack()
        {
            int id = _toActivate.Dequeue();
            _activeAttacks[id].Item1.ActivateAttack(id);
        }

        private void OnAttackAnimationDeactivateAttack()
        {
            int id = _toDeactivate.Dequeue();
            _activeAttacks[id].Item1.DeactivateAttack(id);
        }
        
        private void OnHitboxColliderHit(int id, Attack attackerAttack, CombatSystem attackerCombatSystem, Vector3 contactPoint)
        {
            if (!unitType.CanBeHit(attackerCombatSystem.unitType)) return;

            if (attackerAttack.GetData().Blockable && _block.TryBlock())
            {
                OnBlockedHitReceived?.Invoke(attackerAttack,attackerCombatSystem,contactPoint);
                
                attackerCombatSystem.OnBlockedHitDealt?.Invoke(attackerAttack,this,contactPoint);
            }
            else
            {
                OnDamageHitReceived?.Invoke(attackerAttack,attackerCombatSystem,contactPoint);
                
                attackerCombatSystem.OnDamageHitDealt?.Invoke(attackerAttack,this,contactPoint);
            }
            
            attackerCombatSystem._activeAttacks[id].Item1.NotifyHit(id);
        }

        #endregion

        #region Interfaces

        public void Block()
        {
            foreach (KeyValuePair<int,Tuple<Attack,Coroutine>> item in _activeAttacks)
            {
                item.Value.Item1.SafeStop(item.Key,this,StopAttack);    
            }
            
            _toActivate.Clear();
            _toDeactivate.Clear();
            _activeAttacks.Clear();
            
            _combatSystemLock.AddLock();
        }

        public void Unblock() => _combatSystemLock.RemoveLock();

        public UnitActionsBlockManager.UnitAction GetAction() => UnitActionsBlockManager.UnitAction.Attack;

        #endregion
    }
}