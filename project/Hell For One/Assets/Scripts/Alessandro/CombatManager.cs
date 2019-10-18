using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField]
    private GameObject attackCollider;
    [SerializeField]
    private GameObject blockCollider;
    [SerializeField]
    private GameObject idleCollider;

    [SerializeField]
    Stats stats;

    private bool isIdle = true;
    private bool isBlocking = false;
    
    void Start()
    {
        // -TODO- add if null coditions.
        // and if true init GO.

        attackCollider.SetActive(false);
        blockCollider.SetActive(false);
        idleCollider.SetActive(true);
    }

    void Update()
    {
        
    }

    public void StartBlock() {
        if (isIdle) { 
            isIdle = false;
            isBlocking = true;

            blockCollider.SetActive(true);
        }
    }
    
    public void StopBlock() {
        if (isBlocking) { 
            blockCollider.SetActive(false);
            
            isBlocking = false;
            isIdle = true;
        }
    }

    public void Attack() {
        if(isIdle)
            StartCoroutine(AttackCoroutine());    
    }

    public void StopAttack() { 
        StopAllCoroutines();    
    }

    // -TODO- Lerp collider.
    private IEnumerator AttackCoroutine() { 
        isIdle = false;

        attackCollider.SetActive(true);
        
        Vector3 startPosition = attackCollider.transform.localPosition;
        Vector3 targetPosition = attackCollider.transform.localPosition + new Vector3(0.0f,0.0f, stats.AttackRange);

        float timeAcc = 0f;
        // -TODO-   See if can remove tollerance
        //          Accelerate lerp
        //          Give sense to range
        //          Test attack and block in same time
        while(Vector3.Distance(attackCollider.transform.localPosition,targetPosition) > 0.1) { 
            attackCollider.transform.localPosition = Vector3.Lerp(startPosition,targetPosition, timeAcc * stats.AttackDurationMultiplier);
            timeAcc += Time.deltaTime;
            yield return null;
        }

        //yield return new WaitForSeconds(0.1f);
        attackCollider.transform.localPosition = startPosition;
        attackCollider.SetActive(false);
        yield return null;

        isIdle = true;
    }
}
