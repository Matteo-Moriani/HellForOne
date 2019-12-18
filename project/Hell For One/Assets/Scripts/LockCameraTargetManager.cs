using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LockCameraTargetManager : MonoBehaviour
{

    private GameObject player;

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag( "Player" );
        GetComponent<CinemachineTargetGroup>().m_Targets[ 0 ].target = player.transform;
        GetComponent<CinemachineTargetGroup>().m_Targets[ 1 ].target = transform;
    }

    void Start()
    {
        FindPlayer();
    }

    void Update()
    {
        if ( !player )
        {
            FindPlayer();
        }
    }
}
