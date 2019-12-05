using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Position {
    front,
    behind,
    left,
    right
}

public class MeleePositionScript : MonoBehaviour
{
    public Position position;
    private Transform groupsFormation;
    float correction = 2.5f;

    private void Awake() {
        groupsFormation = GameObject.FindGameObjectWithTag("GroupsFormation").transform;
    }

    void FixedUpdate()
    {
        NavMeshHit hit;
        Vector3 rayOrigin = new Vector3(0, 0, 0);

        switch(position) {
            case Position.front:
                rayOrigin = groupsFormation.position + groupsFormation.forward * correction;
                break;
            case Position.behind:
                rayOrigin = groupsFormation.position + groupsFormation.forward * correction * -1f;
                break;
            case Position.left:
                rayOrigin = groupsFormation.position + groupsFormation.right * correction * -1f;
                break;
            case Position.right:
                rayOrigin = groupsFormation.position + groupsFormation.right * correction;
                break;
            default:
                break;
        }

        if(NavMesh.SamplePosition(rayOrigin, out hit, 3f, NavMesh.AllAreas))
            transform.position = hit.position;
        else if(NavMesh.SamplePosition(rayOrigin, out hit, 3f, NavMesh.AllAreas))
            transform.position = hit.position;
        else
            Debug.Log("I didn't found a navmesh through raycast");

    }
}
