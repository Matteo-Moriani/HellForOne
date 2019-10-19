using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPositions : MonoBehaviour
{

    private Vector3[] positions;
    private bool[] available;

    // Start is called before the first frame update
    void Start()
    {
        positions = new Vector3[4];
        positions[0] = new Vector3(gameObject.transform.localScale.x, 0f, 0f);
        positions[1] = new Vector3(-gameObject.transform.localScale.x, 0f, 0f);
        positions[2] = new Vector3(gameObject.transform.localScale.x, 0f, 0f);
        positions[3] = new Vector3(gameObject.transform.localScale.x, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
