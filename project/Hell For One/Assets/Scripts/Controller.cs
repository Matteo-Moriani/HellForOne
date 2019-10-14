using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    //TODO Organize attributes with [Header("Inputs")], [Header("Stats")] ...

    public float speed = 10.0f;
    //public float rotationSpeed = 100.0f;
    public Rigidbody rb;
    public float zMovement, xMovement, oldZMovement, oldXMovement, resultingMovement;
    public Vector3 newZPosition, newXPosition, resultingNewPosition, resultingNewPositionStillMoving;
    public Camera camera;
    public GameObject mainCamera;
    public cameraManager cameraManager;
    public Vector3 mainCameraForward;
    public float moveAmount, moveDir;
    public float rotateSpeed = 5f;

    public float yRotation;
    public float xRotation;

    // Used to set the movement direction when the player stops
    public Vector3 cameraForward;

    // Indicative one
    private float cameraDistanceFromPlayer = 40f;

    // Used to move in the same direction even if the camera rotates (if the movement is continuos)
    //public bool stillMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = GameObject.FindGameObjectWithTag( "MainCamera" );
        camera = mainCamera.GetComponent<Camera>();
        cameraManager = mainCamera.GetComponent<cameraManager>();
        newZPosition = new Vector3();
        newXPosition = new Vector3();
        resultingNewPosition = new Vector3();
        cameraForward = Camera.main.transform.forward;
    }

    // TODO add a way to keep moving in the same direction even if the camera rotates (only if left stick input is continuos),
    // but when input stops, the next movement is referred to the camera
    // Update is called once per frame
    void Update()
    {
        mainCameraForward = mainCamera.transform.forward;
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1

        // Horizontal2 non funge bene
        yRotation = Input.GetAxis( "Horizontal2" );
        xRotation = Input.GetAxis( "Vertical2" );

        zMovement = Input.GetAxis( "Vertical" );
        xMovement = Input.GetAxis( "Horizontal" );



        if ( (zMovement != 0f || xMovement != 0f) )
        {
            //// Make it move 10 meters per second instead of 10 meters per frame...
            //zMovement *= Time.deltaTime;
            //xMovement *= Time.deltaTime;

            //// These 2 Vectors can be comprimed in 1 with the 2 components
            //newZPosition.Set( 0, 0, zMovement );
            //newXPosition.Set( xMovement, 0, 0 );

            //resultingNewPosition = (newZPosition + newXPosition) * 10;

            //transform.rotation = Quaternion.LookRotation( resultingNewPosition );

            //transform.position += resultingNewPosition.normalized * 10 * Time.deltaTime;

            //transform.position += resultingNewPositionStillMoving.normalized * 10 * Time.deltaTime;

            //ORIGINAL 

            //transform.rotation = Quaternion.LookRotation( resultingNewPosition );

            //Vector3 cameraOrientation = Camera.main.transform.rotation.eulerAngles;
            //cameraOrientation.x = 0;
            //cameraOrientation.z = 0;
            //transform.rotation = Quaternion.Euler( cameraOrientation );

            //var CharacterRotation = Camera.main.transform.rotation;
            //CharacterRotation.x = 0;
            //CharacterRotation.z = 0;

            //transform.rotation = CharacterRotation;

            //transform.position += resultingNewPosition.normalized * (10 * Time.deltaTime);

            Vector3 vertical = zMovement * Camera.main.transform.forward;
            Vector3 horizontal = xMovement * Camera.main.transform.right;
            Vector3 moveDir = (vertical + horizontal).normalized;
            moveDir.y = 0f;
            float m = Mathf.Abs( zMovement ) + Mathf.Abs( xMovement );
            moveAmount = Mathf.Clamp01( m );
            transform.position += moveDir.normalized * 10 * Time.deltaTime;

            Vector3 targetDir = moveDir;
            targetDir.y = 0f;
            if ( targetDir == Vector3.zero )
                targetDir = Vector3.forward;
            Quaternion tr = Quaternion.LookRotation( targetDir );
            Quaternion targetRotation = Quaternion.Slerp( transform.rotation, tr, moveAmount * rotateSpeed );
            transform.rotation = targetRotation;



            //resultingNewPosition = new Vector3();
            //resultingNewPosition.z = mainCamera.transform.forward.z * zMovement;
            //resultingNewPosition.y = 0f;
            //resultingNewPosition.x = mainCamera.transform.right.x * xMovement;


            //transform.position += resultingNewPosition * (10 * Time.deltaTime);

            //transform.Translate( resultingNewPosition * 10 * Time.deltaTime, Space.Self );

            //TRY WITH PLAYER ORIENTATION (THINK I NEED TO USE Transform.InverseTransformDirection())
            //resultingNewPosition = new Vector3();
            //resultingNewPosition.z = transform.forward.z * zMovement;
            //resultingNewPosition.y = 0f;
            //resultingNewPosition.x = transform.right.x * xMovement;

            //resultingNewPosition = new Vector3();
            //resultingNewPosition.z = mainCamera.transform.forward.z * zMovement;
            //resultingNewPosition.y = 0f;
            //resultingNewPosition.x = mainCamera.transform.right.x * xMovement;

            //if ( zMovement != oldZMovement || xMovement != oldXMovement )
            //{
            //    transform.rotation *= Quaternion.Euler( 0, (xMovement - oldXMovement) * 90, 0 );
            //}

            //// TODO Here there is 1 of the problems
            ////transform.rotation = Quaternion.LookRotation( resultingNewPosition );
            //transform.position += transform.forward * 10 * Time.deltaTime;

            //oldZMovement = zMovement;
            //oldXMovement = xMovement;
        }

        else if ( (zMovement != 0f || xMovement != 0f) )
        {

            //// Make it move 10 meters per second instead of 10 meters per frame...
            //zMovement *= Time.deltaTime;
            //xMovement *= Time.deltaTime;

            //// These 2 Vectors can be comprimed in 1 with the 2 components
            //newZPosition.Set( 0, 0, zMovement );
            //newXPosition.Set( xMovement, 0, 0 );

            //resultingNewPosition = (newZPosition + newXPosition) * 10;

            //resultingNewPositionStillMoving = resultingNewPosition;

            // TransformDirection transform the vector using local space into a vector using world's space
            //resultingNewPosition = Camera.main.transform.TransformDirection( resultingNewPosition );
            //resultingNewPosition.y = 0.0f;

            // Need to turn and walk in the direction of the camera
            // ORIGINAL
            //resultingNewPosition = new Vector3();
            //resultingNewPosition.z += Camera.main.transform.forward.z * zMovement * Time.deltaTime;
            //resultingNewPosition.y = 0f;
            //resultingNewPosition.x += Camera.main.transform.right.x * xMovement * Time.deltaTime;

            //transform.rotation = Quaternion.LookRotation( resultingNewPosition );

            //transform.position += resultingNewPosition.normalized * 10 * Time.deltaTime;

            // TRY
            //Vector3 cameraOrientation = Camera.main.transform.rotation.eulerAngles;
            //cameraOrientation.x = 0;
            //cameraOrientation.z = 0;
            //transform.rotation = Quaternion.Euler( cameraOrientation );

            //transform.rotation.eulerAngles.y = Camera.main.transform.rotation.eulerAngles.y;

            //resultingNewPosition = new Vector3();
            //resultingNewPosition.z = transform.forward.z * zMovement * Time.deltaTime;
            //resultingNewPosition.y = 0f;
            //resultingNewPosition.x = transform.right.x * xMovement * Time.deltaTime;

            //transform.position += resultingNewPosition.normalized * 10 * Time.deltaTime;

            //cameraOrientation.z = Camera.main.transform.forward.z;
            //transform.rotation = Quaternion.LookRotation( cameraOrientation );
            //cameraOrientation.x = Camera.main.transform.right.x;
            //transform.rotation = Quaternion.LookRotation( cameraOrientation );

            //resultingNewPosition = new Vector3();
            //resultingNewPosition.z += Camera.main.transform.forward.z * cameraDistanceFromPlayer * zMovement * Time.deltaTime;
            //resultingNewPosition.y = 0f;
            //resultingNewPosition.x += Camera.main.transform.right.x * xMovement * Time.deltaTime;

            //transform.rotation = Quaternion.LookRotation( resultingNewPosition );

            //transform.position += resultingNewPosition.normalized * 10 * Time.deltaTime;

            oldZMovement = zMovement;
            oldXMovement = xMovement;

            //stillMoving = true;
        }

        else if ( zMovement == 0f && xMovement == 0f )
        {
            //stillMoving = false;
            oldZMovement = 0f;
            oldXMovement = 0f;
        }
    }

    private void FixedUpdate()
    {
    }
}
