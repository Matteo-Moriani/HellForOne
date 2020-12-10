﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class GroupsFormation : MonoBehaviour
// {
//     private GameObject player;
//     private Transform[] positions;
//     private Dictionary<Transform, bool> available = new Dictionary<Transform, bool>();
//     private float centerDistanceFromPlayer = 5f;
//
//     void Start()
//     {
//         player = GameObject.FindGameObjectWithTag( "Player" );
//     }
//
//     void Awake()
//     {
//         Transform[] tempPositions = gameObject.GetComponentsInChildren<Transform>();
//
//         positions = new Transform[ tempPositions.Length - 1 ];
//
//         for ( int i = 1; i < tempPositions.Length; i++ )
//         {
//             positions[ i - 1 ] = tempPositions[ i ];
//         }
//
//         // assegnare posizioni fisse a gruppi invece che cercare il primo disponibile. cambiare questa cosa in boss positions
//         foreach ( Transform position in positions )
//         {
//             available.Add( position, true );
//         }
//     }
//
//     void FixedUpdate()
//     {
//         // This check is for reincarnation sake
//         if(!player)
//             player = GameObject.FindGameObjectWithTag("Player");
//         else {
//             FacePlayer();
//             transform.position = player.transform.position - centerDistanceFromPlayer * player.transform.forward;
//         }
//     }
//
//     public void SetAvailability( Transform t, bool b )
//     {
//         available[ t ] = b;
//     }
//
//     public bool GetAvailability( Transform t )
//     {
//         bool b;
//         available.TryGetValue( t, out b );
//         return b;
//     }
//
//     public Transform[] GetPositions()
//     {
//         return positions;
//     }
//
//     private void FacePlayer()
//     {
//         Vector3 targetPosition = player.transform.position;
//         Vector3 vectorToTarget = targetPosition - transform.position;
//         vectorToTarget.y = 0f;
//         Quaternion facingDir = Quaternion.LookRotation( vectorToTarget );
//         Quaternion newRotation = Quaternion.Slerp( transform.rotation, facingDir, 0.1f );
//         transform.rotation = newRotation;
//     }
// }
