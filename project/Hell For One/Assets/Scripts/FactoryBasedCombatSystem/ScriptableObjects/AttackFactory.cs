using System;
using UnityEngine;

namespace FactoryBasedCombatSystem.ScriptableObjects
{
     #region Factories

     public abstract class AttackFactory : ScriptableObject
     {
         public abstract Attack GetAttack();
     }

     public class AttackFactory<TAttack, TAttackData> : AttackFactory
     where TAttack : Attack, new()
     where TAttackData : AttackData
     {
         [SerializeField]
         private TAttackData data;

         public override Attack GetAttack() => new TAttack { data = this.data};
     }

     #endregion

    #region Attack data

    [Serializable]
    public class AttackData
    {
        
    }
    
    #endregion

    #region Attacks

    public abstract class Attack
    {
        internal AttackData data;

        public AttackData GetData() => data;
    }
    
    #endregion
}