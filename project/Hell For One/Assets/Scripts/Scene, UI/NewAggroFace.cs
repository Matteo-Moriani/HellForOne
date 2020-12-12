using AI.MidBoss;
using ArenaSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Must be added to the real aggro face in the scene so it can move that aggro face to the aggroed imp 
/// </summary>

public class NewAggroFace : MonoBehaviour
{
    private Vector3 cameraPosition;
    private Vector3 rotateDirection;
    private Quaternion lookRotation;
    private Transform impTargetedTransform;
    private Vector3 aggroFacePosition;
    private bool inBattle = false;

    private void OnEnable()
    {
        MidBossAi.OnBossTargetChanged += OnBossTargetChanged;
        ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
    }

    private void OnDisable()
    {
        MidBossAi.OnBossTargetChanged -= OnBossTargetChanged;
        ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;
    }

    private void OnGlobalEndBattle( ArenaManager obj )
    {
        inBattle = false;

        aggroFacePosition = new Vector3( 0f , -100f , 0f );
        gameObject.transform.position = aggroFacePosition;
    }

    private void OnBossTargetChanged( Transform targetTransform )
    {
        if ( !inBattle )
        {
            // First imp targeted in the fight
            inBattle = true;
        }

        impTargetedTransform = targetTransform;
    }

    void Update()
    {
        if ( inBattle )
        {
            // 1.825f is the height where the AggroFace should be
            aggroFacePosition = impTargetedTransform.position + new Vector3( 0f , 1.825f , 0f );
            gameObject.transform.position = aggroFacePosition;
        }
        
        cameraPosition = Camera.main.transform.position;
        rotateDirection = (cameraPosition - transform.position).normalized;
        lookRotation = Quaternion.LookRotation( rotateDirection );
        transform.rotation = Quaternion.Slerp( transform.rotation , lookRotation , 1 );
    }
}
