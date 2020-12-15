using System;
using System.Collections;
using System.Collections.Generic;
using AI.Imp;
using ArenaSystem;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;
using Utils.ObjectPooling;
using Random = UnityEngine.Random;

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
        protected override IEnumerator InnerDoGroupAbility(Transform groupTransform)
        {
            //List<GameObject> rangedImps = new List<GameObject>();

            ArenaManager arena = groupTransform.GetComponent<ImpGroupAi>().Target.Target.GetComponent<ArenaBoss>()
                .Arena;

            CombatSystem[] spetatorsSystem = arena.GetComponentsInChildren<CombatSystem>();
            
            for (int i = 0; i < data.Bursts; i++)
            {
                foreach (var combatSystem in spetatorsSystem)
                {
                    combatSystem.StartAttack(data.AssociatedAttack.GetAttack(),arena.Boss.transform);

                    yield return null;
                }

                yield return new WaitForSeconds(data.TimeBetweenBursts);
            }

            // for (int i = 0; i < data.ToSpawn; i++)
            // {
            //     GameObject rangedImp = PoolersManager.Instance.TryGetPooler(data.RangedImpPrefab).GetPooledObject();
            //
            //     Vector2 rand = Random.insideUnitCircle * 5f;
            //
            //     rangedImp.transform.position = arena.transform.position + new Vector3(rand.x, arena.transform.position.y, rand.y);
            //
            //     rangedImp.transform.rotation =
            //         Quaternion.LookRotation((arena.Boss.transform.position - rangedImp.transform.position).normalized,
            //             Vector3.up);
            //
            //     rangedImps.Add(rangedImp);
            // }
            //
            // yield return new WaitForSeconds(0.5f);
            //
            // foreach (var gameObject in rangedImps)
            // {
            //     gameObject.GetComponentInChildren<CombatSystem>().StartAttack(data.AssociatedAttack.GetAttack());
            // }
            //
            // yield return new WaitForSeconds(data.ActivatedDuration);
            //
            // foreach (var gameObject in rangedImps)
            // {
            //     PoolersManager.Instance.TryGetPooler(data.RangedImpPrefab).DeactivatePooledObject(gameObject);
            // }


            // GameObject hammer = PoolersManager.Instance.TryGetPooler(data.HammerPrefab).GetPooledObject();
            //
            // hammer.transform.position = groupTransform.position + new Vector3(0f,1f,0f);
            // hammer.transform.rotation = groupTransform.rotation;
            //
            // hammer.transform.SetParent(groupTransform);
            //
            // CombatSystem hammerCombatSystem = hammer.GetComponentInChildren<CombatSystem>();
            //
            // hammerCombatSystem.OnStopAttack += OnStopAttack;
            //
            // hammer.GetComponentInChildren<CombatSystem>().StartAttack(data.AssociatedAttack.GetAttack());
            //
            // yield return new WaitForSeconds(data.ActivatedDuration);
            //
            // PoolersManager.Instance.TryGetPooler(data.HammerPrefab).DeactivatePooledObject(hammer);
        }

        private void OnStopAttack()
        {
        }

        protected override void InnerSetup()
        {
        }

        protected override void InnerDispose()
        {
        }
    }
}