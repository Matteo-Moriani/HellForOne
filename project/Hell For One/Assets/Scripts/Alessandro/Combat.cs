using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [SerializeField]
    private GameObject combatManagerParent;

    public CombatManager combatManager;

    // Used for testing - Put attack button in player controller
    private string parentTag;

    private void Start()
    {
        combatManager = combatManagerParent.GetComponent<CombatManager>();

        // Used for testing - Put attack button in player controller
        parentTag = this.gameObject.tag;
    }

    void Update()
    {
        // left click
        // -TODO- add controller button
        // Used for testing - Put attack button in player controller
        if ( parentTag == "Player" )
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
        combatManager.StopAllCoroutines();
    }
}
