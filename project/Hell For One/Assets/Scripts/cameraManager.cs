using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraManager : MonoBehaviour
{
    public GameObject player;
    [SerializeField]
    private float turnSpeed = 4.0f;
    private Vector3 offset;

    // For testing sake
    [SerializeField]
    private float distance;

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag( "Player" );
        offset = new Vector3( player.transform.position.x, player.transform.position.y + 20.0f, player.transform.position.z - 30.0f );
    }

    void Start()
    {
        FindPlayer();
    }

    void LateUpdate()
    {
        if ( player == null )
        {
            FindPlayer();
        }
        offset = Quaternion.AngleAxis( Input.GetAxis( "Vertical2" ) * turnSpeed, Vector3.up ) * offset;
        transform.position = player.transform.position + offset;
        transform.LookAt( player.transform.position );

        // Testing if the distance remains the same during reincarnations
        distance = (player.transform.position - transform.position).magnitude;
    }
}
