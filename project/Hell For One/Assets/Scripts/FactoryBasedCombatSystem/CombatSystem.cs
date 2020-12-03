using System;
using System.Collections.Generic;
using FactoryBasedCombatSystem.ScriptableObjects;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

namespace FactoryBasedCombatSystem
{
    public class CombatSystem : MonoBehaviour
    {
        #region Fields
    
        //[SerializeField] private Unit unitType;
        
        [SerializeField] private Transform projectileAnchor;

        [Header("Stats")]
        //[SerializeField] private BuffableStats unitBuffableStats;

        // [SerializeField] private float unitBaseAttackMultiplier = 1f;
        // [SerializeField] private float unitMinAttackMultiplier = .1f;
        // [SerializeField] private float unitMaxAttackMultiplier = 10f;
        //
        // [SerializeField] private float unitBaseAttackAlliesHealing = 0.0f;
        // [SerializeField] private float unitMinAttackAlliesHealing = 0.0f;
        // [SerializeField] private float unitMaxAttackAlliesHealing = 0.0f;
        
        // private AttackDamage _unitAttackDamageMultiplier;
        // private WeakSpotBonusDamageMultiplier _unitWeakSpotBonusDamageMultiplier;
        // private AttackAlliesHealing _unitAttackAlliesHealing;
        // private readonly Defence _unitDefence = new Defence(0f,-10f,10f);

        // TODO - Use ID system
        // private byte _currentMaxId = 0;
        //
        private readonly Dictionary<int,Coroutine> _activeAttacks = new Dictionary<int, Coroutine>(); 
        private HitboxCollider _hitboxCollider;
        
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
            _hitboxCollider = GetComponentInChildren<HitboxCollider>();
        }

        private void OnEnable()
        {
            _hitboxCollider.OnBeingHit += OnBeingHit;
        }

        private void OnDisable()
        {
            _hitboxCollider.OnBeingHit -= OnBeingHit;
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
        
        private void OnBeingHit()
        {
            throw new NotImplementedException();
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