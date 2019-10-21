using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reincarnation : MonoBehaviour
{
    private Stats playerStats;
    private GameObject player;

    public void Reincarnate()
    {
        player.GetComponent<Controller>().enabled = false;
        player.GetComponent<TacticsManager>().enabled = false;
        player.GetComponent<Dash>().enabled = false;
        player.GetComponent<Combat>().enabled = false;
        player.GetComponent<CombatManager>().enabled = false;
        player.tag = "DeadPlayer";

        player = GameObject.FindGameObjectWithTag( "Demon" );
        player.tag = "Player";
        player.GetComponent<DemonBehaviour>().enabled = false;
        player.GetComponent<Controller>().enabled = true;
        player.GetComponent<TacticsManager>().enabled = true;
        player.GetComponent<Dash>().enabled = true;
        player.GetComponent<Reincarnation>().enabled = true;

        // Removing the new player from the group belonging to
        player.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupBehaviour>().reincarnationHappened = true;

        Destroy( gameObject );
    }

    void Start()
    {
        player = gameObject;
        playerStats = gameObject.GetComponent<Stats>();
    }

    void Update()
    {
        if ( playerStats.health <= 0 )
        {
            Reincarnate();
        }
    }
}
