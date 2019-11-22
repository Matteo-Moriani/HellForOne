using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class Controller : MonoBehaviour
{
    [Header( "Input" )]
    private float zMovement, xMovement;
    private float moveAmount, moveDir;

    [Header( "Stats" )]
    [SerializeField]
    private float rotateSpeed = 5f;
    [SerializeField]
    private float runSpeed = 10f;

    void Update()
    {
        zMovement = Input.GetAxis( "Vertical" );
        xMovement = Input.GetAxis( "Horizontal" );
    }

    private void FixedUpdate()
    {
        if ( zMovement != 0 || xMovement != 0 )
        {
            Vector3 vertical = zMovement * Camera.main.transform.forward;
            Vector3 horizontal = xMovement * Camera.main.transform.right;

            Vector3 moveDir = (vertical + horizontal).normalized;

            // Problem of inabyssing
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
