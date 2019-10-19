using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroupMovement : MonoBehaviour
{

    //[RequireComponent(EnemyPositions)]
    public GameObject enemy;
    public float speed = 8f;

    private GameObject currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform position in enemy.GetComponent<EnemyPositions>().GetPositions()) {
            if(enemy.GetComponent<EnemyPositions>().GetAvailability(position)) {
                currentTarget = position.gameObject;
                enemy.GetComponent<EnemyPositions>().SetAvailability(position, false);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {

        GetComponent<NavMeshAgent>().destination = currentTarget.transform.position;


    }

    private float HorizDistFromTargetBorders(GameObject target) {
        //Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);
        Vector3 closestPoint = target.transform.position;
        Vector3 targetPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        return (targetPosition - transform.position).magnitude;
    }
}
