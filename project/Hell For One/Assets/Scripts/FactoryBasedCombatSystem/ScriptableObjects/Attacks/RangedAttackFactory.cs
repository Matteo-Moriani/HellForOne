using System;
using System.Collections;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;
using Utils.ObjectPooling;

namespace FactoryBasedCombatSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = ("CombatSystem/Attacks/MeleeAttack"),fileName = "MeleeAttack", order = 1)]
    public class RangedAttackFactory : AttackFactory<RangedAttack,RangedAttackData> { }

    [Serializable]
    public class RangedAttackData : AttackData
    {
        [SerializeField] private GameObject projectilePrefab;

        [Space] [Header("Projectile movement")] 
        
        [SerializeField, Min(0f)] private float destroyTime;
        [SerializeField, Min(0f)] private float projectileSpeed;
        [SerializeField, Min(0f)] private float minDistance;
        [SerializeField, Min(0f)] private float maxDistance;
        [SerializeField] private bool stopsOnHit;
        
        public GameObject ProjectilePrefab
        {
            get => projectilePrefab;
            private set => projectilePrefab = value;
        }

        public float ProjectileSpeed
        {
            get => projectileSpeed;
            private set => projectileSpeed = value;
        }

        public float MinDistance
        {
            get => minDistance;
            private set => minDistance = value;
        }

        public float MaxDistance
        {
            get => maxDistance;
            private set => maxDistance = value;
        }

        public float DestroyTime
        {
            get => destroyTime;
            private set => destroyTime = value;
        }
    }

    public class RangedAttack : Attack<RangedAttackData>
    {
        protected override IEnumerator InnerDoAttack(CombatSystem ownerCombatSystem, Transform target)
        {
            // TODO :- check if it's ok to break like this
            if(target == null) yield break;

            while (!StartAttack) yield return null;

            GameObject projectile = PoolersManager.Instance.TryGetPooler(data.ProjectilePrefab).GetPooledObject(data.DestroyTime);
            
            AttackCollider projectileAttackCollider = projectile.GetComponentInChildren<AttackCollider>();
            projectileAttackCollider.Initialize(this,ownerCombatSystem.transform.root,ownerCombatSystem);
            projectileAttackCollider.SetSize(data.ColliderRadius);
            
            projectile.SetActive(true);

            ProjectileMovement projectileMovement = projectile.GetComponent<ProjectileMovement>();
            
            if(!
                projectileMovement.TryLaunch(
                    target,
                    ownerCombatSystem.ProjectileAnchor,
                    data.MinDistance,
                    data.MaxDistance,
                    data.ProjectileSpeed)
            ) yield break;

            
            float timer = 0.0f;

            while (timer <= data.DestroyTime)
            {
                if (projectileAttackCollider.HasHit)
                {
                    projectileMovement.Stop();

                    if (data.SplashDamage)
                    {
                        projectileAttackCollider.gameObject.transform.localScale = Vector3.one * data.SplashDamageRadius;
                    
                        yield return new WaitForSeconds(0.2f);
                    }
                    
                    yield break;
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}