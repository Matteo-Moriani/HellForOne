using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryBasedCombatSystem.ScriptableObjects.Attacks
{
    #region Factories

     public abstract class AttackFactory : ScriptableObject
     {
         public abstract Attack GetAttack();
     }

     public abstract class AttackFactory<TAttack, TAttackData> : AttackFactory
     where TAttack : Attack<TAttackData>, new()
     where TAttackData : AttackData
     {
         [SerializeField]
         private TAttackData data;

         public override Attack GetAttack() => new TAttack
         {
             data = this.data, 
             name = this.name
         };
     }

     #endregion

     #region Attack data

     [Serializable]
    public abstract class AttackData
    {
        #region Fields
        
        [Header("Common")]
        [SerializeField, Min(0f)] private float damage;
        [SerializeField, Min(0f)] private float colliderRadius;
        [SerializeField] private bool blockable;
        [SerializeField] private bool canBeMultiple;
        [SerializeField] private bool canDamageMultipleUnits;

        [Header("Knockback")]
        [SerializeField] private bool dealsKnockback;
        [SerializeField] private bool dealKnockbackWhenBlocked;
        [SerializeField, Min(0f)] private float knockbackDuration;
        [SerializeField, Min(0f)] private float knockbackSize;
        
        [Header("Stun")]
        [SerializeField] private bool dealsStun;
        [SerializeField] private bool dealsStunWhenBlocked;
        [SerializeField, Min(0f)] private float stunTime;
        
        [Header("Splash Damage")]
        [SerializeField] private bool splashDamage;
        [SerializeField, Min(0f)] private float splashDamageRadius;
        [SerializeField, Min(0f)] private float splashDamageTime;

        #endregion

        #region Properties

        public bool CanBeMultiple
        {
            get => canBeMultiple;
            private set => canBeMultiple = value;
        }

        public float ColliderRadius
        {
            get => colliderRadius;
            private set => colliderRadius = value;
        }

        public bool SplashDamage
        {
            get => splashDamage;
            private set => splashDamage = value;
        }

        public float SplashDamageRadius
        {
            get => splashDamageRadius;
            private set => splashDamageRadius = value;
        }

        public float Damage
        {
            get => damage;
            private set => damage = value;
        }

        public bool Blockable
        {
            get => blockable;
            private set => blockable = value;
        }

        public bool DealsStun
        {
            get => dealsStun;
            private set => dealsStun = value;
        }

        public bool DealsStunWhenBlocked
        {
            get => dealsStunWhenBlocked;
            private set => dealsStunWhenBlocked = value;
        }

        public float StunTime
        {
            get => stunTime;
            private set => stunTime = value;
        }

        public bool DealsKnockback
        {
            get => dealsKnockback;
            private set => dealsKnockback = value;
        }

        public bool DealKnockbackWhenBlocked
        {
            get => dealKnockbackWhenBlocked;
            private set => dealKnockbackWhenBlocked = value;
        }

        public float KnockbackDuration
        {
            get => knockbackDuration;
            private set => knockbackDuration = value;
        }

        public float KnockbackSize
        {
            get => knockbackSize;
            private set => knockbackSize = value;
        }

        public float SplashDamageTime
        {
            get => splashDamageTime;
            private set => splashDamageTime = value;
        }

        public bool CanDamageMultipleUnits
        {
            get => canDamageMultipleUnits;
            private set => canDamageMultipleUnits = value;
        }

        #endregion
    }
    
    #endregion

    #region Attacks

    public abstract class Attack
    {
        public string name;
        
        protected Dictionary<int, bool> HasHit { get; private set; } = new Dictionary<int, bool>();
        protected Dictionary<int, bool> AnimationStates { get; private set; } = new Dictionary<int, bool>();

        public IEnumerator DoAttack(int id, CombatSystem ownerCombatSystem, Action<Attack,int> stopAction, Transform target = null)
        {
            Setup(id,ownerCombatSystem, target);
            
            yield return InnerDoAttack(id,ownerCombatSystem, target);
            
            Dispose(id,ownerCombatSystem,stopAction);
        }

        public void SafeStop(int id, CombatSystem ownerCombatSystem, Action<Attack,int> stopAction) =>
            Dispose(id, ownerCombatSystem, stopAction);

        private void Setup(int id, CombatSystem ownerCombatSystem, Transform target)
        {
            InnerSetup(id,ownerCombatSystem,target);
            
            AnimationStates.Add(id,false); 

            HasHit.Add(id,false);
        }

        private void Dispose(int id, CombatSystem ownerCombatSystem, Action<Attack,int> stopAction)
        {
            InnerDispose(id,ownerCombatSystem);

            AnimationStates.Remove(id);

            HasHit.Remove(id);

            stopAction(this,id);
        }

        public void ActivateAttack(int id) => AnimationStates[id] = true;

        public void DeactivateAttack(int id) => AnimationStates[id] = false;

        public void NotifyHit(int id) => HasHit[id] = true;

        #region Abstract members

        protected abstract IEnumerator InnerDoAttack(int id, CombatSystem ownerCombatSystem, Transform target);
        public abstract AttackData GetData();
        protected abstract void InnerSetup(int id, CombatSystem ownerCombatSystem, Transform target);
        protected abstract void InnerDispose(int id, CombatSystem ownerCombatSystem);

        #endregion
    }

    public abstract class Attack<TData> : Attack 
        where TData : AttackData
    {
        public TData data;

        public override AttackData GetData() => data;
    }

    #endregion
}