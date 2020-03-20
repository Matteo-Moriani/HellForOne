using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LittleEnemyBehaviour : MonoBehaviour
{
    [Range(0f, 1f)]
    public float rotSpeed = 0.1f;
    public float attackRange = 0.85f;
    public float rateo = 2f;

    private float movingSpeedTreshold = 0.2f;
    private GameObject targetDemon;
    private GameObject player;
    private Combat combat;
    private Animator animator;
    private Collider targetCollider;
    private Collider myCollider;
    private Stats stats;

    private CombatEventsManager combatEventsManager;
    private NavMeshAgent agent;
    private bool isMoving = false;
    private bool isAlive = true;

    private void Awake()
    {
        stats = GetComponent<Stats>();
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        combat = GetComponent<Combat>();
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        stats.onDeath += OnDeath;
    }

    private void OnDisable()
    {
        stats.onDeath -= OnDeath;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(AttackOneTime(rateo));
    }

    private void FixedUpdate()
    {
        if (isAlive) {
            if (!player)
                player = GameObject.FindGameObjectWithTag("Player");

            if (targetDemon)
            {
                FaceTarget(targetDemon);

                if (HorizDistFromTargetBorders(targetDemon) > attackRange)
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
        //if(!animator.GetBool("isMeleeAttacking")) {
            //if(targetDemon.GetComponent<Stats>().ThisUnitType == Stats.Type.Ally) {
                //if(HorizDistFromTargetBorders(targetDemon) < attackRange)
                    //combat.PlayerAttack();
            //} else
                //combat.PlayerAttack();

        //}
    }

    public IEnumerator AttackOneTime(float rateo) {
        while(true) {
            yield return new WaitForSeconds(rateo);
            Attack();
        }
    }

    private void OnDeath(Stats sender) { 
        StopAllCoroutines();
        agent.enabled = false;
        isAlive = false;
    }

    public float HorizDistFromTargetBorders(GameObject target) {
        if(!targetCollider)
            targetCollider = target.GetComponent<Collider>();
        Vector3 closestPoint = targetCollider.ClosestPoint(myCollider.ClosestPoint(target.transform.position));
        Vector3 targetPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        return (targetPosition - transform.position).magnitude;
    }
}
