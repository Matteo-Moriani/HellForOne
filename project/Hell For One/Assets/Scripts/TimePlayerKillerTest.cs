using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePlayerKillerTest : MonoBehaviour
{
    //public GameObject[] demons;
    public bool allDemonsKilled;

    public IEnumerator Kill()
    {
        while ( true )
        {
            if ( allDemonsKilled )
                yield break;
            yield return new WaitForSeconds( 2f );
            GameObject.FindGameObjectWithTag( "Player" ).GetComponent<Stats>().health = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //int index = 0;
        //demons = new GameObject[ 16 ];
        //foreach ( GameObject demon in GameObject.FindGameObjectsWithTag( "Demon" ) )
        //{
        //    demons[ index ] = demon;
        //    index++;
        //}

        StartCoroutine( Kill() );
    }

    // Update is called once per frame
    void Update()
    {

    }
}
