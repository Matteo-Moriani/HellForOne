using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyDemonSpawnerTest : MonoBehaviour
{

    public void SpawnAlly()
    {
        while ( true )
        {
            if ( GameObject.FindGameObjectsWithTag( "Demon" ).Length < 16 )
            {
                //GameObject demonToSpawn = Resources.Load("")
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
