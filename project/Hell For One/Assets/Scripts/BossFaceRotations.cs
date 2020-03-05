using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFaceRotations : MonoBehaviour
{
    private Vector3 cameraPosition, rotateDirection;
    private bool bossFaceActive = true;
    private Quaternion lookRotation;

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
