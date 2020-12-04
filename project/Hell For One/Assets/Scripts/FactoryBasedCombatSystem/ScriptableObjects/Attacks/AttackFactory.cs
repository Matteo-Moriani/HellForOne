using System;
using System.Collections;
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

         public override Attack GetAttack() => new TAttack { data = this.data};
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
        
        [Header("Splash Damage")]
        [SerializeField] private bool splashDamage;
        [SerializeField, Min(0f)] private float splashDamageRadius;

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

        #endregion
    }
    
    #endregion

    #region Attacks

    public abstract class Attack
    {
        protected bool StartAttack { get; private set;}

        public IEnumerator DoAttack(int id, CombatSystem ownerCombatSystem, Action<int> stopAction, Transform target = null)
        {
            Setup(ownerCombatSystem);
            
            yield return InnerDoAttack(ownerCombatSystem, target);
            
            Dispose(id,ownerCombatSystem,stopAction);
        }

        protected virtual void Setup(CombatSystem ownerCombatSystem)
        {
            ownerCombatSystem.OnActivateAttack += OnActivateAttack;
            ownerCombatSystem.OnDeactivateAttack += OnDeactivateAttack;
        }

        protected virtual void Dispose(int id, CombatSystem ownerCombatSystem, Action<int> stopAction)
        {
            ownerCombatSystem.OnActivateAttack -= OnActivateAttack;
            ownerCombatSystem.OnDeactivateAttack -= OnDeactivateAttack;

            stopAction(id);
        }

        // TODO :- Stop attack using notify hit (a bool maybe)
        public abstract void NotifyHit();

        #region Abstract members

        protected abstract IEnumerator InnerDoAttack(CombatSystem ownerCombatSystem, Transform target);
        public abstract AttackData GetData();

        #endregion

        #region Event handlers

        private void OnDeactivateAttack() => StartAttack = false;

        private void OnActivateAttack() => StartAttack = true;

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