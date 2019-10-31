using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackAutoTest : MonoBehaviour
{
    public Combat combat;
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(rangedCR());    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator rangedCR() {
        yield return new WaitForSeconds(2.0f);
        combat.RangedAttack(target);
    }
}
