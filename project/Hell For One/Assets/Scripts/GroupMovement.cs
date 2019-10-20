using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroupMovement : MonoBehaviour
{

    //[RequireComponent(EnemyPositions)]
    public GameObject enemy;

    private Transform targetPosition;
    private Transform meleePosition;
    private Transform rangedPosition;
    private bool targetAquired = false;
    private GroupBehaviour gb;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(!targetAquired) {
            foreach(Transform position in enemy.GetComponent<EnemyPositions>().GetMeleePositions()) {
                if(enemy.GetComponent<EnemyPositions>().GetAvailability(position)) {
                    meleePosition = position;
                    rangedPosition = enemy.GetComponent<EnemyPositions>().GetClosestRanged(position);
                    enemy.GetComponent<EnemyPositions>().SetAvailability(position, false);
                    targetAquired = true;
                    break;
                }
            }
        }
        if(gb.currentState == GroupBehaviour.State.MeleeAttack || gb.currentState == GroupBehaviour.State.Tank)
            targetPosition = meleePosition;
        else
            targetPosition = rangedPosition;

    }

    void FixedUpdate() {

        transform.position = targetPosition.position;
        
    }

    public float HorizDistFromTargetBorders(GameObject target) {
        Vector3 closestPoint = target.GetComponent<Collider>().ClosestPoint(transform.position);
        Vector3 targetPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        return (targetPosition - transform.position).magnitude;
    }

    private void Awake() {
        gb = GetComponent<GroupBehaviour>();
    }
}
