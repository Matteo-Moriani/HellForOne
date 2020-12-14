using AI.Imp;
using GroupSystem;
using ReincarnationSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.ObjectPooling;

namespace FactoryBasedCombatSystem.ScriptableObjects.Attacks
{
    [CreateAssetMenu(menuName = ("CombatSystem/Attacks/BossEnvironmentalAttack"), fileName = "BossEnvironmentalAttack", order = 1)]
    public class BossEnvironmentalAttackFactory : BossAttackFactory<BossEnvironmentalAttack, BossEnvironmentalAttackData> { }

    [Serializable]
    public class BossEnvironmentalAttackData : BossAttackData
    {
        [SerializeField] private GameObject attackPrefab;
        [SerializeField] private float maxDistVariation = 2f;
        [SerializeField] private float duration = 5f;

        // 5 colonne di fuoco: una sul player e 4 sui gruppi

        public GameObject AttackPrefab
        {
            get => attackPrefab;
            private set => attackPrefab = value;
        }
        public float MaxDistVariation { get => maxDistVariation; set => maxDistVariation = value; }
        public float Duration { get => duration; set => duration = value; }
    }

    public class BossEnvironmentalAttack : BossAttack<BossEnvironmentalAttackData>
    {
        private readonly Dictionary<int, List<GameObject>> _attackGameObjects = new Dictionary<int, List<GameObject>>();

        protected override IEnumerator InnerDoAttack(int id, CombatSystem ownerCombatSystem, Transform target)
        {
            CapsuleCollider bossCollider = ownerCombatSystem.GetComponentInChildren<CapsuleCollider>();

            foreach(GameObject g in _attackGameObjects[id])
            {
                g.transform.position += (ownerCombatSystem.transform.position - g.transform.position).normalized * UnityEngine.Random.Range(-data.MaxDistVariation, data.MaxDistVariation);

                if((ownerCombatSystem.transform.position - g.transform.position).magnitude < bossCollider.radius + data.ColliderRadius)
                {
                    g.transform.position += (ownerCombatSystem.transform.position - g.transform.position).normalized * (bossCollider.radius + data.ColliderRadius);
                }
            }

            while(!AnimationStates[id]) yield return null;

            // sul vettore/retta che collega boss a gruppo e player, istanzio la colonna sopra il gruppo più o meno un valore casuale
            foreach(GameObject g in _attackGameObjects[id])
            {
                g.GetComponentInChildren<AttackCollider>().Initialize(id, data.ColliderRadius, this, ownerCombatSystem.transform.root, ownerCombatSystem);
                g.GetComponent<EnvironmentalAttackBehaviour>().Activate();
            }

            float timer = 0f;

            while(timer < data.Duration)
            {
                timer += Time.deltaTime;
                yield return null;

                if(!HasHit[id]) continue;
                if(!data.CanDamageMultipleUnits) break;
            }

            foreach(GameObject g in _attackGameObjects[id])
            {
                g.GetComponent<EnvironmentalAttackBehaviour>().Deactivate();
            }
        }

        protected override void InnerSetup(int id, CombatSystem ownerCombatSystem, Transform target)
        {
            _attackGameObjects.Add(id, new List<GameObject>());

            GameObject playerTargetedGameObject = PoolersManager.Instance.TryGetPooler(data.AttackPrefab).GetPooledObject();
            playerTargetedGameObject.transform.position = ReincarnationManager.Instance.CurrentLeader.transform.position;
            _attackGameObjects[id].Add(playerTargetedGameObject);

            List<Vector3> groupPositions = new List<Vector3>();
            foreach(GameObject group in GroupsManager.Instance.Groups)
            {
                groupPositions.Add(group.GetComponent<GroupMeanPosition>().MeanPosition);
            }

            foreach(Vector3 position in groupPositions)
            {
                GameObject groupTargetedGameObject = PoolersManager.Instance.TryGetPooler(data.AttackPrefab).GetPooledObject();
                groupTargetedGameObject.transform.position = position;
                _attackGameObjects[id].Add(groupTargetedGameObject);
            }
        }

        protected override void InnerDispose(int id, CombatSystem ownerCombatSystem)
        {
            foreach(GameObject g in _attackGameObjects[id])
            {
                PoolersManager.Instance.TryGetPooler(data.AttackPrefab).DeactivatePooledObject(g);
            }
            _attackGameObjects.Remove(id);
        }
    }
}