using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    #region fields

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

    [SerializeField]
    private float attackDelayInSeconds = 0f;

    [SerializeField]
    private float attackDurationInSeconds = 0.5f;

    private Coroutine attackCR;
    private Coroutine globalAttackCR;

    private Vector3 startPosition;
    private Vector3 baseAttackColliderScale;

    #endregion

    #region methods

    void Start()
    {
        // -TODO- add if null coditions.
        // and if true init GO.
        if ( stats == null )
            stats = this.transform.root.gameObject.GetComponent<Stats>();

        if ( lancer == null )
        {
            lancer = this.transform.root.gameObject.GetComponent<Lancer>();
        }

        startPosition = attackCollider.transform.localPosition;
        baseAttackColliderScale = attackCollider.transform.localScale;

        attackCollider.SetActive( false );
        blockCollider.SetActive( false );
        idleCollider.SetActive( true );
    }

    public void StartSupport()
    {
        if ( stats.IsIdle && !stats.IsSupporting )
        {
            stats.IsIdle = false;
            stats.IsSupporting = true;

        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.StartSupport is trying to start supporting but is not idle or is already supporting" );
        }
    }

    public void StopSupport()
    {
        if ( !stats.IsIdle && stats.IsSupporting )
        {
            stats.IsSupporting = false;
            stats.IsIdle = true;
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManader.StopSupport is trying to stop supporting but is idle or is not supporting" );
        }
    }

    public void StartBlock()
    {
        if ( stats.IsIdle && !stats.IsBlocking )
        {
            stats.IsIdle = false;
            stats.IsBlocking = true;

            //TODO-Remove this
            blockCollider.SetActive( true );
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.StartBlock is trying to start blocking but is not idle or is already blocking" );
        }

    }

    public void StopBlock()
    {
        if ( !stats.IsIdle && stats.IsBlocking )
        {
            //TODO-Remove this
            blockCollider.SetActive( false );

            stats.IsBlocking = false;
            stats.IsIdle = true;
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManader.StopBlock is trying to stop blocking but is idle or is not blocking" );
        }
    }

    public void Attack()
    {
        if ( stats.IsIdle )
        {
            attackCR = StartCoroutine( AttackCoroutine() );
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.Attack is trying to attack but is not idle." );
        }

    }

    public void StopAttack()
    {
        if ( attackCR != null && !stats.IsIdle )
        {
            StopCoroutine( attackCR );
            attackCR = null;

            attackCollider.transform.localPosition = startPosition;
            attackCollider.SetActive( false );

            stats.IsIdle = true;
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.StopAttack is trying to stop an attack but is not idle or attackCR is null" );
        }
        return;
    }

    public void RangedAttack( GameObject target )
    {
        if ( stats.IsIdle )
        {
            stats.IsIdle = false;
            if ( target != null )
                lancer.Start( target );
            else
                Debug.Log( this.name + "Is trying a ranged attack to a null target" );
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.RangedAttack is trying a ranged attack but is not idle" );
        }
    }

    public void StopRangedAttack()
    {
        if ( !stats.IsIdle )
        {
            lancer.Stop();
            stats.IsIdle = true;
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.StopRangedAttack is trying to stop a ranged attack but it is idle" );
        }
    }

    public void Sweep()
    {
        if ( stats.IsIdle )
        {
            attackCollider.transform.localScale = new Vector3( stats.SweepSize, attackCollider.transform.localScale.y, attackCollider.transform.localScale.z );
            attackCollider.GetComponent<AttackCollider>().isSweeping = true;
            attackCR = StartCoroutine( AttackCoroutine() );
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.Sweep is trying a sweep attack but it is not idle" );
        }
    }

    public void StopSweep()
    {
        if ( attackCR != null && !stats.IsIdle )
        {
            StopCoroutine( attackCR );
            attackCR = null;

            attackCollider.transform.localPosition = startPosition;
            attackCollider.transform.localScale = baseAttackColliderScale;
            attackCollider.GetComponent<AttackCollider>().isSweeping = false;
            attackCollider.SetActive( false );

            stats.IsIdle = true;
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.StopSweep is trying to stop a sweep attack but it is idle" );
        }
        return;
    }

    public void GlobalAttack()
    {
        if ( stats.IsIdle )
        {
            globalAttackCR = StartCoroutine( GlobalAttackCoroutine() );
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.GlobalAttack is trying a global attack but it is not idle" );
        }
    }

    public void StopGlobalAttack()
    {
        if ( globalAttackCR != null && !stats.IsIdle )
        {
            StopCoroutine( globalAttackCR );
            globalAttackCR = null;

            attackCollider.transform.localScale = baseAttackColliderScale;
            attackCollider.GetComponent<AttackCollider>().isGlobalAttacking = false;
            attackCollider.SetActive( false );

            stats.IsIdle = true;
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.StopGlobalAttack is trying to stop a global attack but it is idle or globalAttackCr is null" );
        }
    }

    private IEnumerator AttackCoroutine()
    {
        stats.IsIdle = false;

        Vector3 targetPosition = attackCollider.transform.localPosition + new Vector3( 0.0f, 0.0f, stats.AttackRange );

        yield return new WaitForSeconds( attackDelayInSeconds );

        attackCollider.transform.localPosition = targetPosition;

        attackCollider.SetActive( true );

        yield return new WaitForSeconds( attackDurationInSeconds );

        // Testing new attack logic
        /*
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
        */

        attackCollider.transform.localPosition = startPosition;

        if ( attackCollider.GetComponent<AttackCollider>().isSweeping )
        {
            attackCollider.transform.localScale = baseAttackColliderScale;
            attackCollider.GetComponent<AttackCollider>().isSweeping = false;
        }

        attackCollider.SetActive( false );

        stats.IsIdle = true;
    }

    private IEnumerator GlobalAttackCoroutine()
    {
        stats.IsIdle = false;

        attackCollider.GetComponent<AttackCollider>().isGlobalAttacking = true;
        attackCollider.transform.localScale = new Vector3( stats.GlobalAttackSize, attackCollider.transform.localScale.y, stats.GlobalAttackSize );

        attackCollider.SetActive( true );

        yield return new WaitForSeconds( stats.GlobalAttackDuration );

        attackCollider.transform.localScale = baseAttackColliderScale;
        attackCollider.GetComponent<AttackCollider>().isGlobalAttacking = false;
        attackCollider.SetActive( false );

        stats.IsIdle = true;
    }

    #endregion
}
