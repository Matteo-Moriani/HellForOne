using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonsDollyCamera : MonoBehaviour
{
    private Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera;
    private Cinemachine.CinemachineTrackedDolly dolly;
    private GameObject midBoss;
    public float speed;
    public float rotationSpeed;
    private Vector3 rotation = new Vector3(0f, 0f, 0f);

    void Start()
    {
        cinemachineVirtualCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        dolly = cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();
    }

    // Update is called once per frame
    void Update()
    {
        dolly.m_PathPosition += speed * Time.deltaTime;
        rotation.y += rotationSpeed * Time.deltaTime;
        transform.Rotate( Vector3.up * rotationSpeed * Time.deltaTime, Space.World );
        //transform.rotation.eulerAngles.Set( rotation.x, rotation.y, rotation.z );
    }
}
