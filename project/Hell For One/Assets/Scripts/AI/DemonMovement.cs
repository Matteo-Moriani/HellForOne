using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonMovement : MonoBehaviour
{
    [Range( 0f, 1f )]
    public float facingSpeed = 0.1f;

    public float extraCohesion = 1.75f;
    public float minGroupRepulsion = 0.9f;
    public float maxGroupRepulsion = 1.5f;
    // distanza massima coperta dall'affondo
    private float maxMeleeDist = 0.85f;
    public float maxRangeDist = 8f;
    public float faceAllowedDegreeError = 10f;
    public float faceActualDegreeError = 0f;

    [SerializeField]
    private GameObject target;
    private GameObject player;
    private GameObject group;
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
    public bool InScriptedMovement { get => inScriptedMovement; set => inScriptedMovement = value; }
    public bool PlayerNotified { get => playerNotified; set => playerNotified = value; }
    private Vector3 agentDestination;
    private bool inFormationPosition = false;

    private bool inScriptedMovement = false;
    private bool playerNotified = false;

    private CombatEventsManager combatEventsManager;
    private NavMeshAgent agent;

    private Quaternion facingDir;

    private void OnEnable()
    {
        BattleEventsManager.onBossBattleExit += OnBossBattleExit;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBossBattleExit -= OnBossBattleExit;
    }

    void Start()
    {
        facingDir = Quaternion.LookRotation(transform.forward);
        player = GameObject.FindGameObjectWithTag( "Player" );
        myCollider = GetComponent<Collider>();

        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = maxMeleeDist;
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
                        CloseRangeMovement();
                    }
                    else
                        HighRangeMovement();

                }
                else if ( target.CompareTag( "LittleEnemy" ) )
                {
                    if ( gb.currentState == GroupBehaviour.State.MeleeAttack || gb.currentState == GroupBehaviour.State.Tank )
                    {
                        if ( (HorizDistFromTargetBorders( target ) > maxMeleeDist) )
                        {
                            agent.destination = target.transform.position;
                        }
                        else
                            agent.destination = transform.position;
                    }
                    else
                    {
                        if ( (HorizDistFromTargetBorders( target ) > maxRangeDist) )
                            agent.destination = target.transform.position;
                        else
                            agent.destination = transform.position;
                    }

                    Face(target);

                }
                // out of combat
                else {
                    // I move only if I'm far enough from the group center and if the group center is inside the navmesh
                    if(!InFormationPosition() && agent.CalculatePath(group.transform.position, new NavMeshPath())) {
                        agent.destination = group.transform.position;
                        agentDestination = agent.destination;
                        //Debug.Log(gameObject.name + " still not in position. agent desitination is " + agent.destination);
                    }
                    else {
                        agent.destination = transform.position;
                        Face(target);
                        if(InScriptedMovement && !PlayerNotified) {
                            PlayerNotified = true;
                            player.GetComponent<PlayerScriptedMovements>().NotifyInPosition();
                        }
                    }
                }
            }
        }
        ManageMovementEvents();
    }

    public bool InFormationPosition() {
        if(HorizDistFromTarget(group) <= minGroupRepulsion || (inFormationPosition && HorizDistFromTarget(group) < maxGroupRepulsion)) {
            inFormationPosition = true;
            return true;
        }
        else {
            inFormationPosition = false;
            return false;
        }
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
        Quaternion newFacingDir = Quaternion.LookRotation(vectorToTarget);

        faceActualDegreeError = Mathf.Abs(transform.rotation.eulerAngles.y - newFacingDir.eulerAngles.y);

        // i don't update the facing direction if i'm very close to the correct angle
        if(faceActualDegreeError > faceAllowedDegreeError || faceActualDegreeError < 360f - faceAllowedDegreeError) {
            facingDir = newFacingDir;
        }

        Quaternion newRotation = Quaternion.Slerp(transform.rotation, facingDir, facingSpeed);
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

            // se sono troppo vicino if i'm too close

            //if ( HorizDistFromTargetBorders( target ) < minMeleeDist )
            //{
            //    //GetComponent<Rigidbody>().AddForce(transform.position - targetCollider.ClosestPoint(transform.position));
            //    enemyComponent = transform.position;
            //    farFromEnemy = true;
            //}

        }

        // I move to the group only when I'm far from it as the group is far from the target borders
        if ( HorizDistFromTarget( group ) > group.GetComponent<GroupMovement>().HorizDistFromTargetBorders( target ) + extraCohesion )
        {
            groupComponent = group.transform.position;
            farFromGroup = true;
        }


        if ( farFromEnemy && farFromGroup )
            agent.destination = enemyComponent + groupComponent;
        else if ( farFromEnemy && !farFromGroup )
            agent.destination = enemyComponent;
        else if ( !farFromEnemy && farFromGroup )
            agent.destination = groupComponent;
    }

    private void HighRangeMovement()
    {
        if ( HorizDistFromTarget( group ) > transform.localScale.x * group.GetComponent<GroupMovement>().distanceAllowed && !inPosition )
        {
            if ( agent )
                agent.destination = group.transform.position;
        }
        else
        {
            Face( target );
            agent.destination = transform.position;
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
       if(agent.velocity.magnitude > 0.3) {
            if (!isMoving)
            {
                if (combatEventsManager != null)
                {
                    combatEventsManager.RaiseOnStartMoving();
                }

                isMoving = true;
            }
       }
        // TODO - Parametrize this velocity
        if(agent.velocity.magnitude <= 0.3) {
            if (isMoving) { 
                if(combatEventsManager != null) { 
                    combatEventsManager.RaiseOnStartIdle();    
                }
                isMoving = false;
            }    
        }
            
    }

    private void OnBossBattleExit() { 
        canMove = true;    
    }
}
