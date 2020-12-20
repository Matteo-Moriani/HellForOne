using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Managers;
using Player;
using ArenaSystem;
using System;
using ReincarnationSystem;
using GroupAbilitiesSystem.ScriptableObjects;
using GroupAbilitiesSystem;
using CallToArmsSystem;

public class NewCameraManager : MonoBehaviour
{
    private Transform _currentBoss;
    private Transform _currentLeader;
    private bool _isLocked;
    private float shakingTimer = 0;
    private float shakingTimerTotal;
    private float shakingIntensity;
    private float doubleTargetTimer;
    private float arenaCameraTimer;
    private float callToArmsCameraTimer;
    private bool arenaCameraON = false;
    private Transform callToArmsTarget;

    CinemachineFreeLook _cinemachineFreeLook;
    CinemachineVirtualCamera lockedCamera;
    CinemachineVirtualCamera doubleTargetCamera;
    CinemachineVirtualCamera arenaCamera;
    CinemachineVirtualCamera callToArmsCamera;
    CinemachineBasicMultiChannelPerlin doubleTargetCinemachineBasicMultiChannelPerlin;
    CinemachineBasicMultiChannelPerlin lockedCinemachineBasicMultiChannelPerlin;
    CinemachineTargetGroup targetGroup;



    private void Awake()
    {
        _cinemachineFreeLook = GameObject.FindGameObjectWithTag( "ThirdPersonCamera" ).GetComponent<CinemachineFreeLook>();
        lockedCamera = GameObject.FindGameObjectWithTag( "VirtualCameraLock" ).GetComponent<CinemachineVirtualCamera>();
        doubleTargetCamera = GameObject.FindGameObjectWithTag( "DoubleTargetCamera" ).GetComponent<CinemachineVirtualCamera>();
        doubleTargetCinemachineBasicMultiChannelPerlin = doubleTargetCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        lockedCinemachineBasicMultiChannelPerlin = doubleTargetCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        arenaCamera = GameObject.FindGameObjectWithTag( "ArenaCamera" ).GetComponent<CinemachineVirtualCamera>();
        callToArmsCamera = GameObject.FindGameObjectWithTag( "CallToArmsCamera" ).GetComponent<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
        ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;

        ReincarnationManager.OnLeaderReincarnated += OnLeaderReincarnated;

        CameraShakeRequest.SingleCameraShakeRequest += OnHitReceivedCameraShakeRequest;

        CallToArmsManager.Instance.OnStartCallToArmsImpsSpawn += OnStartCallToArmsImpsSpawn;
    }

    private void OnDisable()
    {
        ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
        ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;

        ReincarnationManager.OnLeaderReincarnated -= OnLeaderReincarnated;

        CameraShakeRequest.SingleCameraShakeRequest -= OnHitReceivedCameraShakeRequest;

        foreach ( GameObject go in GroupSystem.GroupsManager.Instance.Groups.Values )
        {
            go.GetComponentInChildren<GroupAbilities>().OnStartGroupAbility -= OnStartGroupAbility;
        }

        CallToArmsManager.Instance.OnStartCallToArmsImpsSpawn -= OnStartCallToArmsImpsSpawn;
    }

    private void Start()
    {
        lockedCamera.gameObject.SetActive( false );
        doubleTargetCamera.gameObject.SetActive( false );
        arenaCamera.gameObject.SetActive( false );
        callToArmsCamera.gameObject.SetActive( false );
        _cinemachineFreeLook.gameObject.SetActive( true );
        targetGroup = GameObject.FindGameObjectWithTag( "CinemachineTargetGroup" ).GetComponent<CinemachineTargetGroup>();

        foreach ( GameObject go in GroupSystem.GroupsManager.Instance.Groups.Values )
        {
            go.GetComponentInChildren<GroupAbilities>().OnStartGroupAbility += OnStartGroupAbility;
            //go.GetComponentInChildren<GroupAbilitiesSystem.GroupAbilities>().OnStopGroupAbility += OnStopGroupAbility;
        }
    }

    private void OnStartCallToArmsImpsSpawn( Vector3 obj )
    {
        lockedCamera.gameObject.SetActive( false );
        //callToArmsCamera.gameObject.SetActive( true );
        arenaCamera.gameObject.SetActive( true );

        callToArmsCameraTimer = 3f;

        //targetGroup.m_Targets[ 0 ].target = _currentLeader.transform;
        //targetGroup.m_Targets[ 0 ].weight = 1;

        //targetGroup.m_Targets[ 0 ].target = _currentBoss.transform;
        //targetGroup.m_Targets[ 0 ].weight = 1;

        callToArmsTarget = _currentBoss.transform;
        //callToArmsTarget.position = obj;
        targetGroup.m_Targets[ 0 ].target = callToArmsTarget;
        targetGroup.m_Targets[ 0 ].weight = 1;
    }

    private void OnStopGroupAbility()
    {
        /* Controllare che il valore sia lo stesso dello scriptable object dell'abilità, perché significa che
         * il colpo ha missato (fatto per evitare che la camera rimanga in shake)
         */
        //if ( cinemachineBasicMultiChannelPerlin.m_AmplitudeGain == 2f )
        //{
        //    shakingIntensity = 1f;
        //    shakingTimerTotal = 1f;
        //    shakingTimer = 1f;
        //    cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = shakingIntensity;
        //}


    }

