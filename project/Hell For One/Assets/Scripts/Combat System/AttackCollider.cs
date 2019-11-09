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

    [SerializeField]
    private int aggroModifier = 1;

    public bool isSweeping = false;
    public bool isGlobalAttacking = false;

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

        //TODO-Insert here targetStats !null check
        //TODO-Insert here idleCollider tag check

        switch (stats.type) {
            case Stats.Type.Player:
                if (targetRootStats.type == Stats.Type.Enemy || targetRootStats.type == Stats.Type.Boss)
                {
                    if (other.tag == "IdleCollider")
                    {
                        if (targetRootStats.IsBlocking) 
                        { 
                            if(CheckAngle(other.gameObject.transform.root)) { 
                                targetRootStats.TakeHit(stats.Damage);
                                
                                ManageAggro();

                                if (!isSweeping && !isGlobalAttacking)
                                    StopAttack();
                            }
                            else { 
                                ManageAggro();
                                if(!isSweeping && !isGlobalAttacking)
                                    StopAttack();
                            }
                        }
                        if (!targetRootStats.IsBlocking) 
                        {
                            targetRootStats.TakeHit(stats.Damage);

                            ManageAggro();

                            if (!isSweeping && !isGlobalAttacking)
                                StopAttack();
                        }
                    }
                }
                break;
            case Stats.Type.Ally:
                if (targetRootStats.type == Stats.Type.Enemy || targetRootStats.type == Stats.Type.Boss)
                {
                    if (other.tag == "IdleCollider")
                    {
                        if (targetRootStats.IsBlocking) {

                            if (CheckAngle(other.gameObject.transform.root)) {
                                if (targetRootStats.CalculateBeenHitChance(false))
                                {
                                    targetRootStats.TakeHit(stats.Damage);
                                }
                                ManageAggro();

                                if (!isSweeping && !isGlobalAttacking)
                                    StopAttack();
                            }
                            else {
                                if (targetRootStats.CalculateBeenHitChance(true))
                                {
                                    targetRootStats.TakeHit(stats.Damage);
                                }
                                ManageAggro();

                                if (!isSweeping && !isGlobalAttacking)
                                    StopAttack();
                            }
                        }
                        if (!targetRootStats.IsBlocking) {
                            if (targetRootStats.CalculateBeenHitChance(false))
                            {
                                targetRootStats.TakeHit(stats.Damage);
                            }
                            ManageAggro();

                            if (!isSweeping && !isGlobalAttacking)
                                StopAttack();
                        }
                    }
                }
                break;
            case Stats.Type.Enemy:
                if (targetRootStats.type == Stats.Type.Player || targetRootStats.type == Stats.Type.Ally)
                {
                    if (other.tag == "IdleCollider")
                    {
                        if (targetRootStats.IsBlocking)
                        {

                            if (CheckAngle(other.gameObject.transform.root))
                            {
                                if (targetRootStats.CalculateBeenHitChance(false))
                                {
                                    targetRootStats.TakeHit(stats.Damage);
                                }
                                ManageAggro();

                                if (!isSweeping && !isGlobalAttacking)
                                    StopAttack();
                            }
                            else
                            {
                                if (targetRootStats.CalculateBeenHitChance(true))
                                {
                                    targetRootStats.TakeHit(stats.Damage);
                                }
                                ManageAggro();

                                if (!isSweeping && !isGlobalAttacking)
                                    StopAttack();
                            }
                        }
                        if (!targetRootStats.IsBlocking)
                        {
                            if (targetRootStats.CalculateBeenHitChance(false))
                            {
                                targetRootStats.TakeHit(stats.Damage);
                            }
                            ManageAggro();

                            if (!isSweeping && !isGlobalAttacking)
                                StopAttack();
                        }
                    }
                }
                break;
            case Stats.Type.Boss:
                if (targetRootStats.type == Stats.Type.Player || targetRootStats.type == Stats.Type.Ally) 
                {  
                    if (other.tag == "IdleCollider")
                    {
                        if (targetRootStats.IsBlocking)
                        {

                            if (CheckAngle(other.gameObject.transform.root))
                            {
                                if (targetRootStats.CalculateBeenHitChance(false))
                                {
                                    targetRootStats.TakeHit(stats.Damage);

                                    if (Random.Range(1f, 101f) <= stats.KnockBackChance && !isGlobalAttacking)
                                    {
                                        targetRootStats.TakeKnockBack(stats.KnockBackUnits, this.transform.root);
                                    }
                                }
                                ManageAggro();

                                if (!isSweeping && !isGlobalAttacking)
                                    StopAttack();
                            }
                            else
                            {
                                if (targetRootStats.CalculateBeenHitChance(true))
                                {
                                    targetRootStats.TakeHit(stats.Damage);

                                    if (Random.Range(1f, 101f) <= stats.KnockBackChance && !isGlobalAttacking)
                                    {
                                        targetRootStats.TakeKnockBack(stats.KnockBackUnits, this.transform.root);
                                    }
                                }
                                ManageAggro();

                                if (!isSweeping && !isGlobalAttacking)
                                    StopAttack();
                            }
                        }
                        if (!targetRootStats.IsBlocking)
                        {
                            if (targetRootStats.CalculateBeenHitChance(false))
                            {
                                targetRootStats.TakeHit(stats.Damage);

                                if (Random.Range(1f, 101f) <= stats.KnockBackChance && !isGlobalAttacking)
                                {
                                    targetRootStats.TakeKnockBack(stats.KnockBackUnits, this.transform.root);
                                }
                            }
                            ManageAggro();

                            if (!isSweeping && !isGlobalAttacking)
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

    private void ManageAggro() {
        switch (this.type) { 
            case AttackColliderType.Melee:
                stats.RaiseAggro(aggroModifier);
                break;
            case AttackColliderType.Ranged:
                stats.RaiseAggro(aggroModifier);
                break;
            case AttackColliderType.None:
                Debug.Log(this.name + "AttackCollider.tyoe is set to None");
                break;
        }    
    }

    private bool CheckAngle(Transform other) { 
        return Vector3.Angle(this.transform.root.transform.forward, other.forward) < 91;
    }
}
