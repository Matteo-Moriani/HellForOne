using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [Header("Input")]
    public float zMovement, xMovement;
    public float moveAmount, moveDir;
    public bool run;

    [Header("Stats")]
    public float rotateSpeed = 5f;
    public float runSpeed = 10f;

    void Start()
    {
    }

    void Update()
    {
        zMovement = Input.GetAxis( "Vertical" );
        xMovement = Input.GetAxis( "Horizontal" );

        if ( (zMovement != 0f || xMovement != 0f) )
        {
            Vector3 vertical = zMovement * Camera.main.transform.forward;
            Vector3 horizontal = xMovement * Camera.main.transform.right;

            Vector3 moveDir = (vertical + horizontal).normalized;
            moveDir.y = 0f;

            float m = Mathf.Abs( zMovement ) + Mathf.Abs( xMovement );
            moveAmount = Mathf.Clamp01( m );

            transform.position += moveDir.normalized * runSpeed * Time.deltaTime;

            Vector3 targetDir = moveDir;
            targetDir.y = 0f;

            if ( targetDir == Vector3.zero )
                targetDir = Vector3.forward;

            Quaternion tr = Quaternion.LookRotation( targetDir );
            Quaternion targetRotation = Quaternion.Slerp( transform.rotation, tr, moveAmount * rotateSpeed );

            transform.rotation = targetRotation;
        }
    }
}
