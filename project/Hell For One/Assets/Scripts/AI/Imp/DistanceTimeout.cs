using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GroupSystem;
using ArenaSystem;

public class DistanceTimeout : MonoBehaviour
{

    private float timeoutTimer = 2f;
    private float maxDistance = 10f;
    private List<GameObject> otherImpsPosition;
    private Coroutine timerCoroutine;

    void Start()
    {
        otherImpsPosition = new List<GameObject>();

        StartTimer();
    }

    private void OnEnable()
    {
        ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
        ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
    }

    private void OnDisable()
    {
        ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
        ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;
    }

    public void OnGlobalStartBattle(ArenaManager arenaManager)
    {
        StopCoroutine( timerCoroutine );
    }

    public void OnGlobalEndBattle( ArenaManager arenaManager )
    {
        StartTimer();
    }

    // Only outside battle
    public void StartTimer()
    {
        timerCoroutine = StartCoroutine( TeleportToGroup() );
    }

    public IEnumerator TeleportToGroup()
    {
        while ( true )
        {
            yield return new WaitForSeconds( timeoutTimer );

            // Adding new imps if there are
            foreach ( GameObject imp in GameObject.FindGameObjectsWithTag("Demon") )
            {
                if ( !otherImpsPosition.Contains( imp ) )
                {
                    otherImpsPosition.Add( imp );
                }
            }

            // Removing dead imps if there are
            foreach ( GameObject imp in otherImpsPosition )
            {
                if ( imp == null )
                    otherImpsPosition.Remove( imp );
            }

            Vector3 hordeMeanPosition = Vector3.zero;

            // Calculating the mean position of all horde
            foreach ( GameObject imp in otherImpsPosition )
            {
                hordeMeanPosition += imp.transform.position;
            }

            hordeMeanPosition /= otherImpsPosition.Count;

            if ( (gameObject.GetComponent<GroupFinder>().Group.transform.position - transform.position).magnitude > maxDistance )
            {
                transform.position = hordeMeanPosition;
            }
        }
    }
}
