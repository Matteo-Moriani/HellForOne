using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class NewCameraManager : MonoBehaviour
{
    GameObject target;
    GameObject player;
    private bool isLocked = false;

    GameObject[] enemies;
    GameObject boss;

    CinemachineFreeLook cinemachineFreeLook;
    CinemachineVirtualCamera cinemachineVirtualCameraLock;

    GameObject lockCameraPlayerTarget;

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
        FindPlayer();

        lockCameraPlayerTarget = GameObject.FindGameObjectWithTag( "CameraTarget" );

        cinemachineFreeLook = GameObject.FindGameObjectWithTag( "ThirdPersonCamera" ).GetComponent<CinemachineFreeLook>();
        cinemachineVirtualCameraLock = GameObject.FindGameObjectWithTag( "VirtualCameraLock" ).GetComponent<CinemachineVirtualCamera>();

        cinemachineFreeLook.Follow = player.transform;
        cinemachineFreeLook.LookAt = player.transform;
        cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = true;

        cinemachineVirtualCameraLock.Follow = lockCameraPlayerTarget.transform;

        cinemachineVirtualCameraLock.enabled = false;
    }

    void Update()
    {
        if ( !player )
        {
            FindPlayer();

            cinemachineFreeLook.Follow = player.transform;
            cinemachineFreeLook.LookAt = player.transform;

            cinemachineVirtualCameraLock.Follow = lockCameraPlayerTarget.transform;

            cinemachineVirtualCameraLock.enabled = false;
            cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = true;
            cinemachineFreeLook.enabled = true;

            isLocked = false;
        }

        // Remove lock-on
        // if ( Input.GetButtonDown( "R3" ) && isLocked )
        if ( InputManager.Instance.RightStickButtonDown() && isLocked )
        {
            cinemachineVirtualCameraLock.enabled = false;
            cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = true;
            cinemachineFreeLook.enabled = true;
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

            cinemachineVirtualCameraLock.enabled = true;
            cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = false;
            cinemachineFreeLook.enabled = false;

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

            // LockCameraTraget gameobject is always the last child
            cinemachineVirtualCameraLock.LookAt = target.transform;
        }
    }
}
