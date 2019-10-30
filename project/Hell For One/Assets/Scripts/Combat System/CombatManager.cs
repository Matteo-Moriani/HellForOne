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

    private Stats stats;

    public bool isIdle { get; set; } = true;
    private bool isBlocking = false;
    private Coroutine attackCR;

    private Vector3 startPosition;

    void Start()
    {
        // -TODO- add if null coditions.
        // and if true init GO.

        stats = this.transform.root.gameObject.GetComponent<Stats>();   

        startPosition = attackCollider.transform.localPosition;

        attackCollider.SetActive(false);
        blockCollider.SetActive(false);
        idleCollider.SetActive(true);
    }

    void Update()
    {

    }

    public void StartBlock()
    {
        if ( isIdle )
        {
            isIdle = false;
            isBlocking = true;

            blockCollider.SetActive( true );
        }
    }

    public void StopBlock()
    {
        if ( isBlocking )
        {
            blockCollider.SetActive( false );

            isBlocking = false;
            isIdle = true;
        }
    }

    public void Attack()
    {
        if ( isIdle )
            attackCR = StartCoroutine( AttackCoroutine() );
    }

    public void StopAttack() {
        if (attackCR != null)
        {
            StopCoroutine(attackCR);
            attackCR = null;

            attackCollider.transform.localPosition = startPosition;
            attackCollider.SetActive( false );
            
            isIdle = true;
        }
        return;
    }
    
    public void RangedAttack(GameObject target) { 
        Debug.Log("RangedAttack");   
    }

    public void StopRangedAttack() { 
        Debug.Log("Stop RangedAttack");
    }

    private IEnumerator AttackCoroutine()
    {
        isIdle = false;

        attackCollider.SetActive(true);
        
        Vector3 targetPosition = attackCollider.transform.localPosition + new Vector3(0.0f,0.0f, stats.AttackRange);

        float timeAcc = 0f;
        // -TODO-   See if can remove tollerance
        //          Accelerate lerp
        //          Give sense to range
        //          Test attack and block in same time
        while ( Vector3.Distance( attackCollider.transform.localPosition, targetPosition ) > 0.1 )
        {
            attackCollider.transform.localPosition = Vector3.Lerp( startPosition, targetPosition, timeAcc * stats.AttackDurationMultiplier );
            timeAcc += Time.deltaTime;
            yield return null;
        }

        attackCollider.transform.localPosition = startPosition;
        attackCollider.SetActive(false);

        isIdle = true;
    }
}
