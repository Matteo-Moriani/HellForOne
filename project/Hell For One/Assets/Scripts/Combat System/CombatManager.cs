using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    #region fields

    [SerializeField]
    [Tooltip( "The AttackCollider of this unit used for combat" )]
    private GameObject attackCollider;

    [SerializeField]
    [Tooltip( "The IdleCollider of this unit used for combat" )]
    private GameObject idleCollider;

    [SerializeField]
    [Tooltip( "Reference to Lancer component, used for ranged attacks" )]
    private Lancer lancer;

    [SerializeField]
    private Stats stats;

    [SerializeField]
    private float singleAttackDelayInSeconds = 0f;

    [SerializeField]
    private float groupAttackDelayInSeconds = 0f;

    [SerializeField]
    private float globalAttackDelayInSeconds = 0f;

    [SerializeField]
    private float attackDurationInSeconds = 0.5f;

    [SerializeField]
    private float rangedAttackDelayInSeconds = 0f;

    private Coroutine attackCR;
    private Coroutine globalAttackCR;
    private Coroutine rangedAttackCR;

    private Vector3 startPosition;
    private Vector3 baseAttackColliderScale;

    // Min distance for tactics
    [Tooltip( "The maximum distance at where they can melee attack" )]
    private float maxMeleeDistance = 1.5f;
    [Tooltip( "The maximum distance at where they can launch" )]
    private float maxRangeCombatDistance = 15f;
    [Tooltip( "The minimum distance at where they can launch" )]
    private float minRangeCombatDistance = 10f;

    // To check if minDistance is verified
    private bool canAttack = false;

    public float MaxMeleeDistance { get => maxMeleeDistance; set => maxMeleeDistance = value; }
    public float MaxRangeCombatDistance { get => maxRangeCombatDistance; set => maxRangeCombatDistance = value; }
    public float MinRangeCombatDistance { get => minRangeCombatDistance; set => minRangeCombatDistance = value; }

    private Coroutine waitTillMinDistanceCR;

    private CombatEventsManager combatEventsManager;

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
        idleCollider.SetActive( true );

        combatEventsManager = this.transform.root.gameObject.GetComponent<CombatEventsManager>();
    }

    public void ResetCombat()
    {
        if ( !stats.CombatIdle )
        {
            // Stop supporting
            if ( stats.IsSupporting )
            {
                stats.IsSupporting = false;
            }

            // Stop Blocking
            if ( stats.IsBlocking )
            {
                stats.IsBlocking = false;
            }

            // Stop Attacking
            if ( attackCR != null )
            {
                StopCoroutine( attackCR );
                attackCR = null;

                attackCollider.transform.localPosition = startPosition;

                // Stop Sweep
                if ( attackCollider.GetComponent<AttackCollider>().isGroupAttacking )
                {
                    attackCollider.transform.localScale = baseAttackColliderScale;
                    attackCollider.GetComponent<AttackCollider>().isGroupAttacking = false;
                }
            }

            // If we use Launch in RangedAttack we don't need to stop rangedAttacks
            // Maybe we need to reset Lancer's target to null?
            // Stop Ranged Attack
            // lancer.Stop();

            // Stop Global attack
            if ( globalAttackCR != null )
            {
                StopCoroutine( globalAttackCR );
                globalAttackCR = null;

                attackCollider.transform.localScale = baseAttackColliderScale;
                attackCollider.GetComponent<AttackCollider>().isGlobalAttacking = false;
            }

            // If AttackCollider is active we deactivate it
            if ( attackCollider.activeInHierarchy )
            {
                attackCollider.SetActive( false );
            }

            // Now the demon is idle
            stats.CombatIdle = true;
        }
    }

    public void StartSupport()
    {
        if ( stats.CombatIdle && !stats.IsSupporting )
        {
            stats.CombatIdle = false;
            stats.IsSupporting = true;
            GroupSupport gs = this.transform.root.gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupSupport>();

            if ( gs != null )
            {
                gs.AddSupportingUnit();
                gs.StartUpdateSupportAggro();
            }
            else
            {
                Debug.Log( this.transform.root.name + " cannot find GroupSupport" );
            }

        }
        else
        {
            //Debug.Log( this.transform.root.gameObject.name + " CombatManager.StartSupport is trying to start supporting but is not idle or is already supporting" );
        }
    }

    public void StopSupport()
    {
        if ( !stats.CombatIdle && stats.IsSupporting )
        {
            stats.IsSupporting = false;
            stats.CombatIdle = true;

            GroupSupport gs = this.transform.root.gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupSupport>();

            if ( gs != null )
            {
                gs.RemoveSupportingUnit();
                gs.StopUpdateSupportAggro();
            }
            else
            {
                Debug.Log( this.transform.root.name + " cannot find GroupSupport" );
            }
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManader.StopSupport is trying to stop supporting but is idle or is not supporting" );
        }
    }

    public void StartBlock()
    {
        if ( stats.CombatIdle && !stats.IsBlocking )
        {
            stats.CombatIdle = false;
            stats.IsBlocking = true;
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.StartBlock is trying to start blocking but is not idle or is already blocking" );
        }

    }

    public void StopBlock()
    {
        if ( !stats.CombatIdle && stats.IsBlocking )
        {
            stats.IsBlocking = false;
            stats.CombatIdle = true;
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManader.StopBlock is trying to stop blocking but is idle or is not blocking" );
        }
    }

    public void PlayerAttack()
    {
        if ( stats.CombatIdle )
        {
            attackCR = StartCoroutine( AttackCoroutine() );
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.Attack is trying to attack but is not idle." );
        }

    }

    public void SingleAttack( GameObject target )
    {   /*
        // TODO - this check now is in GroupBehaviour
        //if(target != null) {
         //   if (transform.root.gameObject.GetComponent<DemonMovement>().HorizDistFromTargetBorders(target) > maxMeleeDistance) { 
                StopAttack();
                return;
            }
        }
        
        if(target == null) { 
            StopAttack();
            return;
        }
        // ( !canAttack )
        //    return;
        */
        if ( stats.CombatIdle )
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
        if ( attackCR != null && !stats.CombatIdle )
        {
            StopCoroutine( attackCR );
            attackCR = null;

            attackCollider.transform.localPosition = startPosition;
            attackCollider.SetActive( false );

            stats.CombatIdle = true;
        }

        if(combatEventsManager != null) { 
            combatEventsManager.RaiseOnStopAttack();    
        }
        //else
        //{
        //    Debug.Log( this.transform.root.gameObject.name + " CombatManager.StopAttack is trying to stop an attack but is not idle or attackCR is null" );
        //}
        return;
    }

    // TODO same as MeleeAttack(), they don't have to attack if not in distance
    public void RangedAttack( GameObject target )
    {   
        /*
        // Regular Imps need to check if target is in range
        if ( stats.type == Stats.Type.Ally )
        {
            if (target != null)
            {
                if (transform.root.gameObject.GetComponent<DemonMovement>().HorizDistFromTargetBorders(target) > maxMeleeDistance)
                    return;
            }

            if (target == null)
            {
                return;
            }
        }
        */

        if ( stats.CombatIdle )
        {
            if(rangedAttackCR == null) { 
                rangedAttackCR = StartCoroutine(RangedAttackCoroutine(target));    
            }
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.RangedAttack is trying a ranged attack but is not idle" );
        }
    }

    public void StopRangedAttack()
    {
        if (rangedAttackCR != null && !stats.CombatIdle )
        {
            //lancer.Stop();
            StopCoroutine(rangedAttackCR);
            rangedAttackCR = null;
            stats.CombatIdle = true;

            if(combatEventsManager != null) { 
                combatEventsManager.RaiseOnStopRangedAttack();    
            }
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.StopRangedAttack is trying to stop a ranged attack but it is idle" );
        }
    }

    // To avoid to perform actions before imps are at minimum distance
    public IEnumerator WaitTillMinDistance( float distance, GameObject target )
    {
        if ( !target )
            canAttack = false;

        while
            // horizontal distance of the parent game object to the target's borders
            ( transform.root.gameObject.GetComponent<DemonMovement>().HorizDistFromTargetBorders( target ) > distance )
        {
            //((target.transform.position - gameObject.transform.position).magnitude > distance) {
            canAttack = false;
            yield return new WaitForSeconds( 1f );
        }

        canAttack = true;
    }

    public void GroupAttack()
    {
        if ( stats.CombatIdle )
        {
            attackCollider.transform.localScale = new Vector3( stats.GroupAttackSize, attackCollider.transform.localScale.y, attackCollider.transform.localScale.z );
            attackCollider.GetComponent<AttackCollider>().isGroupAttacking = true;
            attackCR = StartCoroutine( AttackCoroutine() );
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.Sweep is trying a sweep attack but it is not idle" );
        }
    }

    public void StopGroupAttack()
    {
        if ( attackCR != null && !stats.CombatIdle )
        {
            StopCoroutine( attackCR );
            attackCR = null;

            attackCollider.transform.localPosition = startPosition;
            attackCollider.transform.localScale = baseAttackColliderScale;
            attackCollider.GetComponent<AttackCollider>().isGroupAttacking = false;
            attackCollider.SetActive( false );

            stats.CombatIdle = true;
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.StopSweep is trying to stop a sweep attack but it is idle" );
        }
        return;
    }

    public void GlobalAttack()
    {
        if ( stats.CombatIdle )
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
        if ( globalAttackCR != null && !stats.CombatIdle )
        {
            StopCoroutine( globalAttackCR );
            globalAttackCR = null;

            attackCollider.transform.localScale = baseAttackColliderScale;
            attackCollider.GetComponent<AttackCollider>().isGlobalAttacking = false;
            attackCollider.SetActive( false );

            stats.CombatIdle = true;
        }
        else
        {
            Debug.Log( this.transform.root.gameObject.name + " CombatManager.StopGlobalAttack is trying to stop a global attack but it is idle or globalAttackCr is null" );
        }
    }

    private IEnumerator AttackCoroutine()
    {
        stats.CombatIdle = false;

        AttackCollider attackColliderComponent = attackCollider.GetComponent<AttackCollider>();

        Vector3 targetPosition = attackCollider.transform.localPosition + new Vector3( 0.0f, 0.0f, stats.AttackRange );
        
        // If this is a regular attack we use regular attack delay
        if (!attackColliderComponent.isGroupAttacking) {
            yield return new WaitForSeconds(singleAttackDelayInSeconds);
        }

        // If this is a group attack we use group attack delay
        if (attackColliderComponent.isGroupAttacking) { 
            yield return new WaitForSeconds(groupAttackDelayInSeconds);    
        }
        
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

        if ( attackColliderComponent.isGroupAttacking )
        {
            attackCollider.transform.localScale = baseAttackColliderScale;
            attackColliderComponent.isGroupAttacking = false;
        }

        attackCollider.SetActive( false );

        stats.CombatIdle = true;
    }

    private IEnumerator RangedAttackCoroutine(GameObject target) {
        stats.CombatIdle = false;

        yield return new WaitForSeconds(rangedAttackDelayInSeconds);

        if (target != null)
        {
            lancer.Launch(target);
        }
        else
            Debug.Log(this.name + "Is trying a ranged attack to a null target");

        stats.CombatIdle = true;
        rangedAttackCR = null;    
    }

    private IEnumerator GlobalAttackCoroutine()
    {
        AttackCollider attackColliderComponent = attackCollider.GetComponent<AttackCollider>();

        stats.CombatIdle = false;

        attackColliderComponent.isGlobalAttacking = true;
        attackCollider.transform.localScale = new Vector3( stats.GlobalAttackSize, attackCollider.transform.localScale.y, stats.GlobalAttackSize );

        yield return new WaitForSeconds(globalAttackDelayInSeconds);

        attackCollider.SetActive( true );

        yield return new WaitForSeconds( stats.GlobalAttackDuration );

        attackCollider.transform.localScale = baseAttackColliderScale;
        attackColliderComponent.isGlobalAttacking = false;
        attackCollider.SetActive( false );

        stats.CombatIdle = true;
    }

    #endregion
}
