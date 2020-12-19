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

     public abstract class BossAttackFactory : ScriptableObject
     {
         public abstract BossAttack GetAttack();
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

     public abstract class BossAttackFactory<TAttack, TAttackData> : BossAttackFactory
         where TAttack : BossAttack<TAttackData>, new()
         where TAttackData : BossAttackData
     { 
         [SerializeField]
         private TAttackData data;

         public override BossAttack GetAttack() => new TAttack
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
        [SerializeField, Min(0f)] private float colliderActivationDelay;
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

        [Header("AI")] 
        [SerializeField, Min(0f)] private float minDistance;
        [SerializeField, Min(0f)] private float maxDistance;

        [Header("Camera shake")]
        [SerializeField] private bool doCameraShakeOnDamageHit;
        [SerializeField] private float onDamageHitShakeDuration;
        [SerializeField] private float onDamageHitShakeIntensity;

        [Header("Sound")]
        [SerializeField] private AudioClip attackCry;
        [SerializeField] private float volume = 0.5f;
        [SerializeField] private float pitch = 1f;

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

        public float ColliderActivationDelay
        {
            get => colliderActivationDelay;
            private set => colliderActivationDelay = value;
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

        public float MinDistance
        {
            get => minDistance;
            private set => minDistance = value;
        }

        public float MaxDistance
        {
            get => maxDistance;
            private set => maxDistance = value;
        }

        public bool DoCameraShakeOnDamageHit
        {
            get => doCameraShakeOnDamageHit;
            private set => doCameraShakeOnDamageHit = value;
        }

        public float ONDamageHitShakeDuration
        {
            get => onDamageHitShakeDuration;
            private set => onDamageHitShakeDuration = value;
        }

        public float ONDamageHitShakeIntensity
        {
            get => onDamageHitShakeIntensity;
            private set => onDamageHitShakeIntensity = value;
        }

        public AudioClip AttackCry {
            get => attackCry;
            private set => attackCry = value;
        }

        public float Volume {
            get => volume;
            private set => volume = value;
        }

        public float Pitch {
            get => pitch;
            private set => pitch = value;
        }

        #endregion
    }

     [Serializable]
     public abstract class BossAttackData : AttackData
     {
         [Header("AttackValues")] 
         [SerializeField] [Range(0f,100f)] private float attackProbability;

         public float AttackProbability
         {
             get => attackProbability;
             private set => attackProbability = value;
         }
     }

    #endregion

    #region Attacks

    public abstract class Attack
    {
        public string name;
        
        protected Dictionary<int, bool> HasHit { get; private set; } = new Dictionary<int, bool>();
        protected Dictionary<int, bool> AnimationStates { get; private set; } = new Dictionary<int, bool>();

        public abstract event Action<Attack> OnAttackActivated;

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

            if(!AnimationStates.ContainsKey(id))
                AnimationStates.Add(id,false);
            if(!HasHit.ContainsKey(id))
                HasHit.Add(id,false);
        }

        private void Dispose(int id, CombatSystem ownerCombatSystem, Action<Attack,int> stopAction)
        {
            InnerDispose(id,ownerCombatSystem);

            AnimationStates.Remove(id);
            HasHit.Remove(id);

            stopAction(this,id);
        }

        public void ActivateAttack(int id)
        {
            if(!AnimationStates.ContainsKey(id)) return;
            
            AnimationStates[id] = true;
        }


        public void DeactivateAttack(int id)
        {
            // If the attack hits before animation deactivation,
            // We already have destroyed this key
            if (!AnimationStates.ContainsKey(id)) return;
            
            AnimationStates[id] = false;
        }

        public void NotifyHit(int id) => HasHit[id] = true;

        #region Abstract members

        protected abstract IEnumerator InnerDoAttack(int id, CombatSystem ownerCombatSystem, Transform target);
        public abstract AttackData GetData();
        protected abstract void InnerSetup(int id, CombatSystem ownerCombatSystem, Transform target);
        protected abstract void InnerDispose(int id, CombatSystem ownerCombatSystem);

        #endregion
    }

    public abstract class BossAttack : Attack
    {
        public abstract BossAttackData GetBossAttackData();
    }

    public abstract class Attack<TData> : Attack 
        where TData : AttackData
    {
        public TData data;

        public override AttackData GetData() => data;
    }

    public abstract class BossAttack<TData> : BossAttack
        where TData : BossAttackData
    {
        public TData data;

        public override AttackData GetData() => data;

        public override BossAttackData GetBossAttackData() => data;
    }

    #endregion
}