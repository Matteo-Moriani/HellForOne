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
    }

    public class HammerAbility : GroupAbility<HammerAbilityData>
    {
        private bool _deactivate = false;
        
        protected override IEnumerator InnerDoGroupAbility(Transform groupTransform)
        {
            GameObject hammer = PoolersManager.Instance.TryGetPooler(data.HammerPrefab).GetPooledObject();
            
            hammer.transform.position = groupTransform.position + new Vector3(0f,1f,0f);
            hammer.transform.rotation = groupTransform.rotation;

            hammer.transform.SetParent(groupTransform);

            CombatSystem hammerCombatSystem = hammer.GetComponentInChildren<CombatSystem>();

            hammerCombatSystem.OnStopAttack += OnStopAttack;
            
            hammer.GetComponentInChildren<CombatSystem>().StartAttack(data.AssociatedAttack.GetAttack());

            yield return new WaitForSeconds(data.ActivatedDuration);

            PoolersManager.Instance.TryGetPooler(data.HammerPrefab).DeactivatePooledObject(hammer);
        }

        private void OnStopAttack()
        {
            _deactivate = true;
        }

        protected override void InnerSetup()
        {
        }
        
        protected override void InnerDispose()
        {
        }
    }
}