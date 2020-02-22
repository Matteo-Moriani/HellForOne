using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDollyCamera : MonoBehaviour
{
    //private Transform target;
    private Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera;
    private Cinemachine.CinemachineTrackedDolly dolly;
    private GameObject midBoss;
    public float speed;

    void Start()
    {
        cinemachineVirtualCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        dolly = cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();
        //target = GameObject.Find( "BossCameraTarget" ).transform;
    }

    void Update()
    {
        dolly.m_PathPosition += speed * Time.deltaTime;
    }
}
