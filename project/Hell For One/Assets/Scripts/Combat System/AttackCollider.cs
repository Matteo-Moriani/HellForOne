using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    private Stats stats;

    [SerializeField]
    private CombatManager combatManager;

    private void Start()
    {
        stats = this.transform.root.gameObject.GetComponent<Stats>();
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
                        combatManager.StopAttack();
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
                        combatManager.StopAttack();
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

                            combatManager.StopAttack();
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
                        combatManager.StopAttack();
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
                            combatManager.StopAttack();
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
                            combatManager.StopAttack();
                        }
                    }
                }
                break;
            case Stats.Type.Boss:
                Debug.Log("AttackCollider - TODO Implement boss attack");
                break;
        }
    }

    private void ManageCollisionUsingTag(Collider other) {
        GameObject targetRootGo = other.transform.root.gameObject;
        GameObject attackerRootGo = this.transform.root.gameObject;
        Stats targetRootStats = targetRootGo.GetComponent<Stats>();

        if (attackerRootGo.tag == "Player")
        {
            if (targetRootGo.tag != "Player" && targetRootGo.tag != "Ally")
            {
                if (other.tag == "BlockCollider")
                {
                    combatManager.StopAttack();
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
                    combatManager.StopAttack();
                }
            }
        }
        if (attackerRootGo.tag == "Ally")
        {
            if (targetRootGo.tag != "Player" && targetRootGo.tag != "Ally")
            {
                if (other.tag == "BlockCollider")
                {
                    if (targetRootStats != null)
                    {
                        if (Random.Range(1, 101) <= stats.AttackChance - targetRootStats.BlockChanceBonus)
                        {
                            targetRootStats.TakeHit(stats.Damage);
                        }

                        combatManager.StopAttack();
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
                    combatManager.StopAttack();
                }
            }
        }
        if (attackerRootGo.tag == "Enemy")
        {
            if (targetRootGo.tag != "Enemy")
            {
                if (other.tag == "BlockCollider")
                {
                    if (targetRootStats != null)
                    {
                        if (Random.Range(1, 101) <= stats.AttackChance - targetRootStats.BlockChanceBonus)
                        {
                            targetRootStats.TakeHit(stats.Damage);
                        }
                        combatManager.StopAttack();
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
                        combatManager.StopAttack();
                    }
                }
            }
        }
    }
}
