﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraManager : MonoBehaviour
{
    public GameObject target;
    public GameObject player;
    private bool isLocked = false;

    public GameObject[] enemies;
    public GameObject boss;

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
        
    }

    void Update()
    {
        // Remove lock-on
        // if ( Input.GetButtonDown( "R3" ) && isLocked )
        if ( InputManager.Instance.RightStickButtonDown() && isLocked )
        {
            isLocked = false;
            target = player;
        }

        // Start lock-on
        //else if ( Input.GetButtonDown( "R3" ) && !isLocked )
        else if ( InputManager.Instance.RightStickButtonDown() && !isLocked )
        {
            //enemies = GameObject.FindGameObjectsWithTag( "LittleEnemy" );
            //boss = GameObject.FindGameObjectWithTag( "Boss" );

            //if ( boss == null && enemies != null )
            if ( EnemiesManager.Instance.Boss == null && EnemiesManager.Instance.LittleEnemiesList != null )
            {
                target = FindNearestEnemy( gameObject, EnemiesManager.Instance.littleEnemiesList.ToArray() );
                if ( target )
                    isLocked = true;
            }
            //else if ( boss != null )
            else if ( EnemiesManager.Instance.Boss != null )
            {
                target = EnemiesManager.Instance.Boss;
                if ( target )
                    isLocked = true;
            }
        }

        if ( isLocked )
        {
            //Vector3 direction = (player.transform.position - target.transform.position).normalized;

            //Vector3 camera_offset = player.transform.position + direction * Mathf.Abs( lockedOffset.z );

            //camera_offset.y = lockedOffset.y;

            //transform.position = camera_offset;
        }
        else
        {
            ////offset = Quaternion.AngleAxis( Input.GetAxis( "Vertical2" ) * turnSpeed, Vector3.down ) * offset;
            //offset = Quaternion.AngleAxis( InputManager.Instance.RightStickHorizontal() * turnSpeed, Vector3.down ) * offset;
            //if ( closedEnvironment )
            //{
            //    transform.position = player.transform.position + closedEnvironmentOffset;
            //    transform.LookAt( player.transform.position );
            //}
            //else
            //    transform.position = player.transform.position + offset;
        }

        //transform.LookAt( target.transform.position );

        //// Change lock-on target
        ////float input = Input.GetAxis( "Vertical2" );
        //float input = InputManager.Instance.RightStickHorizontal();

        //if ( (!rightAxisInUse && Mathf.Abs( input ) > 0.4f) && isLocked )
        //{
        //    rightAxisInUse = true;

        //    //Do nothing
        //    if ( !EnemiesManager.Instance.Boss && target == EnemiesManager.Instance.Boss ) ;

        //    // Cycle through the enemies
        //    else
        //    {
        //        float minLeftDistance = -1 * Mathf.Infinity;
        //        float minRightDistance = Mathf.Infinity;
        //        GameObject leftNearestDemon = null;
        //        GameObject rightNearestDemon = null;

        //        foreach ( GameObject demon in EnemiesManager.Instance.littleEnemiesList )
        //        {
        //            float demonXAxis = transform.InverseTransformPoint( demon.transform.position ).x;

        //            Debug.Log( demonXAxis );

        //            if ( demonXAxis > 0.01f && demonXAxis < minRightDistance )
        //            {
        //                minRightDistance = demonXAxis;
        //                rightNearestDemon = demon;
        //            }

        //            if ( demonXAxis < -0.01f && demonXAxis > minLeftDistance )
        //            {
        //                minLeftDistance = demonXAxis;
        //                leftNearestDemon = demon;
        //            }
        //        }

        //        // Don't know why it is inverted
        //        if ( input < 0 && rightNearestDemon )
        //            target = rightNearestDemon;
        //        else if ( input > 0 && leftNearestDemon )
        //            target = leftNearestDemon;
        //    }
        //}

        //else if ( Mathf.Abs( input ) <= 0.4f )
        //    rightAxisInUse = false;
    }
}