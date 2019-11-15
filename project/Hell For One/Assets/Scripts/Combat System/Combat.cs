using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Stats ) )]
public class Combat : MonoBehaviour
{
    public CombatManager combatManager;

    private Stats stats;

    [SerializeField]
    private float playerAttackCooldown = 0.5f;

    private float coolDownCounter = 0.0f;

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

    public void Attack()
    {
        combatManager.Attack();
    }

    public void StopAttack()
    {
        combatManager.StopAttack();
    }

    public void RangedAttack( GameObject target )
    {
        combatManager.RangedAttack( target );
    }

    public void StopRangedAttack()
    {
        combatManager.StopRangedAttack();
    }

    public void StartBlock()
    {
        combatManager.StartBlock();
    }

    public void StopBlock()
    {
        combatManager.StopBlock();
    }

    public void Sweep()
    {
        combatManager.Sweep();
    }

    public void StopSweep()
    {
        combatManager.StopSweep();
    }

    public void GlobalAttack()
    {
        combatManager.GlobalAttack();
    }

    public void StopGlobalAttack()
    {
        combatManager.StopGlobalAttack();
    }

    public void StartSupport()
    {
        combatManager.StartSupport();
    }

    public void StopSupport()
    {
        combatManager.StopSupport();
    }
}
