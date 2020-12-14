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

    private void OnEnable() => ReincarnationManager.OnLeaderChanged += OnLeaderChanged;

    private void OnDisable() => ReincarnationManager.OnLeaderChanged -= OnLeaderChanged;

    private void OnLeaderChanged(Reincarnation newLeader)
    {
        _cinemachineTargetGroup.m_Targets[ 0 ].target = newLeader.transform;
        _cinemachineTargetGroup.m_Targets[ 1 ].target = transform;   
    }
}
