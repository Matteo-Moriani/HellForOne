using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class BossBehavior : MonoBehaviour
{
    private GameObject[] demonGroups;
    private GameObject currentTarget;
    private float[] aggroValues;
    private float[] probability;
    private readonly float singleAttackProb = 0.6f;
    private readonly float groupAttackProb = 0.3f;
    private readonly float globalAttackProb = 0.1f;
    // initialized at true to avoid attacking himself at the beginning
    private bool attacked = true;
    
    void Start()
    {
        // the initial target is himself to stay on his place for the first seconds
        currentTarget = gameObject;
        demonGroups = GameObject.FindGameObjectsWithTag("Demon");
        aggroValues = new float[demonGroups.Length];
        probability = new float[demonGroups.Length + 1];
        probability[0] = 0f;
        StartCoroutine(MainRoutine());
        //foreach(GameObject g in demonGroups) {
        //    Debug.Log(g.name + " initial aggro " + g.GetComponent<TargetScript>().GetAggro());
        //}
    }
    
    void Update()
    {
        if(HorizDistFromTarget(currentTarget) <= 4f && attacked == false) {
            RandomAttack(currentTarget);
            attacked = true;
        }
    }

    private void ChooseTarget() {
        float totalAggro = 0f;
        for(int i = 0; i < demonGroups.Length; i++) {
            aggroValues[i] = demonGroups[i].GetComponent<TargetScript>().GetAggro();
            totalAggro = totalAggro + demonGroups[i].GetComponent<TargetScript>().GetAggro();
            probability[i + 1] = totalAggro;
        }
        //foreach(GameObject g in demonGroups) {
        //    Debug.Log(g.name + " now has aggro " + g.GetComponent<TargetScript>().GetAggro());
        //}

        float random = Random.Range(0.001f, totalAggro);
        //Debug.Log("random value: " + random);

        for(int i = 1; i < probability.Length; i++) {
            if(random > probability[i - 1] && random <= probability[i])
                currentTarget = demonGroups[i - 1];
        }

        //Debug.Log("current target is " + currentTarget.name);
    }

    private IEnumerator MainRoutine() {
        while(true) {
            yield return new WaitForSeconds(5);

            ChooseTarget();
            attacked = false;
            MoveToTarget();
        }
    }

    private void MoveToTarget() {
        GetComponent<NavMeshAgent>().destination = currentTarget.GetComponent<Transform>().position;
    }

    private void RandomAttack(GameObject targetGroup) {
        float random = Random.Range(0f, singleAttackProb+groupAttackProb+globalAttackProb);
        if(random < singleAttackProb)
            SingleAttack();
        else if(random >= singleAttackProb && random < singleAttackProb + groupAttackProb)
            GroupAttack();
        else
            GlobalAttack();
    }

    private void SingleAttack() {
        Debug.Log("single attack!");
    }

    private void GroupAttack() {
        Debug.Log("group attack!");
    }

    private void GlobalAttack() {
        Debug.Log("global attack!");
    }

    private float HorizDistFromTarget(GameObject target) {
        Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        return (targetPosition - transform.position).magnitude;
    }
}
