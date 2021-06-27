using FactoryBasedCombatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchGameOver : MonoBehaviour
{

    ImpDeath impDeath;

    private void OnEnable()
    {
        ImpDeath.OnImpDeath += OnImpDeath;
    }

    private void OnDisable()
    {
        ImpDeath.OnImpDeath -= OnImpDeath;
    }

    public void OnImpDeath( Transform transform )
    {
        GameObject[] imps;

        // Se il player non si è mai reincarnato

        if ( GameObject.FindGameObjectWithTag( "Player" ) != null )
        {
            imps = GameObject.FindGameObjectsWithTag( "Demon" );

            if ( imps.Length < 1 )
            {
                GameObject.Find( "GameOverScreen" ).SetActive( true );
            }

        }

        // Se invece si è reincarnato almeno una volta

        else
        {
            imps = GameObject.FindGameObjectsWithTag( "Demon" );

            if ( imps.Length == 1 )
            {
                GameObject.Find( "GameOverScreen" ).SetActive( true );
            }
        }
    }
}
