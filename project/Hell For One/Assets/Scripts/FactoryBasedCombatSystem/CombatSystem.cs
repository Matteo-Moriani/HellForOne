using System;
using System.Collections.Generic;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using FactoryBasedCombatSystem.ScriptableObjects.Units;
using UnityEngine;

using Utils;

namespace FactoryBasedCombatSystem
{
    public class CombatSystem : MonoBehaviour
    {
        #region Fields
    
        [SerializeField] private Unit unitType;
        [SerializeField] private Transform projectileAnchor;
        
        private readonly Dictionary<int,Coroutine> _activeAttacks = new Dictionary<int, Coroutine>(); 
        
        private HitboxCollider[] _hitboxColliders;
        
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
        
        #endregion

        #region Unity methods

        private void Awake()
        {
            _hitboxColliders = GetComponentsInChildren<HitboxCollider>();
        }

        private void OnEnable()
        {
            foreach (HitboxCollider hitboxCollider in _hitboxColliders)
            {
                hitboxCollider.OnBeingHit += OnBeingHit;   
            }
        }

        private void OnDisable()
        {
            foreach (HitboxCollider hitboxCollider in _hitboxColliders)
            {
                hitboxCollider.OnBeingHit -= OnBeingHit;   
            }
        }

        #endregion
    
        #region Methods

        // TODO :- Pass AttackCollider directly in order to avoid multiple GetComponentInChildren?
        public void StartAttack(Attack attack, Transform target = null)
        {
            if (_activeAttacks.Keys.Count > 0 && !attack.GetData().CanBeMultiple) return;

            int id = IdManager.Instance.GetId();
            
            _activeAttacks.Add(id,StartCoroutine(attack.DoAttack(id,this, StopAttack, target)));
            
            OnStartAttack?.Invoke();
        }

        private void StopAttack(int id)
        {
            StopCoroutine(_activeAttacks[id]);
            
            _activeAttacks.Remove(id);
            
            IdManager.Instance.FreeId(id);
            
            OnStopAttack?.Invoke();
        }

        // private ProcessedAttackDamage CalculateAttackDamage(Attack attack, AttackDamage attackDamage)
        // {
        //     ProcessedAttackDamage processedAttackDamage = new ProcessedAttackDamage(
        //         attackDamage.GetStatWithModifiersApplied(),
        //         attack.GetData().MinDamage,
        //         attack.GetData().MaxDamage
        //     );
        //
        //     float randomMultiplier = attack.GetData().UseRandomMultiplier
        //         ? Random.Range(attack.GetData().RandomRangeLeft, attack.GetData().RandomRangeRight)
        //         : 1.0f;
        //
        //     float unitAttackMultiplier = _unitAttackDamageMultiplier.GetStatWithModifiersApplied();
        //
        //     processedAttackDamage.ApplyMultiplierModifier(randomMultiplier);
        //     processedAttackDamage.ApplyMultiplierModifier(unitAttackMultiplier);
        //
        //     return processedAttackDamage;
        // }
        //
        // private ProcessedAttackHealing CalculateAttackHealing() => 
        //     new ProcessedAttackHealing(
        //         _unitAttackAlliesHealing.GetStatWithModifiersApplied(),
        //         unitMinAttackAlliesHealing,
        //         unitMaxAttackAlliesHealing
        //         );
        
        #endregion

        #region Event handlers

        // TODO :- Animation events
        public void OnAttackAnimationActivateAttack() => OnActivateAttack?.Invoke();

        // TODO :- Animation events
        public void OnAttackAnimationDeactivateAttack() => OnDeactivateAttack?.Invoke();

        private void OnBeingHit(Attack attack, CombatSystem attackerCombatSystem)
        {
            throw new NotImplementedException();
        }
        
        public void OnHit(Attack attack)
        {
            throw new NotImplementedException();
            
            // CombatSystem hitCombatSystem = colliderHit.transform.root.GetComponentInChildren<CombatSystem>();
            //
            // if (!hitCombatSystem.unitType.CanReceiveDamage(this.unitType)) return;
            //
            // AttackUnitHitData unitHitData = new AttackUnitHitData(
            //     transform.root,
            //     attack,
            //     colliderHit,
            //     CalculateAttackDamage(attack,attackDamage),
            //     CalculateAttackHealing(),
            //     _unitWeakSpotBonusDamageMultiplier.GetStatWithModifiersApplied()
            //     );            
            //
            // attack.NotifyUnitHit(colliderHit, transform, transform.root, colliderHit.transform.root, _networkIdentity);
            // colliderHit.NotifyHit(unitHitData);
        }

        // private void OnBeingHit(AttackUnitHitData hitData)
        // {
        //     hitData.ProcessedDamage.ApplyMultiplierModifier(hitData.ColliderHit.damageMultiplier > 1.0f ? hitData.WeakSpotDamageMultiplier : 1);
        //     hitData.ProcessedDamage.ApplyMultiplierModifier(1 - _unitDefence.GetStatWithModifiersApplied());
        //     
        //     CanvasScaler.Unit attacker = hitData.AttackInstance.GetData().AttackerType;
        //     
        //     if(unitType.CanReceiveDamage(attacker))
        //         OnDamageHitReceived?.Invoke(hitData);
        //     if (unitType.CanReceiveHealing(attacker))
        //         OnHealingHitReceived?.Invoke(hitData);    
        // }

        #endregion
    }
}