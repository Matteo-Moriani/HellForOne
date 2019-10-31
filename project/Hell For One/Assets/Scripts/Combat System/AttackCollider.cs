using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    private enum AttackColliderType{ 
        Ranged,
        Melee,
        None
    }

    [SerializeField]
    AttackColliderType type = AttackColliderType.None;

    private Stats stats;

    private Combat combat;

    //-TODO----
    // Add isRanged and isMelee bool
    // set them from inspector

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
        Stats targetRootStats = other.transform.root.gameObject.GetComponent<Stats>();

        switch (stats.type) {
            case Stats.Type.Player:
                if (targetRootStats.type == Stats.Type.Enemy || targetRootStats.type == Stats.Type.Boss)
                {
                    // If target is blocking player will never hit
                    if (other.tag == "BlockCollider")
                    {
                        StopAttack();   
                    }
                    // If target is not blocking player will allways hit
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
                        StopAttack();
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
                            if (targetRootStats.CalculateBeenHitChance(true))
                            {
                                targetRootStats.TakeHit(stats.Damage);
                            }

                            StopAttack();
                        }
                    }
                    if (other.tag == "IdleCollider")
                    {
                        if (targetRootStats != null)
                        {
                            if (targetRootStats.CalculateBeenHitChance(false))
                            {
                                targetRootStats.TakeHit(stats.Damage);
                            }
                        }
                        StopAttack();
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
                            if (targetRootStats.CalculateBeenHitChance(true))
                            {
                                targetRootStats.TakeHit(stats.Damage);
                            }
                            StopAttack();
                        }
                    }
                    if (other.tag == "IdleCollider")
                    {
                        if (targetRootStats != null)
                        {
                            if (targetRootStats.CalculateBeenHitChance(false))
                            {
                                targetRootStats.TakeHit(stats.Damage);
                            }
                            StopAttack();
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
                            if (targetRootStats.CalculateBeenHitChance(true))
                            {
                                targetRootStats.TakeHit(stats.Damage);
                                // TODO - Manage knockBack chance calculation in Stas?
                                if(Random.Range(1f,101f) <= stats.KnockBackChance) 
                                {
                                    targetRootStats.TakeKnockBack( stats.KnockBackUnits, this.transform.root );
                                }
                            }
                            StopAttack();
                        }   
                    }
                    if(other.tag == "IdleCollider") 
                    {
                        if (targetRootStats != null)
                        {
                            if (targetRootStats.CalculateBeenHitChance(false))
                            {
                                targetRootStats.TakeHit(stats.Damage);
                                // TODO - Manage knockBack chance calculation in Stas?
                                if (Random.Range(1f, 101f) <= stats.KnockBackChance)
                                {
                                    targetRootStats.TakeKnockBack( stats.KnockBackUnits, this.transform.root );
                                }
                            }
                            StopAttack();
                        }
                    }
                }
                break;
        }
    }

    private void StopAttack() {
        switch (this.type)
        {
            case AttackColliderType.Melee:
                combat.StopAttack();
                break;
            case AttackColliderType.Ranged:
                this.gameObject.SetActive(false);
                break;
            case AttackColliderType.None:
                Debug.Log(this.name + "AttackCollider.tyoe is set to None");
                break;
        }
    }
}
