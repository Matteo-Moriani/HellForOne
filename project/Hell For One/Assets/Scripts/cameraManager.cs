using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraManager : MonoBehaviour
{
    // Can be an enemy or the player
    public GameObject target;
    private GameObject player;
    [SerializeField]
    private float turnSpeed = 4.0f;
    private Vector3 offset;

    // For testing sake
    [SerializeField]
    private float distance;

    [SerializeField]
    private bool isLocked = false;

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag( "Player" );
        //offset = new Vector3( player.transform.position.x, player.transform.position.y + 20.0f, player.transform.position.z - 30.0f );
        offset = new Vector3( 0.0f, 20.0f, -30.0f );
    }

    public GameObject FindNearestEnemy( GameObject[] gameObjects )
    {
        float minDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach ( GameObject enemy in gameObjects )
        {
            if ( (enemy.transform.position - transform.position).magnitude < minDistance )
            {
                minDistance = (enemy.transform.position - transform.position).magnitude;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    void Start()
    {
        FindPlayer();
        target = player;
    }

    private void Update()
    {
        if ( Input.GetButtonDown( "R3" ) && isLocked )
        {
            isLocked = false;
            target = player;
        }
        else if ( Input.GetButtonDown( "R3" ) && !isLocked )
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag( "Little Enemy" );
            GameObject boss = GameObject.FindGameObjectWithTag( "Enemy" );

            if ( boss == null && enemies != null )
            {
                isLocked = true;
                target = FindNearestEnemy( enemies );
            }
            else if ( boss != null )
            {
                isLocked = true;
                target = boss;
            }
        }
    }

    void LateUpdate()
    {
        if ( target == null )
        {
            FindPlayer();
        }

        // If target enemy dies
        if ( target != player && target.GetComponent<Stats>().health <= 0 )
            target = player;

        offset = Quaternion.AngleAxis( Input.GetAxis( "Vertical2" ) * turnSpeed, Vector3.up ) * offset;

        if ( isLocked )
        {
            Vector3 cameraPos = player.transform.position - target.transform.position;
            cameraPos.y += 20.0f;
            // Error is here, Can't just subtract 30.0f on z axis, it works like that only if it's local (and I'm looking in enemy direction) not world
            //cameraPos.z += -30.0f;
            //TODO find the error here
            float alfa = Vector3.Angle( new Vector3( 0f, 0f, 1f ), target.transform.position - player.transform.position );
            cameraPos.x += -30 * Mathf.Sin( alfa );
            cameraPos.z += -30 * Mathf.Cos( alfa );

            transform.position = cameraPos;
        }
        else
        {
            transform.position = player.transform.position + offset;
        }

        transform.LookAt( target.transform.position );

        // Testing if the distance remains the same during reincarnations
        distance = (player.transform.position - transform.position).magnitude;
    }
}
