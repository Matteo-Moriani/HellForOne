using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWallsTrigger : MonoBehaviour
{
    public void OnTriggerEnter( Collider other )
    {
        if ( other.gameObject.layer == LayerMask.NameToLayer( "Player" ) )
        {
            Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "Player" ), LayerMask.NameToLayer( "InvisibleWalls" ), false );
            Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "InvisibleWalls" ), LayerMask.NameToLayer( "Player" ), false );
        }
    }
}
