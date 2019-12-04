﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Can be an enemy or the player
    public GameObject target;
    public GameObject player;

    [SerializeField]
    private float turnSpeed = 4.0f;

    private Vector3 offset;
    private Vector3 closedEnvironmentOffset;

    // Use this to set the camera in closed environment mode and viceversa
    public bool closedEnvironment { get; set; } = false;

    [SerializeField]
    private float distance;

    [SerializeField]
    private bool isLocked = false;

    public GameObject[] enemies;
    public GameObject boss;

    // To avoid takin too much inputs
    [SerializeField]
    private bool rightAxisInUse = false;

    private float mouseSensitivity = 0.1f;

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag( "Player" );
        target = player;
    }

    public static GameObject FindNearestEnemy( GameObject objectFrom, GameObject[] gameObjects )
    {
        float minDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach ( GameObject enemy in gameObjects )
        {
            if ( (enemy.transform.position - objectFrom.transform.position).magnitude < minDistance )
            {
                minDistance = (enemy.transform.position - objectFrom.transform.position).magnitude;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    void Start()
    {
        offset = new Vector3( 0.0f, 5.0f, -10.0f );
        closedEnvironmentOffset = offset / 2;
        FindPlayer();
        target = player;
    }

    private void Update()
    {
        if ( closedEnvironment )
        {
            //transform.position = player.transform.position;
            turnSpeed = 0.0f;
        }
        else
            turnSpeed = 4.0f;

        if ( !player )
        {
            isLocked = false;
            FindPlayer();
        }

        // M&K
        if (GameObject.FindGameObjectWithTag("Managers").GetComponentInChildren<InputManager>().Type == InputManager.Controller.MouseAndKeyboard )
        {
            offset = Quaternion.AngleAxis( - Input.GetAxis( "Mouse X" ) * mouseSensitivity * turnSpeed, Vector3.down ) * offset;

            if ( closedEnvironment )
            {
                transform.position = player.transform.position + closedEnvironmentOffset;
                transform.LookAt( player.transform.position );
            }
            else
                transform.position = player.transform.position + offset;

            transform.LookAt( target.transform.position );

        }

        // PS & Xbox
        else
        {
            // Remove lock-on
            if ( Input.GetButtonDown( "R3" ) && isLocked )
            {
                isLocked = false;
                target = player;
            }

            // Start lock-on
            else if ( Input.GetButtonDown( "R3" ) && !isLocked )
            {
                enemies = GameObject.FindGameObjectsWithTag( "LittleEnemy" );
                boss = GameObject.FindGameObjectWithTag( "Boss" );

                if ( boss == null && enemies != null )
                {
                    isLocked = true;
                    target = FindNearestEnemy( gameObject, enemies );
                }
                else if ( boss != null )
                {
                    isLocked = true;
                    target = boss;
                }
            }

            if ( isLocked )
            {
                Vector3 direction = (player.transform.position - target.transform.position).normalized;

                Vector3 camera_offset = player.transform.position + direction * Mathf.Abs( offset.z );

                camera_offset.y = offset.y;

                transform.position = camera_offset;
            }
            else
            {
                offset = Quaternion.AngleAxis( Input.GetAxis( "Vertical2" ) * turnSpeed, Vector3.down ) * offset;

                if ( closedEnvironment )
                {
                    transform.position = player.transform.position + closedEnvironmentOffset;
                    transform.LookAt( player.transform.position );
                }
                else
                    transform.position = player.transform.position + offset;
            }

            transform.LookAt( target.transform.position );

            // Change lock-on target
            float input = Input.GetAxis( "Vertical2" );
            if ( (!rightAxisInUse && Mathf.Abs( input ) > 0.4f) && isLocked )
            {
                rightAxisInUse = true;

                //Do nothing
                if ( !boss && target == boss ) ;

                // Cycle through the enemies
                else
                {
                    float minLeftDistance = -1 * Mathf.Infinity;
                    float minRightDistance = Mathf.Infinity;
                    GameObject leftNearestDemon = null;
                    GameObject rightNearestDemon = null;

                    foreach ( GameObject demon in enemies )
                    {
                        float demonXAxis = transform.InverseTransformPoint( demon.transform.position ).x;

                        Debug.Log( demonXAxis );

                        if ( demonXAxis > 0.01f && demonXAxis < minRightDistance )
                        {
                            minRightDistance = demonXAxis;
                            rightNearestDemon = demon;
                        }

                        if ( demonXAxis < -0.01f && demonXAxis > minLeftDistance )
                        {
                            minLeftDistance = demonXAxis;
                            leftNearestDemon = demon;
                        }
                    }

                    // Don't know why it is inverted
                    if ( input < 0 && rightNearestDemon )
                        target = rightNearestDemon;
                    else if ( input > 0 && leftNearestDemon )
                        target = leftNearestDemon;
                }
            }

            else if ( Mathf.Abs( input ) <= 0.4f )
                rightAxisInUse = false;
        }
    }

    void LateUpdate()
    {
        if ( target == null )
        {
            FindPlayer();
        }

        // If target enemy dies
        if ( target != player && target.GetComponent<Stats>().health <= 0 )
        {
            target = player;
            isLocked = false;
        }
    }
}