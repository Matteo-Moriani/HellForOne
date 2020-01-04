using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Stats ) )]
public class Combat : MonoBehaviour
{
    #region fields

    public CombatManager combatManager;

    private CombatEventsManager combatEventsManager;

    // We store a whole reference to Stats.
    // Used only to check the type of this Imp.
    // Type can change during time, so it's better to
    // have a reference to Stats.
    private Stats stats;

    // This could be a solution
    // TODO -   Check if this can be removed
    //          now has zero references
    public GameObject target;

    private GameObject rangeTarget;
    
    public float playerAttackCooldown = 0.65f;

    private float coolDownCounter = 0.0f;

    #endregion

    #region methods

    private void Start()
    {
        stats = GetComponent<Stats>();

        coolDownCounter = playerAttackCooldown;

        // Need this because boss has many children in his transform
        if(stats.type == Stats.Type.Player || stats.type == Stats.Type.Ally) {
            foreach (Transform child in transform)
            {
                if (child.tag == "PlayerRangedTarget")
                    rangeTarget = child.gameObject;
            }
        }
        
        combatEventsManager = GetComponent<CombatEventsManager>();
    }

    void Update()
    {
        if ( coolDownCounter <= playerAttackCooldown )
        {
            coolDownCounter += Time.deltaTime;
        }
    }

    /// <summary>
    /// Deals an attack.
    /// Calls CombatManager.Attack
    /// Used for the player
    /// </summary>
    public void PlayerAttack()
    {   if(coolDownCounter >= playerAttackCooldown) { 
            coolDownCounter = 0f;
            
            // Do attack
            combatManager.PlayerAttack();
            
            // Melee attack event
            if(combatEventsManager != null) { 
                combatEventsManager.RaiseOnStartSingleAttack();    
            }
        }
    }

    /// <summary>
    /// Deals an attack to a target.
    /// Calls CombatManager.Attack.
    /// Used for generic demons
    /// </summary>
    /// <param name="target">The target of the attack</param>
    public void SingleAttack( GameObject target )
    {
        combatManager.SingleAttack( target );

        // Melee attack event
        if (combatEventsManager != null)
        {
            combatEventsManager.RaiseOnStartSingleAttack();
        }
    }

    /// <summary>
    /// Stop an ongoing attack.
    /// Calls CombatManager.StopAttack
    /// </summary>
    public void StopAttack()
    {
        combatManager.StopSingleAttack();

        // Stop melee attack event
        if (combatEventsManager != null)
        {
            combatEventsManager.RaiseOnStopSingleAttack();
        }
    }

    /// <summary>
    /// Deals A ranged attack to a target.
    /// </summary>
    /// <param name="target">The target of the ranged attack</param>
    public void RangedAttack( GameObject target )
    {
        // If the palyer wants to range attack...
        if(stats.type == Stats.Type.Player) { 
            if(coolDownCounter >= playerAttackCooldown) {
                coolDownCounter = 0f;
                if(rangeTarget != null) {
                    combatManager.RangedAttack(rangeTarget);

                    // RangedAttack attack event
                    if (combatEventsManager != null)
                    {
                        combatEventsManager.RaiseOnStartRangedAttack();
                    }
                }
                else {
                    Debug.Log("Player cannot find rangeTarget");
                }
            }
        }
        // For everty other demon...
        else {
            combatManager.RangedAttack(target);

            // RangedAttack attack event
            if (combatEventsManager != null)
            {
                combatEventsManager.RaiseOnStartRangedAttack();
            }
        }
    }

    /// <summary>
    /// TODO -  Check if this is needed.
    ///         Ranged attack are continuos if we call
    ///         Lancer.Start (in this case StopRangedAttack is needed)
    ///         or can be single if we call Lancer.Launch
    ///         (in this case StopRangedAttack is not needed).
    /// </summary>  
    public void StopRangedAttack()
    {
        combatManager.StopRangedAttack();

        // Stop rangedAttack attack event
        if (combatEventsManager != null)
        {
            combatEventsManager.RaiseOnStopRangedAttack();
        }
    }

    /// <summary>
    /// This units starts blocking.
    /// Calls CombatManager.StartBlock.
    /// </summary>
    public void StartBlock()
    {
        combatManager.StartBlock();

        // Start block event
        if (combatEventsManager != null)
        {
            combatEventsManager.RaiseOnStartBlock();
        }
    }

    /// <summary>
    /// This unit stops blocking
    /// Calls CombatManager.StopBlock.
    /// </summary>
    public void StopBlock()
    {
        combatManager.StopBlock();

        // StopBlock event
        if (combatEventsManager != null)
        {
            combatEventsManager.RaiseOnStopBlock();
        }
    }

    /// <summary>
    /// Deals a Sweep Attack.
    /// (Heavy attack or Area attack)
    /// Calls CombatManager.Sweep.
    /// </summary>
    public void GroupAttack()
    {
        combatManager.GroupAttack();

        // Sweep attack event
        if (combatEventsManager != null)
        {
            combatEventsManager.RaiseOnStartGroupAttack();
        }
    }

    /// <summary>
    /// Stops an Ongoing Sweep attack.
    /// (Heavy attack or Area attack)
    /// Calls CombatManager.StopSweep.
    /// </summary>
    public void StopGroupAttack()
    {
        combatManager.StopGroupAttack();

        // Stop sweep event
        if (combatEventsManager != null)
        {
            //combatEventsManager.RaiseOnStopAnimation();
            combatEventsManager.RaiseOnStopGroupAttack();
        }
    }

    /// <summary>
    /// Deals a GlobalAttack
    /// Calls CombatManager.GlobalAttack.
    /// </summary>
    public void GlobalAttack()
    {
        combatManager.GlobalAttack();

        // Global attack event
        if (combatEventsManager != null)
        {
            combatEventsManager.RaiseOnStartGlobalAttack();
        }
    }

    /// <summary>
    /// Stops an ongoing Global attack.
    /// Calss CombatManager.StopGlobalAttack.
    /// </summary>
    public void StopGlobalAttack()
    {
        combatManager.StopGlobalAttack();
        
        // Stop global attack event
        if (combatEventsManager != null)
        {
            //combatEventsManager.RaiseOnStopAnimation();
            combatEventsManager.RaiseOnStopGlobalAttack();
        }
    }

    /// <summary>
    /// This unit start supporting.
    /// Calss CombatManager.StartSupport.
    /// </summary>
    public void StartSupport()
    {
        combatManager.StartSupport();

        // Start support attack event
        if (combatEventsManager != null)
        {
            combatEventsManager.RaiseOnStartSupport();
        }
    }

    /// <summary>
    /// This unit stop supporting.
    /// Calls CombatManager.StopSupport 
    /// </summary>
    public void StopSupport()
    {
        combatManager.StopSupport();

        // Stop support event
        if (combatEventsManager != null)
        {
            combatEventsManager.RaiseOnStopSupport();
        }
    }

    /// <summary>
    /// Stops all ongoing combat actions
    /// Calls CombatManager.ResetCombat
    /// </summary>
    public void ResetCombat()
    {
        combatManager.ResetCombat();
    }

    #endregion
}
