using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyDemonSpawnerTest : MonoBehaviour
{

    public IEnumerator SpawnAlly()
    {
        while ( true )
        {
            yield return new WaitForSeconds( 2f );

            if ( GameObject.FindGameObjectsWithTag( "Demon" ).Length < 16 )
            {
                GameObject demonToSpawn = Resources.Load( "Prefabs/PlayerEvolved" ) as GameObject;
                Instantiate( demonToSpawn, SpawnPosition(), Quaternion.identity );
            }
        }
    }

    public Vector3 SpawnPosition()
    {
        Vector3 spawnPosition = new Vector3( 0, 1, 0 );
        spawnPosition.x = Random.Range( -10f, 10f );
        spawnPosition.z = Random.Range( -10f, 10f );
        return spawnPosition;
    }

    void Start()
    {
        StartCoroutine( SpawnAlly() );
    }
}
