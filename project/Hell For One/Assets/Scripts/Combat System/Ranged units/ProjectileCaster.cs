using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCaster : MonoBehaviour
{
    //[SerializeField]
    //private ObjectsPooler ProjectilesPooler;
    
    [Space]
    
    [SerializeField, Min( 0 ), Tooltip( "The initial speed of the lance." )]
    private float speed;
    
    [Space]
    
    [SerializeField, Min( 0 ), Tooltip( "The mininum distance of attack of the ranged unit." )]
    private float minDistance;
    [SerializeField, Min( 0 ), Tooltip( "The maximum distance of attack of the ranged unit." )]
    private float maxDistance;
    
    [Space]
    
    [SerializeField, Tooltip( "If the lance follow a direct trajectory or not (false is recommended)." )]
    private bool direct;
    
    [SerializeField]
    private GameObject projectilePosition;
    
    //private Vector3 spearLaunchPoint;

    private GameObject projectile;
    
    /*
    // Start is called before the first frame update
    void Start()
    {
        //ProjectilesPooler = ProjectilesPooler.GetComponent<ObjectsPooler>();
        
        //spearLaunchPoint = spearPosition.transform.position;
        
        // TODO - Set positions properly
        //spearLaunchPoint = new Vector3(0f, 0.7f, 0f);
    }
    */
    
    public GameObject LaunchNewCombatSystem( GameObject target, ObjectsPooler projectilePooler)
    {
        float distance;
        float alpha;
        if ( target == null )
        {
            //Debug.LogError( "Target cannot be null" );
        }

        distance = Vector3.Distance( transform.position, target.transform.position );
        if ( distance < minDistance || distance > maxDistance )
        {
            return null;
            
            Debug.LogError("Distance error");
        }

        alpha = 0;

        if (target.tag == "Boss" )
        {
            Vector3 targetPosFixed = target.transform.position + new Vector3( 0f, 1f, 0f );

            //if ( !calculateAngle( transform.position + spearLaunchPoint + RightComponent(), targetPosFixed, out alpha ) )
            if ( !calculateAngle( projectilePosition.transform.position, targetPosFixed, out alpha ) )
            {
                return null;
                
                Debug.LogError("Angle error");
            }
        }
        
        /*
        else if ( !calculateAngle( transform.position + spearLaunchPoint + RightComponent(), target.transform.position, out alpha ) )
        {
            return null;
            
            Debug.LogError("Unknown error");
        }
        */

        //transform.forward = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z) - transform.position;
        projectile = projectilePooler.GetNotActiveObject();
        projectile.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //projectile.transform.position = transform.position + spearLaunchPoint + RightComponent();
        projectile.transform.position = projectilePosition.transform.position;
        projectile.transform.forward = new Vector3( target.transform.position.x, projectile.transform.position.y, target.transform.position.z ) - projectile.transform.position;
        projectile.transform.rotation = Quaternion.Euler( 90f - alpha, projectile.transform.eulerAngles.y, 0 );
        projectile.SetActive( true );
        
        projectile.GetComponent<Rigidbody>().AddForce( projectile.transform.up * (speed), ForceMode.VelocityChange );
        //lance = null;

        return projectile;
    }

    /// <summary>
    /// Returns the angle range (with 0° corresponding to the ground) to launch an object from a position to another with a velocity that is "speed".
    /// </summary>
    /// <param name="from">The start position of the launch.</param>
    /// <param name="to">The final position of the launch.</param>
    /// <param name="angle">The varible that will contain the angle in degree.</param>
    /// <returns>Returns true if it is mathematically possible to have a trajectory, otherwise returns false.</returns>
    private bool calculateAngle( Vector3 from, Vector3 to, out float angle )
    {
        float x, y, g, v;
        float tempResult;

        x = Vector3.Distance( new Vector3( from.x, 0, from.z ), new Vector3( to.x, 0, to.z ) );

        y = to.y - from.y;
        g = -Physics.gravity.y;
        v = speed;

        tempResult = g * x * x + 2 * y * v * v;
        tempResult *= g;
        tempResult = v * v * v * v - tempResult;
        if ( tempResult < 0 )
        {
            angle = 0;
            
            Debug.LogError(this.gameObject.name + " Lancer launch failed");
            
            return false;
        }

        tempResult = Mathf.Sqrt( tempResult );

        if ( direct )
        {
            tempResult = v * v - tempResult;
        }
        else
        {
            tempResult = v * v + tempResult;
        }

        tempResult = Mathf.Atan2( tempResult, g * x );

        angle = tempResult * Mathf.Rad2Deg;

        //Debug.Log( "x: " + x.ToString() + " alpha:" + angle.ToString() );
        return true;
    }
    /*
    private Vector3 RightComponent() {
        return transform.right * 0.4f;
    }
    */
}
