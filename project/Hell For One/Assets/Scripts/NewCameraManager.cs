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

        foreach ( GameObject go in GroupSystem.GroupsManager.Instance.Groups.Values )
        {
            go.GetComponentInChildren<GroupAbilitiesSystem.GroupAbilities>().OnStartGroupAbility += OnStartGroupAbility;
            //go.GetComponentInChildren<GroupAbilitiesSystem.GroupAbilities>().OnStopGroupAbility += OnStopGroupAbility;
        }
    }

    private void OnStopGroupAbility()
    {
        /* Controllare che il valore sia lo stesso dello scriptable object dell'abilità, perché significa che
         * il colpo ha missato (fatto per evitare che la camera rimanga in shake)
         */
        if ( cinemachineBasicMultiChannelPerlin.m_AmplitudeGain == 2f )
        {
            shakingIntensity = 1f;
            shakingTimerTotal = 1f;
            shakingTimer = 1f;
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = shakingIntensity;
        }
    }

    private void OnStartGroupAbility( GroupAbility groupAbility )
    {
        shakingIntensity = groupAbility.GetData().CameraShakeIntensity;
        shakingTimerTotal = groupAbility.GetData().CameraShakeDuration;
        shakingTimer = shakingTimerTotal;
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = shakingIntensity;
    }

    

    private void OnEnable()
    {
        ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
        ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        
        ReincarnationManager.OnLeaderReincarnated += OnLeaderReincarnated;

        ShakeOnHit.OnHitReceivedCameraShakeRequest += OnHitReceivedCameraShakeRequest;
    }

    private void OnDisable()
    {
        ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
        ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;

        ReincarnationManager.OnLeaderReincarnated -= OnLeaderReincarnated;

        ShakeOnHit.OnHitReceivedCameraShakeRequest -= OnHitReceivedCameraShakeRequest;

        foreach ( GameObject go in GroupSystem.GroupsManager.Instance.Groups.Values )
        {
            go.GetComponentInChildren<GroupAbilitiesSystem.GroupAbilities>().OnStartGroupAbility -= OnStartGroupAbility;
        }
    }

    private void OnHitReceivedCameraShakeRequest( float duration , float intensity )
    {
        shakingIntensity = intensity;
        shakingTimerTotal = duration;
        shakingTimer = shakingTimerTotal;
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = shakingIntensity;
    }

    private void OnLeaderReincarnated(ReincarnableBehaviour obj)
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
