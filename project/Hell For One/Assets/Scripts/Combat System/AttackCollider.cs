using System.Collections;
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

    public bool isSweeping = false;
    public bool isGlobalAttacking = false;

    private Stats stats;

    private Combat combat;

    private DemonBehaviour demonBehaviour;

    public float meleeAggroModifier = 1.1f;
    public float rangeAggroModifier = 1.05f;

    private Audio audio;

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

        audio = GetComponent<Audio>();
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
                switch (stats.type)
                {
                    case Stats.Type.Player:
                        if (targetRootStats.type == Stats.Type.Enemy || targetRootStats.type == Stats.Type.Boss)
                        {
                            ManagePlayerCollisions(targetRootStats, other);
                        }
                        break;
                    case Stats.Type.Ally:
                        if (targetRootStats.type == Stats.Type.Enemy || targetRootStats.type == Stats.Type.Boss)
                        {
                            ManageSimpleDemonCollisions(targetRootStats, other);
                        }
                        break;
                    case Stats.Type.Enemy:
                        if (targetRootStats.type == Stats.Type.Player || targetRootStats.type == Stats.Type.Ally)
                        {
                            ManageSimpleDemonCollisions(targetRootStats, other);
                        }
                        break;
                    case Stats.Type.Boss:
                        if (targetRootStats.type == Stats.Type.Player || targetRootStats.type == Stats.Type.Ally)
                        {
                            ManageBossCollisions(targetRootStats, other);
                        }
                        break;
                }
            }
        }
        else
        {
            Debug.Log(this.transform.root.gameObject.name + " is trying to hit a target without stats. target is: " + other.transform.root.gameObject.name);
        }
    }

    private void StopAttack()
    {
        switch (this.type)
        {
            case AttackColliderType.Melee:
                if (!isGlobalAttacking && !isSweeping)
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
            if (stats.type == Stats.Type.Ally)
            {
                if (demonBehaviour == null)
                {
                    demonBehaviour = this.transform.root.gameObject.GetComponent<DemonBehaviour>();
                }
                if (demonBehaviour != null)
                {
                    demonBehaviour.groupBelongingTo.GetComponent<GroupAggro>().RaiseGroupAggro((aggroModifier - 1f) * stats.Aggro);
                }
            }

            stats.RaiseAggro(aggroModifier);
        }
        else
        {
            Debug.Log(this.name + "AttackCollider.type is set to None");
        }
    }

    private bool CheckAngle(Transform other)
    {
        return Vector3.Angle(this.transform.root.transform.forward, other.forward) < 91;
    }

    private void ManageKnockBack(Stats targetRootStats)
    {
        // If we can deal a knockback
        if (Random.Range(1f, 101f) <= stats.KnockBackChance)
        {
            // If we are dealing a sweep attack (heavy attack)
            if (isSweeping)
            {
                // If we are hitting a non player that is not blocking
                if (!targetRootStats.IsBlocking && targetRootStats.type != Stats.Type.Player)
                {
                    targetRootStats.TakeKnockBack(stats.KnockBackSize, this.transform.root, stats.KnockBackTime);
                }
                // If target is blocking we have to understand the angle to know if we have to deal a knockback or not
                if (targetRootStats.IsBlocking)
                {
                    if (CheckAngle(targetRootStats.gameObject.transform))
                    {
                        targetRootStats.TakeKnockBack(stats.KnockBackSize, this.transform.root, stats.KnockBackTime);
                    }
                }
                // Player cannot block an heavy attack, he/she has to dodge it
                if (targetRootStats.type == Stats.Type.Player)
                {
                    targetRootStats.TakeKnockBack(stats.KnockBackSize, this.transform.root, stats.KnockBackTime);
                }
            }
            // for any other attack we knockback only if the target is not blocking
            if (!targetRootStats.IsBlocking)
            {
                targetRootStats.TakeKnockBack(stats.KnockBackSize, this.transform.root, stats.KnockBackTime);
            }
        }
        else
        {
            Debug.Log("No KnockBack, probably the target is blocking");
        }
    }
    
    private void ManageAudio(Stats targetRootStats) {
        // Global attack will have his own sound I think.
        if (!isGlobalAttacking) {
            if (audio != null)
            {
                audio.PlayRandomCombatAudioClip(AudioManager.CombatAudio.Hit);
            }
            else
            {
                Debug.Log(stats.gameObject.name + " cannot find audio in " + targetRootStats.gameObject.name);
            }
        }
    }

    private void ManageHit(Stats targetRootStats) { 
        DealDamage(targetRootStats);
        
        ManageAudio(targetRootStats);

        ManageAggro();

        StopAttack();
    }

    // TODO - Implement this
    private void ManageMiss() { }

    private void ManagePlayerCollisions(Stats targetRootStats, Collider other)
    {
        if (targetRootStats.IsBlocking)
        {
            if (CheckAngle(other.gameObject.transform.root))
            {
                ManageHit(targetRootStats);
            }
            else
            {
                ManageAggro();

                StopAttack();
            }
        }
        if (!targetRootStats.IsBlocking)
        {
            ManageHit(targetRootStats);
        }
    }

    private void ManageSimpleDemonCollisions(Stats targetRootStats, Collider other)
    {
        if (targetRootStats.IsBlocking)
        {
            if (CheckAngle(other.gameObject.transform.root))
            {
                if (targetRootStats.CalculateBeenHitChance(false))
                {
                    ManageHit(targetRootStats);
                }
                else {
                    ManageAggro();

                    StopAttack();
                }
            }
            else
            {
                if (targetRootStats.CalculateBeenHitChance(true))
                {
                    ManageHit(targetRootStats);
                }
                else {
                    ManageAggro();

                    StopAttack();
                }
            }
        }
        if (!targetRootStats.IsBlocking)
        {
            if (targetRootStats.CalculateBeenHitChance(false))
            {
                ManageHit(targetRootStats);
            }
            else {
                ManageAggro();

                StopAttack();
            }
        }
    }

    private void ManageBossCollisions(Stats targetRootStats, Collider other)
    {
        if (targetRootStats.IsBlocking)
        {
            // Player cannot block sweeping (heavy attack)
            if (isSweeping && targetRootStats.type == Stats.Type.Player)
            {
                ManageHit(targetRootStats);
                
                ManageKnockBack(targetRootStats);
            }

            // if target is blocking but is not looking towards the boss
            if (CheckAngle(other.gameObject.transform.root))
            {
                // calculate been hit chance without counting block bonus
                if (targetRootStats.CalculateBeenHitChance(false))
                {
                    ManageHit(targetRootStats);

                    ManageKnockBack(targetRootStats);
                }
                else {
                    ManageAggro();

                    // We stop the attack only if is a simple attack
                    StopAttack();
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
                }
                else {
                    ManageAggro();

                    StopAttack();
                }
            }
        }
        if (!targetRootStats.IsBlocking)
        {
            // Calculate been hit chance without counting block bonus
            if (targetRootStats.CalculateBeenHitChance(false))
            {
                ManageHit(targetRootStats);

                ManageKnockBack(targetRootStats);
            }
            else {
                ManageAggro();

                // We stop the attack only if is a simple attack
                StopAttack();

            }
        }
    }

    private void DealDamage(Stats targetRootStats)
    {
        // If the player or an Ally has to deal damage
        if (stats.type == Stats.Type.Player || stats.type == Stats.Type.Ally)
        {

            float damage = 0f;

            // if attacking demons are 0 we have a divison by 0, BEWARE
            float attackingDemons = 0;
            int supportingDemons = 0;

            // Calculate all attacking and supporting demons
            if (stats.Groups != null)
            {
                foreach (GameObject group in stats.Groups)
                {
                    GroupBehaviour gb = group.GetComponent<GroupBehaviour>();
                    if (gb != null)
                    {
                        if (gb.currentState == GroupBehaviour.State.MeleeAttack || gb.currentState == GroupBehaviour.State.RangeAttack)
                        {
                            attackingDemons += gb.GetDemonsNumber();
                        }
                        if (gb.currentState == GroupBehaviour.State.Support)
                        {
                            supportingDemons += gb.GetDemonsNumber();
                        }
                    }
                }
            }

            // Avoid division by 0
            if (attackingDemons == 0)
            {
                attackingDemons = stats.SupportDamageBuffMultiplier;
            }

            // Calculate damage
            if (type == AttackColliderType.Melee)
            {
                damage = supportingDemons * (stats.SupportDamageBuffMultiplier / attackingDemons) + stats.MeleeDamage;
            }
            if (type == AttackColliderType.Ranged)
            {
                damage = supportingDemons * (stats.SupportDamageBuffMultiplier / attackingDemons) + stats.RangedDamage;
            }
            if (type == AttackColliderType.None)
            {
                Debug.Log(this.transform.root.gameObject.name + " attack collide type not set");
            }

            // Deal damage
            targetRootStats.TakeHit(damage);
        }

        // If an Enemy or a Boss has to deal damage
        if (stats.type == Stats.Type.Enemy || stats.type == Stats.Type.Boss)
        {
            if (type == AttackColliderType.Melee)
            {
                targetRootStats.TakeHit(stats.MeleeDamage);
            }
            if (type == AttackColliderType.Ranged)
            {
                targetRootStats.TakeHit(stats.RangedDamage);
            }
            if (type == AttackColliderType.None)
            {
                Debug.Log(this.transform.root.gameObject.name + " attack collide type not set");
            }
        }
    }

    #endregion
}
