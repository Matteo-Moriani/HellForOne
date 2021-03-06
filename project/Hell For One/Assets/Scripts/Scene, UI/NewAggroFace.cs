﻿using ArenaSystem;
using AI.Boss;
using FactoryBasedCombatSystem;
using UnityEngine;

/// <summary>
/// Must be added to the real aggro face in the scene so it can move that aggro face to the aggroed imp 
/// </summary>

public class NewAggroFace : MonoBehaviour
{
    private Vector3 cameraPosition;
    private Vector3 rotateDirection;
    private Quaternion lookRotation;
    private Transform _impTargetedTransform;
    private Vector3 aggroFacePosition;

    private void OnEnable()
    {
        BossAi.OnBossTargetChanged += OnBossTargetChanged;
        
        ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;

        ImpDeath.OnImpDeath += OnImpDeath;
    }

    private void OnDisable()
    {
        BossAi.OnBossTargetChanged -= OnBossTargetChanged;
        
        ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;

        ImpDeath.OnImpDeath -= OnImpDeath;
    }

    void Update()
    {
        aggroFacePosition = _impTargetedTransform != null
            ? _impTargetedTransform.position + new Vector3(0f, 1.825f, 0f)
            : new Vector3(0f, -100f, 0f);
        
        transform.position = aggroFacePosition;
        
        cameraPosition = Camera.main.transform.position;
        rotateDirection = (cameraPosition - transform.position).normalized;
        lookRotation = Quaternion.LookRotation( rotateDirection );
        transform.rotation = Quaternion.Slerp( transform.rotation , lookRotation , 1 );
    }

    private void OnImpDeath(Transform deadImp) =>
        _impTargetedTransform = 
            deadImp == _impTargetedTransform 
            ? null 
            : _impTargetedTransform;

    private void OnGlobalEndBattle(ArenaManager obj) => _impTargetedTransform = null;

    private void OnBossTargetChanged( Transform targetTransform ) => _impTargetedTransform = targetTransform;
}
