using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Can be an enemy or the player
    public GameObject target;
    public GameObject player;
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
        offset = new Vector3( 0.0f, 20.0f, -30.0f );
        target = player;
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
        if ( !player )
            FindPlayer();

        if ( Input.GetButtonDown( "R3" ) && isLocked )
        {
            isLocked = false;
            target = player;
        }
        else if ( Input.GetButtonDown( "R3" ) && !isLocked )
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag( "LittleEnemy" );
            GameObject boss = GameObject.FindGameObjectWithTag( "Boss" );

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
        {
            target = player;
            isLocked = false;
        }

        offset = Quaternion.AngleAxis( Input.GetAxis( "Vertical2" ) * turnSpeed, Vector3.up ) * offset;

        if ( isLocked )
        {
            Vector3 cameraPos = player.transform.position - target.transform.position;

            Ray ray = new Ray( target.transform.position, cameraPos );
            Vector3 rayOffset = ray.GetPoint( cameraPos.magnitude + 30.0f );
            rayOffset.y += 20.0f;

            transform.position = rayOffset;
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
