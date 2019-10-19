using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject groupBelongingTo;
    private bool groupFound = false;

    public GameObject group;
    public GameObject leader;
    public float speed = 8f;

    private GameObject currentTarget;
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

    

    private void Start()
    {
        FindGroup();
        currentTarget = group;
        targetCollider = currentTarget.GetComponent<Collider>();
    }

    private void Update()
    {
        if ( !groupFound )
            FindGroup();
    }

    void FixedUpdate() {

        if(HorizDistFromTargetBorders(currentTarget) < repulsionDist)
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * -1);
        else if(HorizDistFromTargetBorders(currentTarget) > repulsionDist && HorizDistFromTargetBorders(currentTarget) < meleeDist)
            GetComponent<NavMeshAgent>().destination = transform.position;
        else
            GetComponent<NavMeshAgent>().destination = targetCollider.ClosestPoint(transform.position);


    }

    private float HorizDistFromTargetBorders(GameObject target) {
        Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);
        Vector3 targetPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        return (targetPosition - transform.position).magnitude;
    }

    public void FindGroup() {
        GameObject[] go = GameObject.FindGameObjectsWithTag("group");
        foreach(GameObject g in go) {
            GameObject[] demonsArray = g.GetComponent<GroupBehaviour>().demons;
            for(int i = 0; i < demonsArray.Length; i++) {
                if(demonsArray[i] == null) {
                    demonsArray[i] = gameObject;
                    groupFound = true;
                    break;
                }
            }
            if(groupFound) {
                groupBelongingTo = g;
                break;
            }
        }
    }

}
