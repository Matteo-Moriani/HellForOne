using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Stats ) )]
[RequireComponent(typeof(CombatAudio))]
public class Combat : MonoBehaviour
{
    #region fields

    public CombatManager combatManager;

    // We store a whole reference to Stats.
    // Used only to check the type of this Imp.
    // Type can change during time, so it's better to
    // have a reference to Stats.
    private Stats stats;

    // This could be a solution
    // TODO -   Check if this can be removed
    //          now has zero references
    public GameObject target;

    [SerializeField]
    private float playerAttackCooldown = 0.5f;

    private float coolDownCounter = 0.0f;

    #endregion

    #region methods

    private void Start()
    {
        stats = GetComponent<Stats>();

        coolDownCounter = playerAttackCooldown;
    }

    void Update()
    {
        if ( coolDownCounter <= playerAttackCooldown )
        {
            coolDownCounter += Time.deltaTime;
        }

        // left click
        // -TODO- add controller button
        // Used for testing - Put attack button in player controller
        if ( stats.type == Stats.Type.Player )
        {
            if ( Input.GetMouseButtonDown( 0 ) && coolDownCounter >= playerAttackCooldown )
            {
                coolDownCounter = 0.0f;

                Attack();
            }

            if ( Input.GetMouseButtonDown( 1 ) )
            {
                StartBlock();
            }
            if ( Input.GetMouseButtonUp( 1 ) )
            {
                StopBlock();
            }
        }
    }

    /// <summary>
    /// Deals an attack.
    /// Calls CombatManager.Attack
    /// </summary>
    public void Attack()
    {
        combatManager.MeleeAttack();
    }

    /// <summary>
    /// Deals an attack to a target.
    /// Calls CombatManager.Attack.
    /// </summary>
    /// <param name="target">The target of the attack</param>
    public void Attack( GameObject target )
    {
        combatManager.MeleeAttack( target );
    }

    /// <summary>
    /// Stop an ongoing attack.
    /// Calls CombatManager.StopAttack
    /// </summary>
    public void StopAttack()
    {
        combatManager.StopMeleeAttack();
    }

    /// <summary>
    /// Deals A ranged attack to a target.
    /// </summary>
    /// <param name="target">The target of the ranged attack</param>
    public void RangedAttack( GameObject target )
    {
        combatManager.RangedAttack( target );
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
    }

    /// <summary>
    /// This units starts blocking.
    /// Calls CombatManager.StartBlock.
    /// </summary>
    public void StartBlock()
    {
        combatManager.StartBlock();
    }

    /// <summary>
    /// This unit stops blocking
    /// Calls CombatManager.StopBlock.
    /// </summary>
    public void StopBlock()
    {
        combatManager.StopBlock();
    }

    /// <summary>
    /// Deals a Sweep Attack.
    /// (Heavy attack or Area attack)
    /// Calls CombatManager.Sweep.
    /// </summary>
    public void Sweep()
    {
        combatManager.Sweep();
    }

    /// <summary>
    /// Stops an Ongoing Sweep attack.
    /// (Heavy attack or Area attack)
    /// Calls CombatManager.StopSweep.
    /// </summary>
    public void StopSweep()
    {
        combatManager.StopSweep();
    }

    /// <summary>
    /// Deals a GlobalAttack
    /// Calls CombatManager.GlobalAttack.
    /// </summary>
    public void GlobalAttack()
    {
        combatManager.GlobalAttack();
    }

    /// <summary>
    /// Stops an ongoing Global attack.
    /// Calss CombatManager.StopGlobalAttack.
    /// </summary>
    public void StopGlobalAttack()
    {
        combatManager.StopGlobalAttack();
    }

    /// <summary>
    /// This unit start supporting.
    /// Calss CombatManager.StartSupport.
    /// </summary>
    public void StartSupport()
    {
        combatManager.StartSupport();
    }

    /// <summary>
    /// This unit stop supporting.
    /// Calls CombatManager.StopSupport 
    /// </summary>
    public void StopSupport()
    {
        combatManager.StopSupport();
    }

    /// <summary>
    /// Stops all ongoing combat actions
    /// Calls CombatManager.StopAll
    /// </summary>
    public void StopAll()
    {
        combatManager.StopAll();
    }

    #endregion
}
