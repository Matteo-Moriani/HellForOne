using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFaceRotations : MonoBehaviour
{
    private Vector3 cameraPosition, rotateDirection;
    private bool bossFaceActive = false;
    private Quaternion lookRotation;

    public void BossFaceON()
    {
        GetComponent<Canvas>().enabled = true;
        bossFaceActive = true;
    }

    public void BossFaceOFF()
    {
        GetComponent<Canvas>().enabled = false;
        bossFaceActive = false;
    }

    void Update()
    {
        if ( bossFaceActive )
        {
            cameraPosition = Camera.main.transform.position;
            rotateDirection = (cameraPosition - transform.position).normalized;
            lookRotation = Quaternion.LookRotation( rotateDirection );
            transform.rotation = Quaternion.Slerp( transform.rotation, lookRotation, 1 );
        }
    }
}
