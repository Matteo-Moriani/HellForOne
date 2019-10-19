using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonMovement : MonoBehaviour
{


    // togliere il layer group come ostacolo della navmesh




    //[RequireComponent(EnemyPositions)]
    public GameObject enemy;
    public float speed = 8f;

    private GameObject currentTarget;
    private GameObject group;
    private Collider targetCollider;
    private int groupSize;
    private float repulsionDist = 0.7f;
    private float meleeDist = 1.5f;
    private float cohesionDist;
    private Collider[] members = new Collider[17];
    private bool canMove = true;

    // repulsion su tutti i demonietti e sui boss
    // cohesion sul suo gruppetto
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



        Vector3 enemyComponent = transform.position;
        Vector3 groupComponent = transform.position;

        if(HorizDistFromTargetBorders(currentTarget) < repulsionDist)
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * -1);
        //else if(HorizDistFromTargetBorders(currentTarget) > repulsionDist && HorizDistFromTargetBorders(currentTarget) < meleeDist)
        //    enemyComponent = transform.position;
        else if(HorizDistFromTargetBorders(currentTarget) > meleeDist)
            enemyComponent = targetCollider.ClosestPoint(transform.position);

        if(group == null) {
            if(GetComponent<DemonBehaviour>().groupFound){
                group = GetComponent<DemonBehaviour>().groupBelongingTo;
            }
        } else {
            if(HorizDistFromTarget(group) > 1) {
                groupComponent = group.transform.position;
            }
        }

        GetComponent<NavMeshAgent>().destination = enemyComponent + groupComponent;

    }

    private float HorizDistFromTargetBorders(GameObject target) {
        Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);
        //Vector3 closestPoint = target.transform.position;
        Vector3 targetPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        return (targetPosition - transform.position).magnitude;
    }

    private float HorizDistFromTarget(GameObject target) {
        //Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);
        Vector3 closestPoint = target.transform.position;
        Vector3 targetPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        return (targetPosition - transform.position).magnitude;
    }
}
