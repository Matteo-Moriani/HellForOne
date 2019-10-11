using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    public Rigidbody rb;
    public float zMovement, xMovement, resultingMovement;
    public Vector3 newPosition;
    public Camera camera;
    public GameObject mainCamera;

    public float yRotation;
    public float xRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = GameObject.FindGameObjectWithTag( "MainCamera" );
        camera = mainCamera.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1

        // Horizontal2 non funge bene
        yRotation = Input.GetAxis( "Horizontal2" );
        xRotation = Input.GetAxis( "Vertical2" );

        zMovement = Input.GetAxis( "Vertical" );
        xMovement = Input.GetAxis( "Horizontal" );

        if ( zMovement != 0f || xMovement != 0f )
        {
            // Make it move 10 meters per second instead of 10 meters per frame...
            zMovement *= Time.deltaTime;
            xMovement *= Time.deltaTime;

            Vector3 newZPosition = new Vector3( 0, 0, zMovement );
            Vector3 newXPosition = new Vector3( xMovement, 0, 0 );

            Vector3 resultingNewPosition = newZPosition + newXPosition;
            resultingNewPosition *= 10;

            //Quaternion orientation = new Quaternion( 0, xMovement, 0, 0 );
            //transform.rotation = orientation;

            //newPosition = new Vector3(  - transform.position.x, 0, zMovement - transform.position.z );

            transform.rotation = Quaternion.LookRotation( resultingNewPosition );
            transform.position += new Vector3( newXPosition.x, 0, newZPosition.z ).normalized * 10 * Time.deltaTime;

            mainCamera.transform.position += new Vector3( newXPosition.x, 0, newZPosition.z ).normalized * 10 * Time.deltaTime;

            // Move translation along the object's z-axis
            //transform.Translate( 0, 0, zMovement );


            // Rotate around our y-axis
            //We don't want to rotate, we want a sharp turn and then he goes that way
            //transform.Rotate( 0, xMovement, 0 );
        }


    }

    private void FixedUpdate()
    {
        //transform.SetPositionAndRotation( newPosition, Quaternion.identity );
        //transform.Translate( 0, 0, resultingMovement );
        //rb.AddForce(0, 0, translation);
    }
}
