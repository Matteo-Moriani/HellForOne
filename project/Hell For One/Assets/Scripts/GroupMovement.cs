using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroupMovement : MonoBehaviour
{

    //[RequireComponent(EnemyPositions)]
    public GameObject enemy;
    public float cohesion = 2f;

    private Transform target;
    private bool targetAquired = false;

    // Start is called before the first frame update
    void Start()
    {
        target = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(!targetAquired) {
            foreach(Transform position in enemy.GetComponent<EnemyPositions>().GetPositions()) {
                if(enemy.GetComponent<EnemyPositions>().GetAvailability(position)) {
                    target = position;
                    enemy.GetComponent<EnemyPositions>().SetAvailability(position, false);
                    targetAquired = true;
                    break;
                }
            }
        }
    }

    void FixedUpdate() {

        transform.position = target.position;

    }
}
