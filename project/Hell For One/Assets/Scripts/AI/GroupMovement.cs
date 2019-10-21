using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroupMovement : MonoBehaviour
{
    public GameObject positions;

    private GameObject targetEnemy;
    private Transform targetPosition;
    private Transform meleePosition;
    private Transform rangedPosition;
    private bool haveTarget = false;
    private GroupBehaviour gb;
    private bool vsLittleEnemies;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(!haveTarget) {

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Little Enemy");
            if(enemies.Length != 0) {
                vsLittleEnemies = true;
                targetEnemy = enemies[Random.Range(0, enemies.Length)];
                SetDemonsTarget();
                haveTarget = true;
            } else {
                vsLittleEnemies = false;
                GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
                if(enemy.Length != 0) {
                    targetEnemy = enemy[0];
                    SetDemonsTarget();
                    haveTarget = true;
                }
                // else il target è il leader
            }

            if(!vsLittleEnemies) {
                foreach(Transform position in positions.GetComponent<EnemyPositions>().GetMeleePositions()) {
                    if(positions.GetComponent<EnemyPositions>().GetAvailability(position)) {
                        meleePosition = position;
                        rangedPosition = positions.GetComponent<EnemyPositions>().GetClosestRanged(position);
                        positions.GetComponent<EnemyPositions>().SetAvailability(position, false);
                        haveTarget = true;
                        break;
                    }
                }
            }
            
        }

        if(!vsLittleEnemies) {
            if(gb.currentState == GroupBehaviour.State.MeleeAttack || gb.currentState == GroupBehaviour.State.Tank)
                targetPosition = meleePosition;
            else
                targetPosition = rangedPosition;
        }
        else
            targetPosition = targetEnemy.transform;
        
    }

    void FixedUpdate() {

        if(targetPosition)
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

    private void SetDemonsTarget() {
        foreach(GameObject demon in GetComponent<GroupBehaviour>().demons) {
            demon.GetComponent<DemonMovement>().SetTargetEnemy(targetEnemy);
        }
    }
}
