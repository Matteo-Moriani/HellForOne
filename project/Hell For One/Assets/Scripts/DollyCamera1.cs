using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyCamera1 : MonoBehaviour
{
    private Transform target;
    private Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera;
    private Cinemachine.CinemachineTrackedDolly dolly;
    private GameObject midBoss;

    void Start()
    {
        cinemachineVirtualCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        dolly = cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();
        target = GameObject.Find( "DollyTrackTarget" ).transform;
        midBoss = GameObject.Find( "MidBoss" );
    }

    // Update is called once per frame
    void Update()
    {
        if ((target.position -transform.position).magnitude >= 30f )
        {
            dolly.m_AutoDolly.m_Enabled = true;
        }

        //if ( dolly.m_PathPosition >= 11 )
        //{
        //    cinemachineVirtualCamera.m_LookAt = midBoss.transform;
        //}
    }
}
