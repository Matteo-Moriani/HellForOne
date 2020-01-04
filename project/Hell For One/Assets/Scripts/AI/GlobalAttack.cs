using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAttack : MonoBehaviour
{
    public float globalAttackDelay = 2.8f;
    private float circlesTimeGap = 0.4f;
    private float circleDuration = 0.5f;
    private int circles = 6;
    private int particlesPerCircle = 18;
    private GameObject[,] flameCircleArray;
    private GameObject boss;

    private void Awake() {
        boss = transform.root.gameObject;
    }

    private void OnEnable()
    {
        boss.GetComponent<CombatEventsManager>().onStartGlobalAttack += StartFlamingAttack;
        //boss.GetComponent<CombatEventsManager>().onStartGlobalAttack += StopFlamingAttack;

        for ( int i = 0; i < circles; i++ )
        {
            for ( int j = 0; j < particlesPerCircle; j++ )
            {
                flameCircleArray[ i, j ] = transform.GetChild( i ).gameObject.transform.GetChild( j ).gameObject;
                ParticleSystem particleSystem = flameCircleArray[ i, j ].GetComponent<ParticleSystem>();
                if ( particleSystem )
                    particleSystem.Stop();
            }
        }
    }

    private void OnDisable() {
        boss.GetComponent<CombatEventsManager>().onStartGlobalAttack -= StartFlamingAttack;
    }

    void Start()
    {
        flameCircleArray = new GameObject[ circles, particlesPerCircle ];

        for ( int i = 0; i < circles; i++ )
        {
            for ( int j = 0; j < particlesPerCircle; j++ )
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
        yield return new WaitForSeconds(globalAttackDelay);

        for ( int i = 0; i < circles; i++ )
        {
            yield return new WaitForSeconds( circlesTimeGap );

            for ( int j = 0; j < particlesPerCircle; j++ )
            {
                ParticleSystem particleSystem = flameCircleArray[ i, j ].GetComponent<ParticleSystem>();
                if ( particleSystem )
                    particleSystem.Play();
            }
        }

        yield return new WaitForSeconds(circleDuration);

        StartCoroutine(DeactivateFlamingSpecialAttack());
    }

    public void StopFlamingAttack()
    {
        StartCoroutine( DeactivateFlamingSpecialAttack() );
    }

    public IEnumerator DeactivateFlamingSpecialAttack()
    {
        for ( int i = 0; i < circles; i++ )
        {
            yield return new WaitForSeconds( circlesTimeGap );

            for ( int j = 0; j < particlesPerCircle; j++ )
            {
                ParticleSystem particleSystem = flameCircleArray[ i, j ].GetComponent<ParticleSystem>();
                if ( particleSystem )
                    particleSystem.Stop();
            }
        }
    }

}
