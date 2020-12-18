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
        private GameObject _hammer;
        
        protected override IEnumerator InnerDoGroupAbility(Transform groupTransform)
        {
            ImpGroupAi impGroupAi = groupTransform.GetComponent<ImpGroupAi>();

            CombatSystem hammerCombatSystem = _hammer.GetComponentInChildren<CombatSystem>();

            while (impGroupAi.Target.GetTransformDistanceFromTarget(_hammer.transform) > data.MINDistanceBeforeAttack)
            {
                _hammer.transform.position = groupTransform.position + new Vector3(0f,1f,0f) + (_hammer.transform.position - impGroupAi.Target.Target.position).normalized;
                _hammer.transform.rotation = Quaternion.LookRotation((impGroupAi.Target.Target.position - _hammer.transform.position).normalized,Vector3.up);
                
                yield return null;
            }
            
            hammerCombatSystem.StartAttack(data.AssociatedAttack.GetAttack());

            float timer = 0f;

            while (timer <= data.ActivatedDuration)
            {
                _hammer.transform.position = groupTransform.position + new Vector3(0f,1f,0f) + (_hammer.transform.position - impGroupAi.Target.Target.position).normalized;
                _hammer.transform.rotation = Quaternion.LookRotation((impGroupAi.Target.Target.position - _hammer.transform.position).normalized,Vector3.up);

                yield return null;
                
                timer += Time.deltaTime;
            }

            _hammer.transform.rotation = Quaternion.Euler(0f,0f,0f);
        }

        protected override void InnerSetup()
        {
            _hammer = PoolersManager.Instance.TryGetPooler(data.HammerPrefab).GetPooledObject();
        }
        
        protected override void InnerDispose()
        {
            if(_hammer == null) return;
            
            PoolersManager.Instance.TryGetPooler(data.HammerPrefab).DeactivatePooledObject(_hammer);

            _hammer = null;
        }
    }
}