using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stats))]
public class Combat : MonoBehaviour
{
    public CombatManager combatManager;

    private Stats stats;

    private void Start()
    {
        stats = GetComponent<Stats>();
    }

    void Update()
    {
        // left click
        // -TODO- add controller button
        // Used for testing - Put attack button in player controller
        if ( stats.type == Stats.Type.Player )
        {
            if ( Input.GetMouseButtonDown( 0 ) )
            {
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

    public void StartBlock()
    {
        combatManager.StartBlock();
    }

    public void StopBlock()
    {
        combatManager.StopBlock();
    }

    public void StopAttack()
    {
        combatManager.StopAttack();
    }
}
