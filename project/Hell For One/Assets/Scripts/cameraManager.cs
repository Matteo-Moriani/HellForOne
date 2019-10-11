using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraManager : MonoBehaviour
{

    GameObject player;
    public float yRotation;
    public Vector3 rotation;
    public float cameraDistance = 20f;
    public float cameraHeight = 7.5f;

    Vector3 cameraHeightFixed;
    Vector3 cameraToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag( "Player" );
    }

    // TODO if I move then rotate camera ==> MESS
    void Update()
    {
        yRotation = Input.GetAxis( "Vertical2" );
        transform.LookAt( player.transform );

        if ( yRotation != 0f )
        {
            rotation = Vector3.right * yRotation;

            transform.Translate( rotation );

            cameraToPlayer = player.transform.position - transform.position;
            transform.position += (cameraToPlayer - cameraToPlayer.normalized * 20);

            // Don't do new in Update()!!!
            cameraHeightFixed = new Vector3( transform.position.x, cameraHeight, transform.position.z );

            //Vector3 cameraHeightVector = new Vector3( transform.position.x, cameraHeight , transform.position.z );
            transform.position += cameraHeightFixed - cameraHeightFixed.normalized * 7.5f;

            cameraDistance = cameraToPlayer.magnitude;

            //transform.Translate( Vector3.forward * Time.deltaTime * 30 );
        }
    }
}
