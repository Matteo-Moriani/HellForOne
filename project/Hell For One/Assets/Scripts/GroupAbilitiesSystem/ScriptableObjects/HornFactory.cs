using System;
using System.Collections;
using AI.Imp;
using ArenaSystem;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;
using Utils.ObjectPooling;

namespace GroupAbilitiesSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "GroupAbilities/HornAbility", fileName = "HornAbility", order = 1)]
    public class HornFactory : GroupAbilityFactory<HornAbility,HornAbilityData>
    {
        
    }

    [Serializable]
    public class HornAbilityData : GroupAbilityData
    {
        [SerializeField] private int bursts;
        [SerializeField] private float timeBetweenBursts;
        [SerializeField] private AttackFactory associatedAttack;
        
        public AttackFactory AssociatedAttack
        {
            get => associatedAttack;
            private set => associatedAttack = value;
        }

        public int Bursts
        {
            get => bursts;
            private set => bursts = value;
        }

        public float TimeBetweenBursts
        {
            get => timeBetweenBursts;
            private set => timeBetweenBursts = value;
        }
    }

    public class HornAbility : GroupAbility<HornAbilityData>
    {
        private GameObject _horn;
        
        protected override IEnumerator InnerDoGroupAbility(Transform groupTransform)
        {
            _horn.transform.position = groupTransform.position + new Vector3(0f, 1f, 0f);
            
            _horn.transform.SetParent(groupTransform);
            
            ArenaManager arena = groupTransform.GetComponent<ImpGroupAi>().Target.Target.GetComponent<ArenaBoss>()
                .Arena;

            CombatSystem[] spectatorsSystems = arena.GetComponentsInChildren<CombatSystem>();

            float timer = 0f;
            int i = 0;
            
            while (timer <= data.ActivatedDuration)
            {
                if(spectatorsSystems[i].enabled)
                    spectatorsSystems[i].StartAttack(data.AssociatedAttack.GetAttack(),arena.Boss.transform);

                yield return null;
                
                timer += Time.deltaTime;
                i = i < spectatorsSystems.Length - 1 ? i + 1 : 0;
            }
        }

        protected override void InnerSetup()
        {
            _horn = PoolersManager.Instance.TryGetPooler(data.AbilityPrefab).GetPooledObject();
        }

        protected override void InnerDispose()
        {
            if (_horn == null) return;
            
            PoolersManager.Instance.TryGetPooler(data.AbilityPrefab).DeactivatePooledObject(_horn);

            _horn = null;
        }
    }
}