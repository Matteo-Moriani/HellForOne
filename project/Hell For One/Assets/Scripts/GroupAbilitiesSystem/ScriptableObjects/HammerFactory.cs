using AI.Imp;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using System;
using System.Collections;
using System.Collections.Generic;
using AI.Movement;
using FactoryBasedCombatSystem;
using UnityEngine;
using Utils.ObjectPooling;

namespace GroupAbilitiesSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "GroupAbilities/HammerAbility", fileName = "HammerAbility", order = 1)]
    public class HammerFactory : GroupAbilityFactory<HammerAbility,HammerAbilityData>
    {
        
    }

    [Serializable]
    public class HammerAbilityData : GroupAbilityData
    {
        [SerializeField] private GameObject hammerPrefab;
        [SerializeField] private AttackFactory associatedAttack;
        [SerializeField] private float minDistanceBeforeAttack;

        public GameObject HammerPrefab
        {
            get => hammerPrefab; 
            private set => hammerPrefab = value;
        }

        public AttackFactory AssociatedAttack
        {
            get => associatedAttack;
            private set => associatedAttack = value;
        }

        public float MINDistanceBeforeAttack
        {
            get => minDistanceBeforeAttack;
            private set => minDistanceBeforeAttack = value;
        }
    }

    public class HammerAbility : GroupAbility<HammerAbilityData>
    { 
        protected override IEnumerator InnerDoGroupAbility(Transform groupTransform)
        {
            ImpGroupAi impGroupAi = groupTransform.GetComponent<ImpGroupAi>();
            
            GameObject hammer = PoolersManager.Instance.TryGetPooler(data.HammerPrefab).GetPooledObject();

            CombatSystem hammerCombatSystem = hammer.GetComponentInChildren<CombatSystem>();

            while (impGroupAi.Target.GetTransformDistanceFromTarget(hammer.transform) > data.MINDistanceBeforeAttack)
            {
                hammer.transform.position = groupTransform.position + new Vector3(0f,1f,0f) + (hammer.transform.position - impGroupAi.Target.Target.position).normalized;
                hammer.transform.rotation = Quaternion.LookRotation((impGroupAi.Target.Target.position - hammer.transform.position).normalized,Vector3.up);
                
                yield return null;
            }
            
            hammerCombatSystem.StartAttack(data.AssociatedAttack.GetAttack());

            float timer = 0f;

            while (timer <= data.ActivatedDuration)
            {
                hammer.transform.position = groupTransform.position + new Vector3(0f,1f,0f) + (hammer.transform.position - impGroupAi.Target.Target.position).normalized;
                hammer.transform.rotation = Quaternion.LookRotation((impGroupAi.Target.Target.position - hammer.transform.position).normalized,Vector3.up);

                yield return null;
                
                timer += Time.deltaTime;
            }

            hammer.transform.rotation = Quaternion.Euler(0f,0f,0f);
            
            PoolersManager.Instance.TryGetPooler(data.HammerPrefab).DeactivatePooledObject(hammer);
        }

        protected override void InnerSetup()
        {
        }
        
        protected override void InnerDispose()
        {
        }
    }
}