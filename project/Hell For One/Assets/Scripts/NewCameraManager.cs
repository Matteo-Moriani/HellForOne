using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class NewCameraManager : MonoBehaviour
{
    GameObject target;
    private GameObject player;
    private bool isLocked = false;

    GameObject[] enemies;
    GameObject boss;

    CinemachineFreeLook cinemachineFreeLook;
    CinemachineVirtualCamera cinemachineVirtualCameraLock;
    GameObjectSearcher gameObjectSearcher;

    GameObject lockCameraPlayerTarget;

    public bool IsLocked { get => isLocked; set => isLocked = value; }
    public GameObject Player { get => player; set => player = value; }

    private void FindPlayer()
    {
        Player = GameObject.FindGameObjectWithTag( "Player" );
        gameObjectSearcher.GetChildObject( Player.transform, "CameraTarget" );
        lockCameraPlayerTarget = gameObjectSearcher.GetFirstChildWithTag();
    }

    public void PlayerReincarnated()
    {
        IsLocked = false;

        if ( cinemachineVirtualCameraLock.enabled )
        {
            FindPlayer();

            if ( lockCameraPlayerTarget )
            {
                cinemachineVirtualCameraLock.GetComponent<CinemachineVirtualCamera>().Follow = lockCameraPlayerTarget.transform;
            }

            cinemachineVirtualCameraLock.gameObject.SetActive( false );
        }

        cinemachineFreeLook.gameObject.SetActive( true );
        cinemachineFreeLook.Follow = Player.transform;
        cinemachineFreeLook.LookAt = Player.transform;
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
        gameObjectSearcher = GameObject.FindGameObjectWithTag( "ChildrenSearcher" ).GetComponent<GameObjectSearcher>();

        FindPlayer();

        cinemachineFreeLook = GameObject.FindGameObjectWithTag( "ThirdPersonCamera" ).GetComponent<CinemachineFreeLook>();
        cinemachineVirtualCameraLock = GameObject.FindGameObjectWithTag( "VirtualCameraLock" ).GetComponent<CinemachineVirtualCamera>();

        cinemachineFreeLook.Follow = Player.transform;
        cinemachineFreeLook.LookAt = Player.transform;

        cinemachineVirtualCameraLock.Follow = lockCameraPlayerTarget.transform;

        cinemachineVirtualCameraLock.gameObject.SetActive( false );
        //cinemachineVirtualCameraLock.enabled = false;
    }

    void Update()
    {
        if ( !Player )
        {
            FindPlayer();

            cinemachineFreeLook.Follow = Player.transform;
            cinemachineFreeLook.LookAt = Player.transform;

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
                    FindPlayer();
                    IsLocked = true;
                    cinemachineVirtualCameraLock.gameObject.SetActive( true );
                    //cinemachineVirtualCameraLock.enabled = true;
                    cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = false;
                    cinemachineFreeLook.gameObject.SetActive( false );
                    //cinemachineFreeLook.enabled = false;
                    cinemachineVirtualCameraLock.LookAt = target.transform;
                    cinemachineVirtualCameraLock.Follow = lockCameraPlayerTarget.transform;
                }
            }
            //else if ( boss != null )
            else if ( EnemiesManager.Instance.Boss != null )
            {
                target = EnemiesManager.Instance.Boss;
                if ( target )
                {
                    FindPlayer();
                    IsLocked = true;
                    cinemachineVirtualCameraLock.gameObject.SetActive( true );
                    //cinemachineVirtualCameraLock.enabled = true;
                    cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = false;
                    cinemachineFreeLook.gameObject.SetActive( false );
                    //cinemachineFreeLook.enabled = false;
                    cinemachineVirtualCameraLock.LookAt = target.transform;
                    cinemachineVirtualCameraLock.Follow = lockCameraPlayerTarget.transform;
                }
            }
        }
    }
}