    private void OnStartGroupAbility( GroupAbilities groupAbilities , GroupAbility groupAbility )
    {
        if ( groupAbility.GetData().DoCameraShake )
        {
            shakingIntensity = groupAbility.GetData().CameraShakeIntensity;
            //shakingTimerTotal = groupAbility.GetData().CameraShakeDuration;
            //shakingTimer = shakingTimerTotal;
            doubleTargetCinemachineBasicMultiChannelPerlin.m_AmplitudeGain = shakingIntensity;
            lockedCinemachineBasicMultiChannelPerlin.m_AmplitudeGain = shakingIntensity;
        }

        if ( groupAbility.GetData().DoCameraDoubleLookAt )
        {
            lockedCamera.gameObject.SetActive( false );
            doubleTargetCamera.gameObject.SetActive( true );

            doubleTargetTimer = 3f;

            //targetGroup.m_Targets[ 0 ].target = _currentLeader.transform;
            //targetGroup.m_Targets[ 0 ].weight = 1;

            //targetGroup.m_Targets[ 0 ].target = _currentBoss.transform;
            //targetGroup.m_Targets[ 0 ].weight = 1;

            targetGroup.m_Targets[ 0 ].target = groupAbilities.transform.root;
            targetGroup.m_Targets[ 0 ].weight = 1;
        }

        if ( groupAbility.GetData().DoCameraUnzoom )
        {
            lockedCamera.gameObject.SetActive( false );
            doubleTargetCamera.gameObject.SetActive( true );

            doubleTargetTimer = 1.5f;

            targetGroup.m_Targets[ 0 ].target = groupAbilities.transform.root;
            targetGroup.m_Targets[ 0 ].weight = 1;

            arenaCameraON = true;
            arenaCameraTimer = 6f;
        }

    }

    private void OnHitReceivedCameraShakeRequest( float duration , float intensity , bool doCameraShakeOnHit )
    {
        if ( doCameraShakeOnHit )
        {
            shakingIntensity = intensity;
            shakingTimerTotal = duration;
            shakingTimer = duration;
            doubleTargetCinemachineBasicMultiChannelPerlin.m_AmplitudeGain = shakingIntensity;
            lockedCinemachineBasicMultiChannelPerlin.m_AmplitudeGain = shakingIntensity;
        }
    }

    private void OnLeaderReincarnated( ReincarnableBehaviour obj )
    {
        // TODO :- transform.Find is bad
        _currentLeader = obj.transform.Find( "CameraTarget" ).transform;

        _cinemachineFreeLook.Follow = _currentLeader;
        _cinemachineFreeLook.LookAt = _currentLeader;

        lockedCamera.Follow = _currentLeader;

        doubleTargetCamera.Follow = _currentLeader;
        callToArmsCamera.Follow = _currentLeader;

        arenaCamera.Follow = _currentLeader;
    }

    private void OnGlobalStartBattle( ArenaManager arenaManager )
    {
        _currentBoss = arenaManager.Boss.transform;

        lockedCamera.gameObject.SetActive( true );
        _cinemachineFreeLook.gameObject.SetActive( false );

        lockedCamera.LookAt = _currentBoss;

        doubleTargetCamera.LookAt = targetGroup.transform;
        callToArmsCamera.LookAt = targetGroup.transform;

        arenaCamera.LookAt = _currentBoss;

        // _cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = true;
    }

    private void OnGlobalEndBattle( ArenaManager obj )
    {
        lockedCamera.gameObject.SetActive( false );
        _cinemachineFreeLook.gameObject.SetActive( true );

        // _cinemachineVirtualCameraLock.LookAt = _currentLeader;
        // _cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = true;
    }

    private void Update()
    {
        if ( shakingTimer > 0 )
        {
            doubleTargetCinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp( 0f , shakingIntensity , shakingTimer / shakingTimerTotal );
            lockedCinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp( 0f , shakingIntensity , shakingTimer / shakingTimerTotal );

            shakingTimer -= Time.deltaTime;
        }

        if ( callToArmsCameraTimer > 0 )
        {
            callToArmsCameraTimer -= Time.deltaTime;
        }
        else
        {
            if ( arenaCamera.gameObject.activeSelf )
            {
                //callToArmsCamera.gameObject.SetActive( false );
                arenaCamera.gameObject.SetActive( false );
                lockedCamera.gameObject.SetActive( true );
            }           
        }

        if ( doubleTargetTimer > 0 )
        {
            doubleTargetTimer -= Time.deltaTime;
        }
        else
        {
            if ( doubleTargetCamera.gameObject.activeSelf )
            {
                doubleTargetCamera.gameObject.SetActive( false );

                if ( !arenaCameraON )
                {
                    lockedCamera.gameObject.SetActive( true );
                }
            }
            // Transition to ArenaCamera
            if ( !doubleTargetCamera.gameObject.activeSelf )
            {
                if ( arenaCameraON )
                {
                    //doubleTargetCamera.gameObject.SetActive( false );
                    arenaCamera.gameObject.SetActive( true );

                    if ( arenaCameraTimer > 0 )
                    {
                        arenaCameraTimer -= Time.deltaTime;
                    }
                    else
                    {
                        arenaCameraON = false;
                        arenaCamera.gameObject.SetActive( false );
                        lockedCamera.gameObject.SetActive( true );
                    }
                }
            }
        }
    }
}
