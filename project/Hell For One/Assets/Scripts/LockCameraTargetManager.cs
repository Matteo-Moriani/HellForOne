using UnityEngine;
using Cinemachine;
using ReincarnationSystem;

public class LockCameraTargetManager : MonoBehaviour
{
    private CinemachineTargetGroup _cinemachineTargetGroup;

    private void Awake()
    {
        _cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void OnEnable() => ReincarnationManager.OnLeaderReincarnated += OnLeaderChanged;

    private void OnDisable() => ReincarnationManager.OnLeaderReincarnated -= OnLeaderChanged;

    private void OnLeaderChanged(ReincarnableBehaviour newLeader)
    {
        _cinemachineTargetGroup.m_Targets[ 0 ].target = newLeader.transform;
        _cinemachineTargetGroup.m_Targets[ 1 ].target = transform;   
    }
}
