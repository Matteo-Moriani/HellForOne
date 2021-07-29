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
    private bool inBattle = false;

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

    public void OnGlobalStartBattle( ArenaManager arenaManager )
    {
        StopCoroutine( timerCoroutine );
        inBattle = true;
    }

    public void OnGlobalEndBattle( ArenaManager arenaManager )
    {
        StartTimer();
        inBattle = false;
    }

    // Only outside battle
    public void StartTimer()
    {
        timerCoroutine = StartCoroutine( TeleportToGroup() );
    }

    public IEnumerator TeleportToGroup()
    {
        while ( !inBattle )
        {
            yield return new WaitForSeconds( timeoutTimer );

            // Adding new imps if there are
            foreach ( GameObject imp in GameObject.FindGameObjectsWithTag( "Demon" ) )
            {
                if ( !otherImpsPosition.Contains( imp ) && imp != gameObject )
                {
                    otherImpsPosition.Add( imp );
                }
            }

            // Removing dead imps if there are
            for ( int i = 0; i < otherImpsPosition.Count; i++ )
            {
                if ( otherImpsPosition[ i ] == null )
                    otherImpsPosition.Remove( otherImpsPosition[ i ] );
            }

            Vector3 hordeMeanPosition = Vector3.zero;

            // Calculating the mean position of all horde
            foreach ( GameObject imp in otherImpsPosition )
            {
                if ( imp != null )
                {
                    // Player's layer
                    if ( imp.layer != LayerMask.NameToLayer( "Player" ) )
                        hordeMeanPosition += imp.transform.position;
                }
            }

            hordeMeanPosition /= otherImpsPosition.Count;

            if ( (gameObject.GetComponent<GroupFinder>().Group.transform.position - transform.position).magnitude > maxDistance )
            {
                // Player's layer
                if ( gameObject.layer != LayerMask.NameToLayer( "Player" ) && inBattle)
                {
                    transform.position = hordeMeanPosition;
                    //Debug.Log(gameObject.name + " teleported");
                }
            }
        }
    }
}
