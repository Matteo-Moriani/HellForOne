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
    private Lancer lancer;

    [SerializeField]
    private Stats stats;

    //public bool isIdle { get; set; } = true;
    //private bool isBlocking = false;
    private Coroutine attackCR;

    private Vector3 startPosition;
    private Vector3 baseAttackColliderScale;

    void Start()
    {
        // -TODO- add if null coditions.
        // and if true init GO.
        if(stats == null)
            stats = this.transform.root.gameObject.GetComponent<Stats>();   

        if(lancer == null) { 
            lancer = this.transform.root.gameObject.GetComponent<Lancer>();    
        }

        startPosition = attackCollider.transform.localPosition;
        baseAttackColliderScale = attackCollider.transform.localScale;

        attackCollider.SetActive(false);
        blockCollider.SetActive(false);
        idleCollider.SetActive(true);
    }

    void Update()
    {

    }

    public void StartBlock()
    {
        if ( stats.IsIdle )
        {
            stats.IsIdle = false;
            stats.IsBlocking = true;

            //TODO-Remove this
            blockCollider.SetActive( true );
        }
    }

    public void StopBlock()
    {
        if ( stats.IsBlocking )
        {   
            //TODO-Remove this
            blockCollider.SetActive( false );

            stats.IsBlocking = false;
            stats.IsIdle = true;
        }
    }

    //-TODO- bool isAttacking
    public void Attack()
    {
        if ( stats.IsIdle )
            attackCR = StartCoroutine( AttackCoroutine() );
    }

    public void StopAttack() {
        if (attackCR != null)
        {
            StopCoroutine(attackCR);
            attackCR = null;

            attackCollider.transform.localPosition = startPosition;
            attackCollider.SetActive( false );
            
            stats.IsIdle = true;
        }
        return;
    }
    
    //-TODO-    Decide if a ranged attack will be continuos (using Start)
    //          or single (using Launch)
    public void RangedAttack(GameObject target) {
        if (stats.IsIdle) {
            stats.IsIdle = false;
            if (target != null)
                //lancer.Launch(target);
                lancer.Start(target);
            else
                Debug.Log(this.name + "Is trying a ranged attack to a null target");
        }
    }

    //-TODO--   If we use continuos attack we need a StopRangedAttack method
    public void StopRangedAttack() { 
        //Debug.Log("Stop RangedAttack");
        lancer.Stop();
        stats.IsIdle = true;
    }

    public void Sweep() {
        if (stats.IsIdle)
        {
            attackCollider.transform.localScale = new Vector3(stats.SweepSize, attackCollider.transform.localScale.y, attackCollider.transform.localScale.z);
            attackCollider.GetComponent<AttackCollider>().isSweeping = true;
            attackCR = StartCoroutine(AttackCoroutine());
        }
    }

    public void StopSweep() {
        if (attackCR != null)
        {
            StopCoroutine(attackCR);
            attackCR = null;

            attackCollider.transform.localPosition = startPosition;
            attackCollider.transform.localScale = baseAttackColliderScale;
            attackCollider.GetComponent<AttackCollider>().isSweeping = false;
            attackCollider.SetActive(false);

            stats.IsIdle = true;
        }
        return;
    }

    public void GlobalAttack() {
        StartCoroutine(GlobalAttackCoroutine());
    }

    // TODO
    public void StopGlobalAttack() { }

    private IEnumerator AttackCoroutine()
    {
        stats.IsIdle = false;

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

        if (attackCollider.GetComponent<AttackCollider>().isSweeping) {
            attackCollider.transform.localScale = baseAttackColliderScale;
            attackCollider.GetComponent<AttackCollider>().isSweeping = false;
        }

        attackCollider.SetActive(false);

        stats.IsIdle = true;
    }

    private IEnumerator GlobalAttackCoroutine() {
        attackCollider.GetComponent<AttackCollider>().isGlobalAttacking = true;
        attackCollider.transform.localScale = new Vector3(stats.GlobalAttackSize, attackCollider.transform.localScale.y, stats.GlobalAttackSize);

        attackCollider.SetActive(true);

        yield return new WaitForSeconds(stats.GlobalAttackDuration);

        attackCollider.transform.localScale = baseAttackColliderScale;
        attackCollider.GetComponent<AttackCollider>().isGlobalAttacking = false;
        attackCollider.SetActive(false);
        
    }
}
