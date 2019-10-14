using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraManager : MonoBehaviour
{

    GameObject player;
    Controller controller;
    public float turnSpeed = 4.0f;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag( "Player" );
        offset = new Vector3( player.transform.position.x, player.transform.position.y + 20.0f, player.transform.position.z - 30.0f );
        controller = player.GetComponent<Controller>();
    }

    void LateUpdate()
    {
        offset = Quaternion.AngleAxis( Input.GetAxis( "Vertical2" ) * turnSpeed, Vector3.up ) * offset;
        transform.position = player.transform.position + offset;
        transform.LookAt( player.transform.position );
        //controller.mainCameraForward = transform.forward;
        //controller.mainCameraForward.y = 0f;
    }
}
