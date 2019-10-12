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
    public Vector3 newZPosition, newXPosition, resultingNewPosition;
    public Camera camera;
    public GameObject mainCamera;
    public cameraManager cameraManager;

    public float yRotation;
    public float xRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = GameObject.FindGameObjectWithTag( "MainCamera" );
        camera = mainCamera.GetComponent<Camera>();
        cameraManager = mainCamera.GetComponent<cameraManager>();
        newZPosition = new Vector3();
        newXPosition = new Vector3();
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

            // These 2 Vectors can be comprimed in 1 with the 2 components
            newZPosition.Set( 0, 0, zMovement );
            newXPosition.Set( xMovement, 0, 0 );

            resultingNewPosition = (newZPosition + newXPosition) * 10;

            // TransformDirection transform the vector using local space into a vector using world's space
            resultingNewPosition = Camera.main.transform.TransformDirection( resultingNewPosition );
            resultingNewPosition.y = 0.0f;

            transform.rotation = Quaternion.LookRotation( resultingNewPosition );

            transform.position += resultingNewPosition.normalized * 10 * Time.deltaTime;

            mainCamera.transform.position += new Vector3( newXPosition.x, 0, newZPosition.z ).normalized * 10 * Time.deltaTime;
        }


    }

    private void FixedUpdate()
    {
    }
}
