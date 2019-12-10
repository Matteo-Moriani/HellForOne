using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttack : MonoBehaviour
{

    private float activationTime = 0.5f;
    private float deactivationTime = 1f;
    private GameObject[,] flameCircleArray = new GameObject[ 3, 18 ];

    private void OnEnable()
    {
        GameObject boss = transform.root.gameObject;
        boss.GetComponent<CombatEventsManager>().onStartGlobalAttack += StartFlamingAttack;
        boss.GetComponent<CombatEventsManager>().onStartGlobalAttack += StopFlamingAttack;

        for ( int i = 0; i < 3; i++ )
        {
            for ( int j = 0; j < 18; j++ )
            {
                flameCircleArray[ i, j ] = transform.GetChild( i ).gameObject.transform.GetChild( j ).gameObject;
                ParticleSystem particleSystem = flameCircleArray[ i, j ].GetComponent<ParticleSystem>();
                if ( particleSystem )
                    particleSystem.Stop();
            }
        }
    }

    void Start()
    {

        for ( int i = 0; i < 3; i++ )
        {
            for ( int j = 0; j < 18; j++ )
            {
                flameCircleArray[ i, j ] = transform.GetChild( i ).gameObject.transform.GetChild( j ).gameObject;
                ParticleSystem particleSystem = flameCircleArray[ i, j ].GetComponent<ParticleSystem>();
                if ( particleSystem )
                    particleSystem.Stop();
            }
        }
    }

    public void StartFlamingAttack()
    {
        StartCoroutine( ActivateFlamingSpecialAttack() );
    }

    public IEnumerator ActivateFlamingSpecialAttack()
    {
        for ( int i = 0; i < 3; i++ )
        {
            yield return new WaitForSeconds( activationTime );

            for ( int j = 0; j < 18; j++ )
            {
                ParticleSystem particleSystem = flameCircleArray[ i, j ].GetComponent<ParticleSystem>();
                if ( particleSystem )
                    particleSystem.Play();
            }
        }
    }

    public void StopFlamingAttack()
    {
        StartCoroutine( DeactivateFlamingSpecialAttack() );
    }

    public IEnumerator DeactivateFlamingSpecialAttack()
    {
        for ( int i = 0; i < 3; i++ )
        {
            yield return new WaitForSeconds( deactivationTime );

            for ( int j = 0; j < 18; j++ )
            {
                ParticleSystem particleSystem = flameCircleArray[ i, j ].GetComponent<ParticleSystem>();
                if ( particleSystem )
                    particleSystem.Stop();
            }
        }
    }

}
