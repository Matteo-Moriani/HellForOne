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

    CinemachineFreeLook _cinemachineFreeLook;
    CinemachineVirtualCamera _cinemachineVirtualCameraLock;
    
    private void Awake()
    {
        _cinemachineFreeLook = GameObject.FindGameObjectWithTag( "ThirdPersonCamera" ).GetComponent<CinemachineFreeLook>();
        _cinemachineVirtualCameraLock = GameObject.FindGameObjectWithTag( "VirtualCameraLock" ).GetComponent<CinemachineVirtualCamera>();
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
    }

    private void OnDisable()
    {
        ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
        ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;

        ReincarnationManager.OnLeaderChanged -= OnLeaderChanged;
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
}
