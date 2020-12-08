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

        private readonly Dictionary<Attack, Dictionary<int,Coroutine>> _activeAttacks = new Dictionary<Attack, Dictionary<int,Coroutine>>();
        private Tuple<Attack, int> _toActivate;
        
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

        public event Action<Attack> OnStartAttack;
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
            if(!_combatSystemLock.CanDoAction()) return;
            
            if(_toActivate != null) return;
            
            if (_activeAttacks.ContainsKey(attack) && !attack.GetData().CanBeMultiple) return;

            int id = IdManager.Instance.GetId();
            
            if(!_activeAttacks.ContainsKey(attack))
                _activeAttacks.Add(attack,new Dictionary<int, Coroutine>());
            
            _activeAttacks[attack].Add(id, StartCoroutine(attack.DoAttack(id, this, StopAttack, target)));
            _toActivate = new Tuple<Attack, int>(attack,id);

            OnStartAttack?.Invoke(attack);
        }

        private void StopAttack(Attack attack,int id)
        {
            StopCoroutine(_activeAttacks[attack][id]);

            _activeAttacks[attack].Remove(id);

            if (_activeAttacks[attack].Keys.Count == 0)
                _activeAttacks.Remove(attack);

            IdManager.Instance.FreeId(id);

            OnStopAttack?.Invoke();
        }

        #endregion

        #region Event handlers

        private void OnAttackAnimationActivateAttack() => _toActivate.Item1.ActivateAttack(_toActivate.Item2);

        private void OnAttackAnimationDeactivateAttack()
        {
            _toActivate.Item1.DeactivateAttack(_toActivate.Item2);
            
            _toActivate = null;
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
            
            attackerAttack.NotifyHit(id);
        }

        #endregion

        #region Interfaces

        public void Block()
        {
            foreach (Attack item in _activeAttacks.Keys)
            {
                foreach (int id in _activeAttacks[item].Keys)
                {
                    item.SafeStop(id,this,StopAttack);   
                }
            }
            _activeAttacks.Clear();
            
            _combatSystemLock.AddLock();
        }

        public void Unblock() => _combatSystemLock.RemoveLock();

        public UnitActionsBlockManager.UnitAction GetAction() => UnitActionsBlockManager.UnitAction.Attack;

        #endregion
    }
}