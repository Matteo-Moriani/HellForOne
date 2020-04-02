﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    #region fields

    private enum AttackColliderType
    {
        Ranged,
        Melee,
        None
    }

    [SerializeField]
    AttackColliderType type = AttackColliderType.None;

    public bool isGroupAttacking = false;
    public bool isGlobalAttacking = false;

    private Stats stats;

    private Combat combat;

    private GroupFinder groupFinder;

    public float meleeAggroModifier = 1.1f;
    public float rangeAggroModifier = 1.05f;

    CombatEventsManager combatEventsManager;

    #endregion

    #region methods

    public void SetStats(Stats value)
    {
        stats = value;
    }

    private void Start()
    {
        if (stats == null)
        {
            stats = this.transform.root.gameObject.GetComponent<Stats>();
        }

        combat = this.transform.root.gameObject.GetComponent<Combat>();

        // I'm using stats because if this collider belongs to a lance
        // transform.root won't be the demon that is launching the lance
        combatEventsManager = stats.gameObject.GetComponent<CombatEventsManager>();
    }

    private void OnEnable()
    {
        if (stats == null)
        {
            stats = this.transform.root.gameObject.GetComponent<Stats>();
        }
        if (combat == null)
        {
            combat = this.transform.root.gameObject.GetComponent<Combat>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ManageCollisionUsingType(other);
    }

    private void ManageCollisionUsingType(Collider other)
    {
        Stats targetRootStats = other.transform.root.gameObject.GetComponent<Stats>();
        if (targetRootStats != null)
        {
            if (other.tag == "IdleCollider")
            {
                switch (stats.ThisUnitType)
                {
                    case Stats.Type.Player:
                        if (targetRootStats.ThisUnitType == Stats.Type.Enemy || targetRootStats.ThisUnitType == Stats.Type.Boss)
                        {
                            ManagePlayerCollisions(targetRootStats, other);
                        }
                        break;
                    case Stats.Type.Ally:
                        if (targetRootStats.ThisUnitType == Stats.Type.Enemy || targetRootStats.ThisUnitType == Stats.Type.Boss)
                        {
                            ManageSimpleDemonCollisions(targetRootStats, other);
                        }
                        break;
                    case Stats.Type.Enemy:
                        if (targetRootStats.ThisUnitType == Stats.Type.Player || targetRootStats.ThisUnitType == Stats.Type.Ally)
                        {
                            ManageSimpleDemonCollisions(targetRootStats, other);
                        }
                        break;
                    case Stats.Type.Boss:
                        if (targetRootStats.ThisUnitType == Stats.Type.Player || targetRootStats.ThisUnitType == Stats.Type.Ally)
                        {
                            ManageBossCollisions(targetRootStats, other);
                        }
                        break;
                }
            }
        }
        else
        {
            //Debug.Log(transform.root.gameObject.name + " is trying to hit a target without stats. target is: " + other.transform.root.gameObject.name);
        }
    }

    private void StopAttack()
    {
        switch (this.type)
        {
            case AttackColliderType.Melee:
                //if (!isGlobalAttacking && !isGroupAttacking)
                    //combat.StopAttack();
                break;
            case AttackColliderType.Ranged:
                this.gameObject.SetActive(false);
                break;
            case AttackColliderType.None:
                //Debug.Log(this.name + "AttackCollider.tyoe is set to None");
                break;
        }
    }

    private void ManageAggro()
    {
        if (this.type != AttackColliderType.None)
        {

            float aggroModifier = 1;

            if (type == AttackColliderType.Melee)
            {
                aggroModifier = meleeAggroModifier;
            }
            if (type == AttackColliderType.Ranged)
            {
                aggroModifier = rangeAggroModifier;
            }

            // We update Group aggro only for Ally Imps
            if (stats.ThisUnitType == Stats.Type.Ally)
            {
                if (type != AttackColliderType.Ranged && groupFinder == null)
                {
                    groupFinder = stats.gameObject.GetComponent<GroupFinder>();
                }

                // We need to update demonBehaviour every ranged attack because
                // lances are pooled, so owner can change every time
                if (type == AttackColliderType.Ranged)
                {
                    groupFinder = stats.gameObject.GetComponent<GroupFinder>();
                }

                if (groupFinder != null)
                {
                    //groupFinder.GroupBelongingTo.GetComponent<GroupAggro>().RaiseGroupAggro((aggroModifier - 1f) * stats.Aggro);
                }
            }

            //stats.RaiseAggro(aggroModifier);
        }
        //else
        //{
        //    Debug.Log(this.name + "AttackCollider.type is set to None");
        //}
    }

    /// <summary>
    /// Checks the angle between the attacker front direction and the target front direction
    /// </summary>
    /// <param name="other">The target transform</param>
    /// <returns></returns>
    private bool CheckAngle(Transform other)
    {
        return Vector3.Angle(this.transform.root.transform.forward, other.forward) < 91;
    }

    private void ManageKnockBack(Stats targetRootStats)
    {
        KnockbackReceiver knockbackReceiver = targetRootStats.gameObject.GetComponent<KnockbackReceiver>();
        KnockbackCaster knockbackCaster = this.transform.root.gameObject.GetComponent<KnockbackCaster>();

        if (knockbackReceiver != null && knockbackCaster != null)
        {
            // If we can deal a knockback
            //if (Random.Range(1f, 101f) <= knockbackCaster.KnockBackChance)
            if(true)
            {
                // For non player-target
                if (targetRootStats.ThisUnitType != Stats.Type.Player)
                {
                    // If we are dealing a sweep attack (heavy attack)
                    if (isGroupAttacking)
                    {
                        // If we are hitting an unit that is not blocking
                        if (!targetRootStats.IsBlocking)
                        {
                            //knockbackReceiver.TakeKnockBack(knockbackCaster.KnockBackSize, this.transform.root, knockbackCaster.KnockBackTime);
                        }
                        // If target is blocking we have to understand the angle to know if we have to deal a knockback or not
                        if (targetRootStats.IsBlocking)
                        {
                            if (CheckAngle(targetRootStats.gameObject.transform))
                            {
                                //knockbackReceiver.TakeKnockBack(knockbackCaster.KnockBackSize, this.transform.root, knockbackCaster.KnockBackTime);
                            }
                        }
                    }
                    // for any other attack we knockback only if the target is not blocking
                    if (!targetRootStats.IsBlocking)
                    {
                        //knockbackReceiver.TakeKnockBack(knockbackCaster.KnockBackSize, this.transform.root, knockbackCaster.KnockBackTime);
                    }
                }
                
                if(targetRootStats.ThisUnitType == Stats.Type.Player) {
                    if (!isGlobalAttacking) { 
                        //knockbackReceiver.TakeKnockBack(knockbackCaster.KnockBackSize, this.transform.root, knockbackCaster.KnockBackTime);
                    }
                }
            }
        }
        else
        {
            if (knockbackReceiver == null)
                Debug.LogError(this.transform.root.gameObject.name + " cannot find KnockbackReceiver in " + targetRootStats.gameObject.name);
            if (knockbackCaster == null)
                Debug.LogError(this.transform.root.gameObject.name + " cannot find KnockbackCaster");
        }
    }

    // TODO - Register this stuff as event?
    private void ManageHit(Stats targetRootStats)
    {
        DealDamage(targetRootStats);

        combatEventsManager.RaiseOnSuccessfulHit();

        targetRootStats.gameObject.GetComponent<CombatEventsManager>().RaiseOnBeenHit(stats);

        ManageAggro();

        StopAttack();
    }

    // TODO - Implement this
    private void ManageMiss() { }

    // TODO - Implement this
    private void ManageBlock(Stats targetRootStats)
    {
        //combatEventsManager.RaiseOnBlockedHit();

        targetRootStats.gameObject.GetComponent<CombatEventsManager>().RaiseOnBlockedHit();

        ManageAggro();

        StopAttack();
    }

    private void ManagePlayerCollisions(Stats targetRootStats, Collider other)
    {
        if (targetRootStats.IsBlocking)
        {
            if (CheckAngle(other.gameObject.transform.root))
            {
                ManageHit(targetRootStats);

                return;
            }
            else
            {
                ManageBlock(targetRootStats);

                return;
            }
        }
        if (!targetRootStats.IsBlocking)
        {
            ManageHit(targetRootStats);

            return;
        }
    }

    
    private void ManageSimpleDemonCollisions(Stats targetRootStats, Collider other)
    {
        if (targetRootStats.IsBlocking)
        {
            if (CheckAngle(other.gameObject.transform.root))
            {
                /*
                if (targetRootStats.CalculateBeenHitChance(false))
                {
                    ManageHit(targetRootStats);

                    return;
                }
                else
                {
                    ManageBlock(targetRootStats);

                    return;
                }
                */
            }
            else
            {
                /*
                if (targetRootStats.CalculateBeenHitChance(true))
                {
                    ManageHit(targetRootStats);

                    return;
                }
                else
                {
                    ManageBlock(targetRootStats);

                    return;
                }
                */
            }
        }
        if (!targetRootStats.IsBlocking)
        {
            /*
            if (targetRootStats.CalculateBeenHitChance(false))
            {
                ManageHit(targetRootStats);

                return;
            }
            else
            {
                ManageBlock(targetRootStats);

                return;
            }
            */
        }
    }

    private void ManageBossCollisions(Stats targetRootStats, Collider other)
    {
        if (targetRootStats.IsBlocking)
        {
            /*
            // Player cannot block sweeping (heavy attack)
            if (isGroupAttacking && targetRootStats.type == Stats.Type.Player)
            {
                ManageHit(targetRootStats);

                ManageKnockBack(targetRootStats);

                return;
            }
            */
            
    
            /*
            // We will take care of global attack in another way
            if (!isGlobalAttacking)
            {
                // if target is blocking but is not looking towards the boss
                if (CheckAngle(other.gameObject.transform.root))
                {
                    // calculate been hit chance without counting block bonus
                    if (targetRootStats.CalculateBeenHitChance(false))
                    {
                        ManageHit(targetRootStats);

                        ManageKnockBack(targetRootStats);

                        return;
                    }
                    else
                    {
                        ManageBlock(targetRootStats);

                        return;
                    }
                }
                // if target is blocking and is looking towards the boss
                else
                {
                    // calculate been hit chance counting block bonus
                    if (targetRootStats.CalculateBeenHitChance(true))
                    {
                        ManageHit(targetRootStats);

                        ManageKnockBack(targetRootStats);

                        return;
                    }
                    else
                    {
                        ManageBlock(targetRootStats);

                        return;
                    }
                }
            }
            */
            /*
            if (isGlobalAttacking)
            {
                // this is like CheckAngle, but now we are checking
                // the angle between the target view direction and the attacker position

                // If the target is blocking but is not looking towards the boss...
                if (!(Vector3.Angle(other.gameObject.transform.root.forward, this.transform.root.position - other.gameObject.transform.root.position) < 45))
                {
                    if (targetRootStats.CalculateBeenHitChance(false))
                    {
                        ManageHit(targetRootStats);

                        ManageKnockBack(targetRootStats);

                        return;
                    }
                    else
                    {
                        ManageBlock(targetRootStats);
                    }
                }
                else
                {
                    if (targetRootStats.CalculateBeenHitChance(true))
                    {
                        ManageHit(targetRootStats);

                        ManageKnockBack(targetRootStats);

                        return;
                    }
                    else
                    {
                        ManageBlock(targetRootStats);

                        return;
                    }
                }
            }
            */
        }
        if (!targetRootStats.IsBlocking)
        {
            /*
            // Calculate been hit chance without counting block bonus
            if (targetRootStats.CalculateBeenHitChance(false))
            {
                ManageHit(targetRootStats);

                ManageKnockBack(targetRootStats);

                return;
            }
            else
            {
                ManageBlock(targetRootStats);

                return;
            }
            */
        }
    }
    

    private void DealDamage(Stats targetRootStats)
    {
        // If the player or an Ally has to deal damage
        if (stats.ThisUnitType == Stats.Type.Player || stats.ThisUnitType == Stats.Type.Ally)
        {

            float damage = 0f;

            // if attacking demons are 0 we have a divison by 0, BEWARE
            float attackingDemons = 0;
            int supportingDemons = 0;

            // Calculate all attacking and supporting demons
            if (GroupsManager.Instance.Groups != null)
            {
                foreach (GameObject group in GroupsManager.Instance.Groups)
                {
                    GroupBehaviour gb = group.GetComponent<GroupBehaviour>();
                    GroupManager groupManager = group.GetComponent<GroupManager>();
                    
                    if (gb != null && groupManager != null)
                    {
                        if (gb.currentState == GroupBehaviour.State.MeleeAttack || gb.currentState == GroupBehaviour.State.RangeAttack)
                        {
                            attackingDemons += groupManager.ImpsInGroupNumber;
                        }
                        if (gb.currentState == GroupBehaviour.State.Support)
                        {
                            supportingDemons += groupManager.ImpsInGroupNumber;
                        }
                    }
                }
            }

            // Avoid division by 0
            if (attackingDemons == 0)
            {
                //attackingDemons = stats.SupportDamageBuffMultiplier;
            }

            // Calculate damage
            if (type == AttackColliderType.Melee)
            {
                //damage = supportingDemons * (stats.SupportDamageBuffMultiplier / attackingDemons) + stats.MeleeDamage;
            }
            if (type == AttackColliderType.Ranged)
            {
                //damage = supportingDemons * (stats.SupportDamageBuffMultiplier / attackingDemons) + stats.RangedDamage;
            }
            if (type == AttackColliderType.None)
            {
                //Debug.Log(this.transform.root.gameObject.name + " attack collide type not set");
            }

            // Deal damage
            targetRootStats.TakeHit(damage);
        }

        // If an Enemy or a Boss has to deal damage
        if (stats.ThisUnitType == Stats.Type.Enemy || stats.ThisUnitType == Stats.Type.Boss)
        {
            if (type == AttackColliderType.Melee)
            {
                //targetRootStats.TakeHit(stats.MeleeDamage);
            }
            if (type == AttackColliderType.Ranged)
            {
                //targetRootStats.TakeHit(stats.RangedDamage);
            }
            if (type == AttackColliderType.None)
            {
                //Debug.Log(this.transform.root.gameObject.name + " attack collide type not set");
            }
        }
    }

    #endregion
}
