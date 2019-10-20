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
    private float[] aggroValues;
    private float[] probability;
    private bool haveTarget = false;

    
    void Start()
    {
        demonGroups = GameObject.FindGameObjectsWithTag("group");
        aggroValues = new float[demonGroups.Length];
        probability = new float[demonGroups.Length + 1];
        probability[0] = 0f;
    }
    
    void Update()
    {
        
    }

    void FixedUpdate() {

        if(haveTarget) {

            FaceTarget();

            if((targetDemon.transform.position - transform.position).magnitude > stopDist) {
                GetComponent<NavMeshAgent>().destination = targetDemon.transform.position;
            }
            else {
                GetComponent<NavMeshAgent>().destination = transform.position;
            }
        }
        else
            ChooseTarget();
    }

    private void FaceTarget() {
        Vector3 targetPosition = targetDemon.transform.position;
        Vector3 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.y = 0f;
        Quaternion facingDir = Quaternion.LookRotation(vectorToTarget);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, facingDir, rotSpeed);
        transform.rotation = newRotation;       
    }

    public void ChooseTarget() {

        if(GameObject.FindGameObjectsWithTag("group").Length == 0)
            return;

        GameObject targetGroup;

        float totalAggro = 0f;
        for(int i = 0; i < demonGroups.Length; i++) {
            //aggroValues[i] = demonGroups[i].GetComponent<TargetScript>().GetAggro();
            aggroValues[i] = 3;

            //totalAggro = totalAggro + demonGroups[i].GetComponent<TargetScript>().GetAggro();
            totalAggro = totalAggro + 3;

            probability[i + 1] = totalAggro;
        }

        float random = Random.Range(0.001f, totalAggro);

        for(int i = 1; i < probability.Length; i++) {
            if(random > probability[i - 1] && random <= probability[i]) {
                targetGroup = demonGroups[i - 1];
                GroupBehaviour gb = targetGroup.GetComponent<GroupBehaviour>();
                int index = Random.Range(0, gb.demons.Length);
                targetDemon = gb.demons[index];
                haveTarget = true;
                return;
            }
        }
        
    }
}
