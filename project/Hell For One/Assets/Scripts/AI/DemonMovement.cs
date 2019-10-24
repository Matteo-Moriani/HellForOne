using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonMovement : MonoBehaviour
{
    public float speed = 8f;
    [Range(0f, 1f)]
    public float rotSpeed = 0.1f;
    public float minMeleeDist = 1f;
    public float extraCohesion = 1.75f;
    // only vs mobs
    public float rangedDist = 5f;
    public float repulsionWithGroup = 1f;

    [SerializeField]
    private GameObject target;
    private GameObject player;
    private float maxMeleeDist;
    private GameObject group;
    private Collider targetCollider;
    private bool farFromEnemy = true;
    private bool farFromGroup = true;
    private Collider myCollider;
    private GroupBehaviour gb;
    private bool inPosition = false;
    private float distanceInPosition;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        maxMeleeDist = minMeleeDist + 1f;
        myCollider = GetComponent<Collider>();
        target = gameObject;
    }
    
    void Update()
    {

    }

    void FixedUpdate() {

        if(!player)
            player = GameObject.FindGameObjectWithTag("Player");

        if(group == null) {
            if(GetComponent<DemonBehaviour>().groupFound) {
                group = GetComponent<DemonBehaviour>().groupBelongingTo;
                gb = group.GetComponent<GroupBehaviour>();
            }
        }
        else if (target){
            if(target.CompareTag("Enemy")) {
                if(gb.currentState == GroupBehaviour.State.MeleeAttack || gb.currentState == GroupBehaviour.State.Tank)
                    CloseRangeMovement();
                else
                    HighRangeMovement();

                if(distanceInPosition < (transform.position - target.transform.position).magnitude)
                    inPosition = false;
            }
            else if(target.CompareTag("Little Enemy")) {
                if(gb.currentState == GroupBehaviour.State.MeleeAttack || gb.currentState == GroupBehaviour.State.Tank) {
                    if((HorizDistFromTargetBorders() > minMeleeDist)) {
                        GetComponent<NavMeshAgent>().destination = target.transform.position;
                    }
                    else
                        GetComponent<NavMeshAgent>().destination = transform.position;
                }
                else {
                    if((HorizDistFromTargetBorders() > rangedDist))
                        GetComponent<NavMeshAgent>().destination = target.transform.position;
                    else
                        GetComponent<NavMeshAgent>().destination = transform.position;
                }

            }
            // out of combat
            else
                if(HorizDistFromTarget(group) > repulsionWithGroup)
                    GetComponent<NavMeshAgent>().destination = group.transform.position;
                else
                    GetComponent<NavMeshAgent>().destination = transform.position;
        }

    }

    private float HorizDistFromTargetBorders() {
        Vector3 closestPoint = targetCollider.ClosestPoint(myCollider.ClosestPoint(target.transform.position));
        Vector3 targetPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        return (targetPosition - transform.position).magnitude;
    }

    private float HorizDistFromTarget(GameObject target) {
        Vector3 closestPoint = target.transform.position;
        Vector3 targetPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        return (targetPosition - transform.position).magnitude;
    }

    private void FaceTarget() {
        Vector3 targetPosition = target.transform.position;
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
        if(HorizDistFromTarget(group) <= group.GetComponent<GroupMovement>().HorizDistFromTargetBorders(target) + extraCohesion && group != null) {

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
        if(HorizDistFromTarget(group) > group.GetComponent<GroupMovement>().HorizDistFromTargetBorders(target) + extraCohesion) {
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
        if(HorizDistFromTarget(group) > transform.localScale.x * group.GetComponent<GroupMovement>().cohesionMultiplier && !inPosition) {
            GetComponent<NavMeshAgent>().destination = group.transform.position;
        }
        else {
            FaceTarget();
            GetComponent<NavMeshAgent>().destination = transform.position;
            if(!inPosition) {
                inPosition = true;
                distanceInPosition = (transform.position - target.transform.position).magnitude;
            }
        }
    }

    public void SetTarget(GameObject target) {
        this.target = target;
        targetCollider = target.GetComponent<Collider>();
    }
}
