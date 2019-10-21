using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAway : MonoBehaviour
{
    public float repulsionDist = 0.3f;
    public float pushIntensity = 0.75f;
    private GameObject[] demons;
    
    void Start()
    {
        demons = GameObject.FindGameObjectsWithTag("Demon");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        PushEnemies();
    }

    private float HorizDistFromTarget(Vector3 targetPosition) {
        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        return (targetPosition - transform.position).magnitude - transform.localScale.x/2;
    }

    private void PushEnemies() {
        foreach(GameObject demon in demons) {
            // closest point of demon's collider to the closest point of my collider to the demon center
            if(HorizDistFromTarget(demon.GetComponent<Collider>().ClosestPoint(GetComponent<Collider>().ClosestPoint(demon.transform.position))) < repulsionDist) {
                demon.GetComponent<Rigidbody>().AddForce((demon.transform.position - transform.position)*pushIntensity, ForceMode.Impulse);
            }
        }
    }
}
