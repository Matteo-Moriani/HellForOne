using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LittleEnemyBehaviour : MonoBehaviour
{
    public float speed = 5f;
    [Range(0f, 1f)]
    public float rotSpeed = 0.1f;
    public float attackRange = 1f;

    private float movingSpeedTreshold = 0.2f;
    private GameObject targetDemon;
    private GameObject player;
    private Combat combat;
    private Animator animator;

    private CombatEventsManager combatEventsManager;
    private NavMeshAgent agent;
    private bool isMoving = false;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        combat = GetComponent<Combat>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        if (targetDemon)
        {
            FaceTarget(targetDemon);

            if ((targetDemon.transform.position - transform.position).magnitude > agent.stoppingDistance)
            {
                GetComponent<NavMeshAgent>().destination = targetDemon.transform.position;
            }
            else
            {
                GetComponent<NavMeshAgent>().destination = transform.position;
                if(IsInRange()) {
                    //combat.SingleAttack(targetDemon);
                    combat.PlayerAttack();
                    //Attack();
                }
            }


        }
        else
            ChooseTarget();

        ManageMovementEvents();
    }

    private void FaceTarget(GameObject target)
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.y = 0f;
        Quaternion facingDir = Quaternion.LookRotation(vectorToTarget);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, facingDir, rotSpeed);
        transform.rotation = newRotation;
    }

    public void ChooseTarget()
    {
        int random = Random.Range(0, AlliesManager.Instance.AlliesList.Count+1);

        if(random == AlliesManager.Instance.AlliesList.Count)
            targetDemon = player;
        else
            targetDemon = AlliesManager.Instance.AlliesList[random];
    }

    private void ManageMovementEvents()
    {
        // TODO - Parametrize this velocity
        if (agent.velocity.magnitude > movingSpeedTreshold)
        {
            if (!isMoving)
            {
                if (combatEventsManager != null)
                {
                    combatEventsManager.RaiseOnStartMoving();
                }

                isMoving = true;
            }
        }
        // TODO - Parametrize this velocity
        if (agent.velocity.magnitude <= movingSpeedTreshold)
        {
            if (isMoving)
            {
                if (combatEventsManager != null)
                {
                    combatEventsManager.RaiseOnStartIdle();
                }
                isMoving = false;
            }
        }
    }

    public bool IsInRange() {
        if((targetDemon.transform.position - transform.position).magnitude <= attackRange)
            return true;
        else
            return false;
    }

    public void Attack() {

        // TODO - Find a better bools
        if(!animator.GetBool("isSingleAttacking")) {
            if(targetDemon.GetComponent<DemonMovement>().HorizDistFromTargetBorders(this.gameObject)<1.5f)
                combat.SingleAttack(targetDemon);
        }
    }
}
