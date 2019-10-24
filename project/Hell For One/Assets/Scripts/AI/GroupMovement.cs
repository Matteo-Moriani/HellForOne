using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroupMovement : MonoBehaviour
{
    public GameObject enemyPositions;
    public GameObject groupsFormation;
    public float rangedDist = 6f;
    public float cohesionMultiplier = 2f;

    private GameObject target;
    private Transform targetPosition;
    private Transform meleePosition;
    private Transform rangedPosition;
    private Transform outOfCombatPosition;
    private bool haveTarget = false;
    private GroupBehaviour gb;
    private bool vsLittleEnemies = false;
    private bool vsBoss = false;
    private bool outOfCombat = false;
    private bool inRangedPosition = false;
    private float distanceInPosition = float.MaxValue;
    private float distanceFromPlayer;
    private GameObject player;
    
    void Start()
    {
        targetPosition = gameObject.transform;
        distanceFromPlayer = Random.Range(2f, 5f);
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    void Update()
    {
        if(!haveTarget) {

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Little Enemy");
            if(enemies.Length != 0) {
                SetVsLittleEnemies();
                target = enemies[Random.Range(0, enemies.Length)];
                SetDemonsTarget(target);
                haveTarget = true;
            } else {
                GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
                if(enemy) {
                    SetVsBoss();
                    target = enemy;
                    SetDemonsTarget(target);
                } else {
                    SetOutOfCombat();
                    target = player;
                    SetDemonsTarget(target);
                }
            }

            // mi sa che non serve più
            if(vsBoss) {
                foreach(Transform position in enemyPositions.GetComponent<EnemyPositions>().GetMeleePositions()) {
                    if(enemyPositions.GetComponent<EnemyPositions>().GetAvailability(position)) {
                        meleePosition = position;
                        rangedPosition = enemyPositions.GetComponent<EnemyPositions>().GetClosestRanged(position);
                        enemyPositions.GetComponent<EnemyPositions>().SetAvailability(position, false);
                        haveTarget = true;

                        // sostituire con questo

                        //switch(outOfCombatPosition.name) {
                        //    case "right":
                        //        break;
                        //    case "left":
                        //        break;
                        //    case "front":
                        //        break;
                        //    case "behind":
                        //        break;
                        //    default:
                        //        break;
                        //}

                        break;
                    }
                }
            }
            if(outOfCombat) {
                foreach(Transform position in groupsFormation.GetComponent<GroupsFormation>().GetPositions()) {
                    if(groupsFormation.GetComponent<GroupsFormation>().GetAvailability(position)) {
                        outOfCombatPosition = position;
                        Debug.Log(position.transform.position);
                        groupsFormation.GetComponent<GroupsFormation>().SetAvailability(position, false);
                        haveTarget = true;
                        break;
                    }
                }
            }

        }

        // i update the target position only if the group is too far to do its things
        if(vsBoss) {
            switch(gb.currentState) {
                case GroupBehaviour.State.MeleeAttack:
                case GroupBehaviour.State.Tank:
                    targetPosition = meleePosition;
                    inRangedPosition = false;
                    break;
                case GroupBehaviour.State.RangeAttack:
                case GroupBehaviour.State.Support:
                    if(!inRangedPosition) {
                        targetPosition = rangedPosition;
                        inRangedPosition = true;
                        distanceInPosition = (transform.position - target.transform.position).magnitude;
                    }
                    break;
                default:
                    break;
            }
            // if the boss is escaped since I positioned
            if(distanceInPosition < (transform.position - target.transform.position).magnitude) {
                inRangedPosition = false;
                distanceInPosition = float.MaxValue;
            }
        }
        else if (vsLittleEnemies){
            switch(gb.currentState) {
                case GroupBehaviour.State.MeleeAttack:
                    targetPosition = target.transform;
                    break;
                case GroupBehaviour.State.Tank:
                    targetPosition = target.transform;
                    break;
                case GroupBehaviour.State.RangeAttack:
                    if(HorizDistFromTargetBorders(target) > rangedDist)
                        targetPosition = target.transform;
                    break;
                case GroupBehaviour.State.Support:
                    if(HorizDistFromTargetBorders(target) > rangedDist)
                        targetPosition = target.transform;
                    break;
                default:
                    break;
            }
        } else {
            FacePlayer();
            targetPosition = outOfCombatPosition;
            
        }
        
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

    private void SetDemonsTarget(GameObject target) {
        foreach(GameObject demon in GetComponent<GroupBehaviour>().demons) {
            demon.GetComponent<DemonMovement>().SetTarget(target);
        }
    }

    private void FacePlayer() {
        Vector3 targetPosition = player.transform.position;
        Vector3 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.y = 0f;
        Quaternion facingDir = Quaternion.LookRotation(vectorToTarget);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, facingDir, 0.1f);
        transform.rotation = newRotation;
    }

    private void SetVsLittleEnemies() {
        vsLittleEnemies = true;
        vsBoss = false;
        outOfCombat = false;
    }

    private void SetVsBoss() {
        vsLittleEnemies = false;
        vsBoss = true;
        outOfCombat = false;
    }

    private void SetOutOfCombat() {
        vsLittleEnemies = false;
        vsBoss = false;
        outOfCombat = true;
    }
}
