using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAutoAttackTest : MonoBehaviour
{
    private Combat combat;
    private Coroutine attackTestCR;
    public float attackRateo = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        combat = GetComponent<Combat>();

        attackTestCR = StartCoroutine(AttackTest());
    }

    private IEnumerator AttackTest() {
        yield return new WaitForSeconds(2.5f);
        while (true) { 
            combat.PlayerAttack();
            yield return new WaitForSeconds(attackRateo);
        }    
    }
}
