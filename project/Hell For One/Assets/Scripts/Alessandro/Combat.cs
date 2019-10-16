using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [SerializeField]
    private GameObject weaponCollider;

    [SerializeField]
    private float colliderDuration=0.1f;

    private PlayerCombatCollisions playerCombatCollisions;

    private bool isBlockCRoutineRunning = false;

    private void Start()
    {
        playerCombatCollisions = weaponCollider.GetComponent<PlayerCombatCollisions>();    
    }

    void Update()
    {
        // left click
        // -TODO- add controller button
        if (Input.GetMouseButtonDown(0)) { 
            Attack();
        }
        if (Input.GetMouseButton(1)) { 
            if(!isBlockCRoutineRunning)
                Block();       
        }
    }

    void Attack() { 
        StartCoroutine(AttackCoroutine());    
    }

    void Block() { 
        StartCoroutine(BlockCoroutine());    
    }

    IEnumerator AttackCoroutine() { 
        playerCombatCollisions.SetMode(PlayerCombatCollisions.Mode.Attack);
        weaponCollider.SetActive(true);
        yield return new WaitForSeconds(colliderDuration);
        weaponCollider.SetActive(false);
        playerCombatCollisions.SetMode(PlayerCombatCollisions.Mode.Idle);
        yield return null;
    }

    IEnumerator BlockCoroutine() { 
        isBlockCRoutineRunning = true;
        weaponCollider.SetActive(true);
        playerCombatCollisions.SetMode(PlayerCombatCollisions.Mode.Block);

        while (Input.GetMouseButton(1)) { 
            yield return null;
        }

        weaponCollider.SetActive(false);
        playerCombatCollisions.SetMode(PlayerCombatCollisions.Mode.Idle);
        isBlockCRoutineRunning = false;

        yield return null;
    }
}
