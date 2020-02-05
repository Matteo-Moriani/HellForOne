using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyTrackTargetMovement : MonoBehaviour
{
    public float speed;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime; 
    }
}
