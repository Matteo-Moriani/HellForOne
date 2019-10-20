using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonMovement : MonoBehaviour
{
    //[RequireComponent(EnemyPositions)]
    public GameObject enemy;
    public float speed = 8f;
    [Range(0f, 1f)]
    public float rotSpeed = 0.1f;
    public float minMeleeDist = 1f;
    public float extraCohesion = 1.75f;

    private float maxMeleeDist;
    private GameObject currentTarget;
    private GameObject group;
    private Collider targetCollider;
    private bool farFromEnemy = true;
    private bool farFromGroup = true;
    private Collider myCollider;
    private GroupBehaviour gb;
    private bool inPosition = false;
    private float distanceInPosition;

    // face nemico target o align demone del giocatore fuori dal combattimento

    // Start is called before the first frame update
    void Start()
    {
        maxMeleeDist = minMeleeDist + 1f;
        myCollider = GetComponent<Collider>();
        currentTarget = enemy;
        targetCollider = currentTarget.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate() {
        if(group == null) {
            if(GetComponent<DemonBehaviour>().groupFound) {
                group = GetComponent<DemonBehaviour>().groupBelongingTo;
                gb = group.GetComponent<GroupBehaviour>();
            }
        }
        else {
            if(gb.currentState == GroupBehaviour.State.MeleeAttack || gb.currentState == GroupBehaviour.State.Tank)
                CloseRangeMovement();
            else
                HighRangeMovement();
        }

        if(distanceInPosition < (transform.position - currentTarget.transform.position).magnitude)
            inPosition = false;

    }

    private float HorizDistFromTargetBorders() {
        Vector3 closestPoint = targetCollider.ClosestPoint(myCollider.ClosestPoint(currentTarget.transform.position));
        Vector3 targetPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        return (targetPosition - transform.position).magnitude;
    }

    private float HorizDistFromTarget(GameObject target) {
        Vector3 closestPoint = target.transform.position;
        Vector3 targetPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        return (targetPosition - transform.position).magnitude;
    }

    private void FaceTarget() {
        Vector3 targetPosition = currentTarget.transform.position;
        Vector3 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.y = 0f;
        Quaternion facingDir = Quaternion.LookRotation(vectorToTarget);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, facingDir, rotSpeed);
        transform.rotation = newRotation;
    }

    private void CloseRangeMovement() {
        farFromEnemy = false;
        farFromGroup = false;

        Vector3 enemyComponent = transform.position;
        Vector3 groupComponent = transform.position;

        
        // I move to the enemy only if I'm far from melee distance and close enough to my group
        if(HorizDistFromTarget(group) <= group.GetComponent<GroupMovement>().HorizDistFromTargetBorders(currentTarget) + extraCohesion && group != null) {

            FaceTarget();

            if(HorizDistFromTargetBorders() > maxMeleeDist) {
                enemyComponent = targetCollider.ClosestPoint(transform.position);
                farFromEnemy = true;
            }

            if(HorizDistFromTargetBorders() < minMeleeDist) {
                //GetComponent<Rigidbody>().AddForce(transform.position - targetCollider.ClosestPoint(transform.position));
                enemyComponent = transform.position;
                farFromEnemy = true;
            }            

        }

        // I move to the group only when I'm far from it as the group is far from the target borders
        if(HorizDistFromTarget(group) > group.GetComponent<GroupMovement>().HorizDistFromTargetBorders(currentTarget) + extraCohesion) {
            groupComponent = group.transform.position;
            farFromGroup = true;
        }
        

        if(farFromEnemy && farFromGroup)
            GetComponent<NavMeshAgent>().destination = enemyComponent + groupComponent;
        else if(farFromEnemy && !farFromGroup)
            GetComponent<NavMeshAgent>().destination = enemyComponent;
        else if(!farFromEnemy && farFromGroup)
            GetComponent<NavMeshAgent>().destination = groupComponent;
    }

    private void HighRangeMovement() {
        if(HorizDistFromTarget(group) > transform.localScale.x * 2.5f && !inPosition) {
            GetComponent<NavMeshAgent>().destination = group.transform.position;
        }
        else {
            FaceTarget();
            GetComponent<NavMeshAgent>().destination = transform.position;
            if(!inPosition) {
                inPosition = true;
                distanceInPosition = (transform.position - currentTarget.transform.position).magnitude;
            }
        }
    }
}
