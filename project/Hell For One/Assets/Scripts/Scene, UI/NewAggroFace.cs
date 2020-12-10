﻿using AI.MidBoss;
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

    void Start()
    {
        
    }

    private void OnEnable()
    {
        MidBossAi.OnBossTargetChanged += OnBossTargetChanged;
    }

    private void OnBossTargetChanged( Transform targetTransform )
    {
        Vector3 aggroFacePosition = new Vector3();

        // 1.825f is the height where the AggroFace should be
        aggroFacePosition = targetTransform.position + new Vector3( 0f , 1.825f , 0f );
        gameObject.transform.position = aggroFacePosition;
    }

    private void OnDisable()
    {
        MidBossAi.OnBossTargetChanged -= OnBossTargetChanged;
    }

    void Update()
    {
        cameraPosition = Camera.main.transform.position;
        rotateDirection = (cameraPosition - transform.position).normalized;
        lookRotation = Quaternion.LookRotation( rotateDirection );
        transform.rotation = Quaternion.Slerp( transform.rotation , lookRotation , 1 );
    }
}