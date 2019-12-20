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
    GameObjectSearcher gameObjectSearcher;

    GameObject lockCameraPlayerTarget;

    public bool IsLocked { get => isLocked; set => isLocked = value; }

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag( "Player" );
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

        gameObjectSearcher = GameObject.FindGameObjectWithTag( "ChildrenSearcher" ).GetComponent<GameObjectSearcher>();
        gameObjectSearcher.GetChildObject( player.transform, "CameraTarget" );
        lockCameraPlayerTarget = gameObjectSearcher.GetFirstChildWithTag();

        cinemachineFreeLook = GameObject.FindGameObjectWithTag( "ThirdPersonCamera" ).GetComponent<CinemachineFreeLook>();
        cinemachineVirtualCameraLock = GameObject.FindGameObjectWithTag( "VirtualCameraLock" ).GetComponent<CinemachineVirtualCamera>();

        cinemachineFreeLook.Follow = player.transform;
        cinemachineFreeLook.LookAt = player.transform;

        cinemachineVirtualCameraLock.Follow = lockCameraPlayerTarget.transform;

        cinemachineVirtualCameraLock.gameObject.SetActive( false );
        //cinemachineVirtualCameraLock.enabled = false;
    }

    void Update()
    {
        if ( !player )
        {
            FindPlayer();

            cinemachineFreeLook.Follow = player.transform;
            cinemachineFreeLook.LookAt = player.transform;

            cinemachineVirtualCameraLock.Follow = lockCameraPlayerTarget.transform;

            cinemachineVirtualCameraLock.gameObject.SetActive( false );
            cinemachineFreeLook.gameObject.SetActive(true);

            //cinemachineVirtualCameraLock.enabled = false;
            //cinemachineFreeLook.enabled = true;

            IsLocked = false;
        }

        // If target dies
        if (IsLocked && !target )
        {
            cinemachineVirtualCameraLock.gameObject.SetActive( false );
            //cinemachineVirtualCameraLock.enabled = false;
            //cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = true;
            cinemachineFreeLook.gameObject.SetActive( true );
            //cinemachineFreeLook.enabled = true;
            IsLocked = false;
        }

        // Remove lock-on
        // if ( Input.GetButtonDown( "R3" ) && isLocked )
        if ( InputManager.Instance.RightStickButtonDown() && IsLocked )
        {
            cinemachineVirtualCameraLock.gameObject.SetActive( false );
            //cinemachineVirtualCameraLock.enabled = false;
            //cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = true;
            cinemachineFreeLook.gameObject.SetActive( true );
            //cinemachineFreeLook.enabled = true;
            IsLocked = false;
        }

        // Start lock-on
        //else if ( Input.GetButtonDown( "R3" ) && !isLocked )
        else if ( InputManager.Instance.RightStickButtonDown() && !IsLocked )
        {
            if ( EnemiesManager.Instance.Boss == null && EnemiesManager.Instance.LittleEnemiesList != null )
            {
                target = FindNearestEnemy( gameObject, EnemiesManager.Instance.littleEnemiesList.ToArray() );
                if ( target )
                {
                    cinemachineVirtualCameraLock.gameObject.SetActive( true );
                    //cinemachineVirtualCameraLock.enabled = true;
                    cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = false;
                    cinemachineFreeLook.gameObject.SetActive( false );
                    //cinemachineFreeLook.enabled = false;
                    cinemachineVirtualCameraLock.LookAt = target.transform;
                    cinemachineVirtualCameraLock.Follow = lockCameraPlayerTarget.transform;
                    IsLocked = true;
                }
            }
            //else if ( boss != null )
            else if ( EnemiesManager.Instance.Boss != null )
            {
                target = EnemiesManager.Instance.Boss;
                if ( target )
                {
                    cinemachineVirtualCameraLock.gameObject.SetActive( true );
                    //cinemachineVirtualCameraLock.enabled = true;
                    cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = false;
                    cinemachineFreeLook.gameObject.SetActive( false );
                    //cinemachineFreeLook.enabled = false;
                    cinemachineVirtualCameraLock.LookAt = target.transform;
                    IsLocked = true;
                }
            }
        }
    }
}
