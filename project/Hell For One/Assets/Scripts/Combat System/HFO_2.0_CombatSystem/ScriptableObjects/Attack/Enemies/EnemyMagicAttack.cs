using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(fileName = "EnemyNormalAttack", menuName = "CombatSystem/Enemy/MagicAttack", order = 1)]
public class EnemyMagicAttack : EnemyAttack
{
    [SerializeField]
    private bool hasSustainedDamage = false;
    [SerializeField]
    private float sustainedDamageRateMin = 2.0f;
    [SerializeField]
    private float sustainedDamageRateMax = 4.0f;

    protected override ObjectsPooler GetPooler()
    {
        if (isRanged)
        {
            if (projectilePooler == null)
            {
                projectilePooler = GameObject.FindWithTag("MagicProjectiles").GetComponent<ObjectsPooler>();
            }

            return projectilePooler;
        }

        return null;
    }
    
    public override IEnumerator ManageHit(GenericIdle targetGenericIdle, IdleCollider targetIdleCollider, NormalCombat attackerNormalCombat,
        Action<GenericIdle> hitAction)
    {
        if (this.IsLegitAttack(targetGenericIdle))
        {
            if (hasSustainedDamage)
            {
                float timer = 0f;
            
                while (true)
                {
                    if (timer >= UnityEngine.Random.Range(sustainedDamageRateMin, sustainedDamageRateMax))
                    {
                        targetIdleCollider.NotifyOnNormalAttackBeingHit(attackerNormalCombat,this);

                        hitAction(targetGenericIdle);
                    }

                    timer += Time.deltaTime;

                    yield return null;
                }
            }
            else
            {
                targetIdleCollider.NotifyOnNormalAttackBeingHit(attackerNormalCombat,this);

                hitAction(targetGenericIdle);
                
                yield break;
            }
        }
    }
}
