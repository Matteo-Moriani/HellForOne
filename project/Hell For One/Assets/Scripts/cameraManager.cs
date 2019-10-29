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

    [SerializeField]
    private bool closedEnvironment = false;

    [SerializeField]
    private float distance;

    [SerializeField]
    private bool isLocked = false;

    public GameObject[] enemies;
    public GameObject boss;

    // To avoid takin too much inputs
    private bool rightAxisInUse = false;

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag( "Player" );
        offset = new Vector3( 0.0f, 5.0f, -10.0f );
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
        if ( closedEnvironment )
            turnSpeed = 0.0f;
        else
            turnSpeed = 4.0f;

        if ( !player )
            FindPlayer();

        // Remove lock-on
        if ( Input.GetButtonDown( "R3" ) && isLocked )
        {
            isLocked = false;
            target = player;
        }

        // Start lock-on
        else if ( Input.GetButtonDown( "R3" ) && !isLocked )
        {
            enemies = GameObject.FindGameObjectsWithTag( "LittleEnemy" );
            boss = GameObject.FindGameObjectWithTag( "Boss" );

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

        // Change lock-on target
        float input = Input.GetAxis( "Vertical2" );
        if ( (!rightAxisInUse && Mathf.Abs( input ) > 0.4f) && isLocked )
        {
            rightAxisInUse = true;

            //Do nothing
            if ( !boss && target == boss ) ;

            // Cycle through the enemies
            else
            {
                //enemies = GameObject.FindGameObjectsWithTag( "LittleEnemy" );
                //boss = GameObject.FindGameObjectWithTag( "Boss" );
                float minLeftDistance = -1 * Mathf.Infinity;
                float minRightDistance = Mathf.Infinity;
                GameObject leftNearestDemon = null;
                GameObject rightNearestDemon = null;

                foreach ( GameObject demon in enemies )
                {
                    float demonXAxis = transform.InverseTransformPoint( demon.transform.position ).x;

                    if ( demonXAxis > 0f && demonXAxis < minRightDistance )
                    {
                        minRightDistance = demonXAxis;
                        rightNearestDemon = demon;
                    }

                    if ( demonXAxis < 0f && demonXAxis > minLeftDistance )
                    {
                        minLeftDistance = demonXAxis;
                        leftNearestDemon = demon;
                    }
                }

                // Don't know why it is inverted
                if ( input < 0 && rightNearestDemon )
                    target = rightNearestDemon;
                else if ( input > 0 && leftNearestDemon )
                    target = leftNearestDemon;
            }
        }

        // TODO The problem might not be here, but maybe it chooses again the already targeted enemy when it should change, need to check
        else if ( Mathf.Abs( input ) <= 0.2f )
            rightAxisInUse = false;
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
