using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatteoSceneCamera : MonoBehaviour
{
    public float height = 25f;

    private GameObject boss;
    
    void Start()
    {

    }

    
    void Update()
    {
        if(!boss)
            boss = GameObject.FindGameObjectWithTag("Boss");

        gameObject.transform.position = new Vector3(boss.transform.position.x, height, boss.transform.position.z);
    }
}
