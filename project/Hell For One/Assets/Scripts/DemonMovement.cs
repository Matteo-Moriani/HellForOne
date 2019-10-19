using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonMovement : MonoBehaviour
{
    //[RequireComponent(EnemyPositions)]
    public GameObject enemy;
    public float speed = 8f;
    public float meleeDist = 1.5f;
    public float rangedDist = 5f;

    private GameObject currentTarget;
    private GameObject group;
    private Collider targetCollider;
    private bool farFromEnemy = true;
    private bool farFromGroup = true;

    // definire bene a che distanza puntare cosa





    // face nemico target o align demone del giocatore fuori dal combattimento

    // Start is called before the first frame update
    void Start()
    {
        currentTarget = enemy;
        targetCollider = currentTarget.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate() {

        farFromEnemy = false;
        farFromGroup = false;

        Vector3 enemyComponent = transform.position;
        Vector3 groupComponent = transform.position;

        //if(HorizDistFromTargetBorders(currentTarget) < repulsionDist)
        //    gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * -1);
        //else if(HorizDistFromTargetBorders(currentTarget) > repulsionDist && HorizDistFromTargetBorders(currentTarget) < meleeDist)
        //    enemyComponent = transform.position;
        if(HorizDistFromTargetBorders(currentTarget) < rangedDist && HorizDistFromTargetBorders(currentTarget) > meleeDist && group != null) {
            enemyComponent = targetCollider.ClosestPoint(transform.position);
            farFromEnemy = true;
        }
        //else if(HorizDistFromTargetBorders(currentTarget) > rangedDist && group != null){
        //    enemyComponent = targetCollider.ClosestPoint(group.transform.position);
        //    farFromEnemy = true;
        //}

        if(group == null) {
            if(GetComponent<DemonBehaviour>().groupFound){
                group = GetComponent<DemonBehaviour>().groupBelongingTo;
            }
        } else {
            if(HorizDistFromTarget(group) > group.GetComponent<GroupMovement>().cohesion && HorizDistFromTargetBorders(currentTarget) > meleeDist) {
                groupComponent = group.transform.position;
                farFromGroup = true;
            }
        }

        if(farFromEnemy && farFromGroup)
            GetComponent<NavMeshAgent>().destination = enemyComponent + groupComponent;
        else if(farFromEnemy && !farFromGroup)
            GetComponent<NavMeshAgent>().destination = enemyComponent;
        else if(!farFromEnemy && farFromGroup)
            GetComponent<NavMeshAgent>().destination = groupComponent;

    }

    private float HorizDistFromTargetBorders(GameObject target) {
        Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);
        Vector3 targetPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        return (targetPosition - transform.position).magnitude;
    }

    private float HorizDistFromTarget(GameObject target) {
        Vector3 closestPoint = target.transform.position;
        Vector3 targetPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        return (targetPosition - transform.position).magnitude;
    }
}
