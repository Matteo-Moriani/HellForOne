using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePlayerKillerTest : MonoBehaviour
{
    // Not used right now
    public bool allDemonsKilled = false;

    public IEnumerator Kill()
    {
        while ( true )
        {
            if ( !allDemonsKilled )
                yield break;
            yield return new WaitForSeconds( 2f );
            GameObject.FindGameObjectWithTag( "Player" ).GetComponent<Stats>().health = 0;
        }
    }

    void Start()
    {
        StartCoroutine( Kill() );
    }
}
