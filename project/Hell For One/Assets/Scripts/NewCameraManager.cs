using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Managers;
using Player;
using ArenaSystem;
using System;
using ReincarnationSystem;

public class NewCameraManager : MonoBehaviour
{
    private Transform _currentBoss;
    private Transform _currentLeader;
    private bool _isLocked;
    private float shakingTimer;
    private float shakingTimerTotal;
    private float shakingIntensity;

    CinemachineFreeLook _cinemachineFreeLook;
    CinemachineVirtualCamera _cinemachineVirtualCameraLock;
    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;


    private void Awake()
    {
        _cinemachineFreeLook = GameObject.FindGameObjectWithTag( "ThirdPersonCamera" ).GetComponent<CinemachineFreeLook>();
        _cinemachineVirtualCameraLock = GameObject.FindGameObjectWithTag( "VirtualCameraLock" ).GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = _cinemachineVirtualCameraLock.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start()
    {
        _cinemachineVirtualCameraLock.gameObject.SetActive( false );
        _cinemachineFreeLook.gameObject.SetActive( true );
    }

    private void OnEnable()
    {
        ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
        ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        
        ReincarnationManager.OnLeaderChanged += OnLeaderChanged;

        ArenaManager.OnGlobalStartBattle += OnHammerHit;
    }

    private void OnDisable()
    {
        ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
        ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;

        ReincarnationManager.OnLeaderChanged -= OnLeaderChanged;

        ArenaManager.OnGlobalStartBattle -= OnHammerHit;
    }

    private void OnHammerHit( ArenaManager arenaManager )
    {
        shakingTimer = 2f;
        shakingTimerTotal = 2f;
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5f;
        shakingIntensity = 5f;
    }

    private void OnLeaderChanged(Reincarnation obj)
    {
        // TODO :- transform.Find is bad
        _currentLeader = obj.transform.Find("CameraTarget").transform;
        
        _cinemachineFreeLook.Follow = _currentLeader;
        _cinemachineFreeLook.LookAt = _currentLeader;
        _cinemachineVirtualCameraLock.Follow = _currentLeader;
    }

    private void OnGlobalStartBattle( ArenaManager arenaManager )
    {
        _currentBoss = arenaManager.Boss.transform;
        
        _cinemachineVirtualCameraLock.gameObject.SetActive(true);
        _cinemachineFreeLook.gameObject.SetActive(false);
        
        _cinemachineVirtualCameraLock.LookAt = _currentBoss;

        // _cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = true;
    }

    private void OnGlobalEndBattle( ArenaManager obj )
    {
        _cinemachineVirtualCameraLock.gameObject.SetActive(false);
        _cinemachineFreeLook.gameObject.SetActive(true);
        
        // _cinemachineVirtualCameraLock.LookAt = _currentLeader;
        // _cinemachineFreeLook.m_RecenterToTargetHeading.m_enabled = true;
    }

    private void Update()
    {
        if ( shakingTimer > 0 )
        {
            shakingTimer -= Time.deltaTime;

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp( 0f , shakingIntensity , shakingTimer / shakingTimerTotal );
        }
    }
}
