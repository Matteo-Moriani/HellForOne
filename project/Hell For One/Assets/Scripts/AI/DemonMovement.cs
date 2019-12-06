using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonMovement : MonoBehaviour
{
    [Range( 0f, 1f )]
    public float facingSpeed = 0.1f;

    // Used to avoid melee atks and tanks if too distant
    private float minMeleeDist;
    private float maxMeleeDist;

    public float extraCohesion = 1.75f;
    // only vs mobs
    //public float rangedDist = 10f;
    public float repulsionWithGroup = 1f;

    [SerializeField]
    private GameObject target;
    private GameObject player;
    private GameObject group;
    [SerializeField]
    private Collider targetCollider;
    private bool farFromEnemy = true;
    private bool farFromGroup = true;
    private Collider myCollider;
    private GroupBehaviour gb;
    private bool inPosition = false;
    private float distanceInPosition = 0f;
    private bool canMove = true;
    public bool CanMove { get => canMove; set => canMove = value; }
    private bool isMoving = false;
    public bool IsMoving { get => isMoving; set => isMoving = value; }

    private CombatEventsManager combatEventsManager;
    private NavMeshAgent agent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag( "Player" );
        maxMeleeDist = GetComponentInChildren<CombatManager>().MaxMeleeDistance;
        minMeleeDist = maxMeleeDist - 1;
        myCollider = GetComponent<Collider>();

        combatEventsManager = this.gameObject.GetComponent<CombatEventsManager>();
        agent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        if ( CanMove )
        {
            if ( !player )
                player = GameObject.FindGameObjectWithTag( "Player" );

            if ( group == null )
            {
                if ( GetComponent<DemonBehaviour>().groupFound )
                {
                    group = GetComponent<DemonBehaviour>().groupBelongingTo;
                    gb = group.GetComponent<GroupBehaviour>();
                    // it's my group that decides my target
                    target = group.GetComponent<GroupMovement>().GetTarget();
                }
            }
            else if ( target )
            {
                if ( target.CompareTag( "Boss" ) )
                {
                    // if the boss is escaping...
                    if ( distanceInPosition < (transform.position - target.transform.position).magnitude )
                        inPosition = false;

                    if ( gb.currentState == GroupBehaviour.State.MeleeAttack || gb.currentState == GroupBehaviour.State.Tank )
                    {
                        distanceInPosition = 0f;

                        //TODO fix
                        CloseRangeMovement();
                    }
                    else
                        HighRangeMovement();

                }
                else if ( target.CompareTag( "LittleEnemy" ) )
                {
                    if ( gb.currentState == GroupBehaviour.State.MeleeAttack || gb.currentState == GroupBehaviour.State.Tank )
                    {
                        if ( (HorizDistFromTargetBorders( target ) > GetComponentInChildren<CombatManager>().MaxMeleeDistance) )
                        {
                            GetComponent<NavMeshAgent>().destination = target.transform.position;
                        }
                        else
                            GetComponent<NavMeshAgent>().destination = transform.position;
                    }
                    else
                    {
                        if ( (HorizDistFromTargetBorders( target ) > GetComponentInChildren<CombatManager>().MinRangeCombatDistance) )
                            GetComponent<NavMeshAgent>().destination = target.transform.position;
                        else
                            GetComponent<NavMeshAgent>().destination = transform.position;
                    }

                }
                // out of combat
                else {
                    // I move only if I'm far enough from the group center and if the group center is inside the navmesh
                    if(HorizDistFromTarget(group) > repulsionWithGroup && GetComponent<NavMeshAgent>().CalculatePath(group.transform.position, new NavMeshPath())) {
                        GetComponent<NavMeshAgent>().destination = group.transform.position;
                    }
                    else {
                        GetComponent<NavMeshAgent>().destination = transform.position;
                        Face(target);
                    }
                }
            }
        }
        ManageMovementEvents();
    }

    public float HorizDistFromTargetBorders( GameObject target )
    {
        if ( !targetCollider )
            targetCollider = target.GetComponent<Collider>();
        Vector3 closestPoint = targetCollider.ClosestPoint( myCollider.ClosestPoint( target.transform.position ) );
        Vector3 targetPosition = new Vector3( closestPoint.x, transform.position.y, closestPoint.z );
        return (targetPosition - transform.position).magnitude;
    }

    private float HorizDistFromTarget( GameObject target )
    {
        Vector3 closestPoint = target.transform.position;
        Vector3 targetPosition = new Vector3( closestPoint.x, transform.position.y, closestPoint.z );
        return (targetPosition - transform.position).magnitude;
    }

    private void Face( GameObject target )
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.y = 0f;
        Quaternion facingDir = Quaternion.LookRotation( vectorToTarget );
        Quaternion newRotation = Quaternion.Slerp( transform.rotation, facingDir, facingSpeed );
        transform.rotation = newRotation;
    }

    private void CloseRangeMovement()
    {
        farFromEnemy = false;
        farFromGroup = false;

        Vector3 enemyComponent = transform.position;
        Vector3 groupComponent = transform.position;


        // I move to the enemy only if I'm far from melee distance and close enough to my group
        if ( HorizDistFromTarget( group ) <= group.GetComponent<GroupMovement>().HorizDistFromTargetBorders( target ) + extraCohesion && group != null )
        {

            Face( target );

            if ( HorizDistFromTargetBorders( target ) > maxMeleeDist )
            {
                enemyComponent = targetCollider.ClosestPoint( transform.position );
                farFromEnemy = true;
            }

            if ( HorizDistFromTargetBorders( target ) < minMeleeDist )
            {
                //GetComponent<Rigidbody>().AddForce(transform.position - targetCollider.ClosestPoint(transform.position));
                enemyComponent = transform.position;
                farFromEnemy = true;
            }

        }

        // I move to the group only when I'm far from it as the group is far from the target borders
        if ( HorizDistFromTarget( group ) > group.GetComponent<GroupMovement>().HorizDistFromTargetBorders( target ) + extraCohesion )
        {
            groupComponent = group.transform.position;
            farFromGroup = true;
        }


        if ( farFromEnemy && farFromGroup )
            GetComponent<NavMeshAgent>().destination = enemyComponent + groupComponent;
        else if ( farFromEnemy && !farFromGroup )
            GetComponent<NavMeshAgent>().destination = enemyComponent;
        else if ( !farFromEnemy && farFromGroup )
            GetComponent<NavMeshAgent>().destination = groupComponent;
    }

    private void HighRangeMovement()
    {
        if ( HorizDistFromTarget( group ) > transform.localScale.x * group.GetComponent<GroupMovement>().distanceAllowed && !inPosition )
        {
            if ( gameObject.GetComponent<NavMeshAgent>() )
                GetComponent<NavMeshAgent>().destination = group.transform.position;
        }
        else
        {
            Face( target );
            GetComponent<NavMeshAgent>().destination = transform.position;
            if ( !inPosition )
            {
                inPosition = true;
                distanceInPosition = (transform.position - target.transform.position).magnitude;
            }
        }
    }

    public void SetTarget( GameObject target )
    {
        this.target = target;
        targetCollider = target.GetComponent<Collider>();
    }

    public bool CanAct()
    {
        switch ( gb.currentState )
        {
            case GroupBehaviour.State.MeleeAttack:
                if ( HorizDistFromTargetBorders( target ) <= maxMeleeDist )
                    return true;
                else
                    return false;
            case GroupBehaviour.State.Tank:
                return true;
            // deve anche essere ruotato correttamente
            case GroupBehaviour.State.RangeAttack:
                if ( inPosition )
                    return true;
                else
                    return false;
            case GroupBehaviour.State.Support:
                if ( inPosition )
                    return true;
                else
                    return false;
            default:
                return false;
        }
    }

    private void ManageMovementEvents() {
        // TODO - Parametrize this velocity
        if(agent.velocity.magnitude > 0.2) {
            if (!isMoving)
            {
                if (combatEventsManager != null)
                {
                    combatEventsManager.RaiseOnStartWalking();
                }

                isMoving = true;
            }
        }
        // TODO - Parametrize this velocity
        if(agent.velocity.magnitude <= 0.2) {
            if (isMoving) { 
                if(combatEventsManager != null) { 
                    combatEventsManager.RaiseOnStartIdle();    
                }
                isMoving = false;
            }    
        }
            
    }
}
