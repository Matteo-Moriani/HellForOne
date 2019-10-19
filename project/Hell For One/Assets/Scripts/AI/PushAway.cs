using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAway : MonoBehaviour
{
    public float repulsion = 1f;
    private GameObject[] demons;
    // Start is called before the first frame update
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
            if(HorizDistFromTarget(demon.GetComponent<Collider>().ClosestPoint(transform.position)) < 0.5f) {
                demon.GetComponent<Rigidbody>().AddForce((demon.transform.position - transform.position), ForceMode.Impulse);
            }
        }
    }
}
