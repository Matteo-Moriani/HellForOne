using System;
using System.Collections;
using UnityEngine;

namespace FactoryBasedCombatSystem.ScriptableObjects.Attacks
{
    [CreateAssetMenu(menuName = ("CombatSystem/Attacks/MeleeAttack"),fileName = "MeleeAttack", order = 1)]
    public class MeleeAttackFactory : AttackFactory<MeleeAttack,MeleeAttackData> { }

    [Serializable]
    public class MeleeAttackData : AttackData
    {
        [SerializeField] private float range;

        public float Range
        {
            get => range;
            private set => range = value;
        }
    }

    public class MeleeAttack : Attack<MeleeAttackData>
    { 
        protected override IEnumerator InnerDoAttack(CombatSystem ownerCombatSystem, Transform target)
        {
            AttackCollider attackCollider = ownerCombatSystem.GetComponentInChildren<AttackCollider>();
            Transform attackColliderTransform = attackCollider.transform;
            
            while (!StartAttack) yield return null;

            attackColliderTransform.position += Vector3.forward * data.Range;
            attackColliderTransform.localScale = Vector3.one * data.ColliderRadius;

            while (StartAttack && !attackCollider.HasHit) yield return null;

            attackColliderTransform.position -= Vector3.forward * data.Range;
            attackColliderTransform.localScale = Vector3.zero;
        }
    }
}