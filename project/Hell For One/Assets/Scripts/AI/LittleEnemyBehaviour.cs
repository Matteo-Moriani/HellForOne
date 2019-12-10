using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LittleEnemyBehaviour : MonoBehaviour
{
    [Range(0f, 1f)]
    public float rotSpeed = 0.1f;
    //must be a little bit higher than navmesh stop distance
    public float attackRange = 1.5f;
    public float rateo = 2f;

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
        StartCoroutine(AttackOneTime(rateo));
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

    // stopping distance is only with transform.positions
    public bool IsInStopDist() {
        if((targetDemon.transform.position - transform.position).magnitude <= agent.stoppingDistance)
            return true;
        else
            return false;
    }

    private void Attack() {

        // TODO - Find a better bools
        // TODO - Understand why they doesn't attack the player
        if(!animator.GetBool("isMeleeAttacking")) {
            if(targetDemon.GetComponent<Stats>().type == Stats.Type.Ally) {
                if(targetDemon.GetComponent<DemonMovement>().HorizDistFromTargetBorders(gameObject) < attackRange)
                    combat.PlayerAttack();
            } else
                combat.PlayerAttack();

        }
    }

    public IEnumerator AttackOneTime(float rateo) {
        while(true) {
            yield return new WaitForSeconds(rateo);
            Attack();
        }
    }
}
