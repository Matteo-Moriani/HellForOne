using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LittleEnemyBehaviour : MonoBehaviour
{
    public float speed = 8f;
    [Range(0f, 1f)]
    public float rotSpeed = 0.1f;
    public float stopDist = 1.5f;

    private GameObject targetDemon;
    private GameObject[] demonGroups;
    private GameObject[] allies;
    private GameObject player;
    private float[] aggroValues;
    private float[] probability;

    private CombatEventsManager combatEventsManager;
    private NavMeshAgent agent;
    private bool isMoving = false;


    void Start()
    {
        demonGroups = GameObject.FindGameObjectsWithTag("Group");
        player = GameObject.FindGameObjectWithTag("Player");
        allies = GameObject.FindGameObjectsWithTag("LittleEnemy");
        aggroValues = new float[demonGroups.Length + 1];
        probability = new float[demonGroups.Length + 2];
        probability[0] = 0f;

        combatEventsManager = this.gameObject.GetComponent<CombatEventsManager>();
        agent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        if (targetDemon)
        {

            FaceTarget(targetDemon);

            if ((targetDemon.transform.position - transform.position).magnitude > stopDist)
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

        if (GameObject.FindGameObjectsWithTag("Group").Length == 0 && !player)
            return;

        // this includes the player
        GameObject targetGroup;

        float totalAggro = 0f;
        for (int i = 0; i < demonGroups.Length; i++)
        {
            //aggroValues[i] = demonGroups[i].GetComponent<TargetScript>().GetAggro();
            aggroValues[i] = 5;

            //totalAggro = totalAggro + demonGroups[i].GetComponent<TargetScript>().GetAggro();
            totalAggro = totalAggro + 5;

            probability[i + 1] = totalAggro;
        }
        // Get player aggro
        aggroValues[demonGroups.Length] = 5;
        totalAggro = totalAggro + 5;
        probability[demonGroups.Length + 1] = totalAggro;

        float random = Random.Range(0.001f, totalAggro);

        for (int i = 1; i < probability.Length; i++)
        {
            if (random > probability[i - 1] && random <= probability[i])
            {
                if (i < probability.Length - 1)
                {
                    targetGroup = demonGroups[i - 1];
                    GroupBehaviour gb = targetGroup.GetComponent<GroupBehaviour>();
                    int index = Random.Range(0, gb.demons.Length);
                    targetDemon = gb.demons[index];
                }
                else
                    targetDemon = player;

                return;
            }
        }

    }

    private void ManageMovementEvents()
    {
        // TODO - Parametrize this velocity
        if (agent.velocity.magnitude > 0.2)
        {
            if (!isMoving)
            {
                if (combatEventsManager != null)
                {
                    combatEventsManager.RaiseOnStartRunning();
                }

                isMoving = true;
            }
        }
        // TODO - Parametrize this velocity
        if (agent.velocity.magnitude <= 0.2)
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
}
