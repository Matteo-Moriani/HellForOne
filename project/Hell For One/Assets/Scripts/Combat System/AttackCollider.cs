using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    private Stats stats;

    private Combat combat;

    private void Start()
    {
        stats = this.transform.root.gameObject.GetComponent<Stats>();
        combat = this.transform.root.gameObject.GetComponent<Combat>();
    }
    
    private void OnEnable()
    {
        if(stats == null) { 
            stats = this.transform.root.gameObject.GetComponent<Stats>();    
        }
        if(combat == null) {
            combat = this.transform.root.gameObject.GetComponent<Combat>();
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        ManageCollisionUsingType(other);    
    }

    private void ManageCollisionUsingType(Collider other) {
        GameObject targetRootGo = other.transform.root.gameObject;
        GameObject attackerRootGo = this.transform.root.gameObject;
        Stats targetRootStats = targetRootGo.GetComponent<Stats>();

        switch (stats.type) { 
            case Stats.Type.Player:
                if (targetRootStats.type == Stats.Type.Enemy || targetRootStats.type == Stats.Type.Boss)
                {
                    if (other.tag == "BlockCollider")
                    {
                        combat.StopAttack();
                    }
                    if (other.tag == "IdleCollider")
                    {
                        if (targetRootStats != null)
                        {
                            targetRootStats.TakeHit(stats.Damage);
                        }
                        else
                        {
                            Debug.Log("Target does not have Stats attached");
                        }
                        combat.StopAttack();
                    }
                }
                break;
            case Stats.Type.Ally:
                if (targetRootStats.type == Stats.Type.Enemy || targetRootStats.type == Stats.Type.Boss)
                {
                    if (other.tag == "BlockCollider")
                    {
                        if (targetRootStats != null)
                        {
                            if (Random.Range(1, 101) <= stats.AttackChance - targetRootStats.BlockChanceBonus)
                            {
                                targetRootStats.TakeHit(stats.Damage);
                            }

                            combat.StopAttack();
                        }
                    }
                    if (other.tag == "IdleCollider")
                    {
                        if (targetRootStats != null)
                        {
                            if (Random.Range(1, 101) <= stats.AttackChance)
                            {
                                targetRootStats.TakeHit(stats.Damage);
                            }
                        }
                        combat.StopAttack();
                    }
                }
                break;
            case Stats.Type.Enemy:
                if (targetRootStats.type == Stats.Type.Player || targetRootStats.type == Stats.Type.Ally)
                {
                    if (other.tag == "BlockCollider")
                    {
                        if (targetRootStats != null)
                        {
                            if (Random.Range(1, 101) <= stats.AttackChance - targetRootStats.BlockChanceBonus)
                            {
                                targetRootStats.TakeHit(stats.Damage);
                            }
                            combat.StopAttack();
                        }
                    }
                    if (other.tag == "IdleCollider")
                    {
                        if (targetRootStats != null)
                        {
                            if (Random.Range(1, 101) <= stats.AttackChance)
                            {
                                targetRootStats.TakeHit(stats.Damage);
                            }
                            combat.StopAttack();
                        }
                    }
                }
                break;
            case Stats.Type.Boss:
                if (targetRootStats.type == Stats.Type.Player || targetRootStats.type == Stats.Type.Ally) 
                {
                    if(other.tag == "BlockCollider") 
                    { 
                        if(targetRootStats != null) 
                        {
                            if (Random.Range(1, 101) <= stats.AttackChance - targetRootStats.BlockChanceBonus)
                            {
                                targetRootStats.TakeHit(stats.Damage);
                                if(Random.Range(1,101) <= stats.KnockBackChance) 
                                {
                                    targetRootStats.KnockBack( stats.KnockBackUnits, this.transform.root );
                                }
                            }
                            combat.StopAttack();
                        }   
                    }
                    if(other.tag == "IdleCollider") 
                    {
                        if (targetRootStats != null)
                        {
                            if (Random.Range(1, 101) <= stats.AttackChance)
                            {
                                targetRootStats.TakeHit(stats.Damage);
                                if (Random.Range(1, 101) <= stats.KnockBackChance)
                                {
                                    targetRootStats.KnockBack( stats.KnockBackUnits, this.transform.root );
                                }
                            }
                            combat.StopAttack();
                        }
                    }
                }
                break;
        }
    }
}
