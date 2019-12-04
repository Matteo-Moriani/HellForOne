using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Combat))]
public class LittleEnemyAutoAttackTest : MonoBehaviour
{
    private Coroutine autoAttackCR;
    
    [SerializeField]
    private Combat combat;
    
    [SerializeField]
    [Tooltip("Seconds to wait before the next attack")]
    private float attackRateo = 1.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        if(combat == null) { 
            combat = GetComponent<Combat>();    
        }

        if(autoAttackCR == null) { 
            autoAttackCR = StartCoroutine(autoAttackCoroutine());    
        }
    }

    private IEnumerator autoAttackCoroutine() {
        while (true) { 
            yield return new WaitForSeconds(attackRateo);

            combat.PlayerAttack();
        }    
    }
}
