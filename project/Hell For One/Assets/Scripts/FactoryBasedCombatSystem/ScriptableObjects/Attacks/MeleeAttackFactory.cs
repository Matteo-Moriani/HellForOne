using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.ObjectPooling;

namespace FactoryBasedCombatSystem.ScriptableObjects.Attacks
{
    [CreateAssetMenu(menuName = ("CombatSystem/Attacks/MeleeAttack"),fileName = "MeleeAttack", order = 1)]
    public class MeleeAttackFactory : AttackFactory<MeleeAttack,MeleeAttackData> { }

    [Serializable]
    public class MeleeAttackData : AttackData
    {
        [SerializeField] private float range;
        [SerializeField] private GameObject attackPrefab;
        
        public float Range
        {
            get => range;
            private set => range = value;
        }

        public GameObject AttackPrefab
        {
            get => attackPrefab;
            private set => attackPrefab = value;
        }
    }

    public class MeleeAttack : Attack<MeleeAttackData>
    { 
        private readonly Dictionary<int,GameObject> _attackGameObjects = new Dictionary<int, GameObject>();
        
        protected override IEnumerator InnerDoAttack(int id,CombatSystem ownerCombatSystem, Transform target)
        {
            while (!AnimationStates[id]) yield return null;

            AttackCollider attackCollider = _attackGameObjects[id].GetComponentInChildren<AttackCollider>();
            attackCollider.Initialize(id,data.ColliderRadius,this,ownerCombatSystem.transform.root,ownerCombatSystem);
            
            _attackGameObjects[id].transform.position = ownerCombatSystem.transform.position + (ownerCombatSystem.transform.forward * data.Range);

            while (AnimationStates[id])
            {
                yield return null;
                
                if (HasHit[id])
                {
                    if(!data.CanDamageMultipleUnits) yield break;

                    if(!data.SplashDamage) continue;

                    float timer = 0f;
                    _attackGameObjects[id].GetComponentInChildren<AttackCollider>().SetRadius(data.SplashDamageRadius);
                
                    while (timer < data.SplashDamageTime)
                    {
                        yield return null;
                        timer += Time.deltaTime;
                    }    
                }
            }
        }

        protected override void InnerSetup(int id, CombatSystem ownerCombatSystem, Transform target)
        {
            GameObject attackGameObject = PoolersManager.Instance.TryGetPooler(data.AttackPrefab).GetPooledObject();
            _attackGameObjects.Add(id,attackGameObject);
        }

        protected override void InnerDispose(int id, CombatSystem ownerCombatSystem)
        {
            PoolersManager.Instance.TryGetPooler(data.AttackPrefab).DeactivatePooledObject(_attackGameObjects[id]);
            _attackGameObjects.Remove(id);
        }
    }
}