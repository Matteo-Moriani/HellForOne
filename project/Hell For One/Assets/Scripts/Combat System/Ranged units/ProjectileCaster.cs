using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCaster : MonoBehaviour
{
    #region Fields

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
    
    [SerializeField, Tooltip("Projectile default start position")] 
    private GameObject projectilePosition;

    [SerializeField, Tooltip("After how many seconds the projectile will be deactivated after being launched")] 
    private float projectileLifeTime = 3.0f;
    
    private GameObject projectile;

    #endregion

    #region Methods

    // TODO - clean this
    /// <summary>
    /// Lauch a generic projectile
    /// </summary>
    /// <param name="target">Target for the projectile</param>
    /// <param name="projectilePooler">Pooler where the projectile will be taken</param>
    /// <returns></returns>
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

        // TODO - Generalize target
        if (target.tag == "Boss" )
        {
            Vector3 targetPosFixed = target.transform.position + new Vector3( 0f, 1f, 0f );
            
            if ( !CalculateAngle( projectilePosition.transform.position, targetPosFixed, out alpha ) )
            {
                return null;
                
                Debug.LogError("Angle error");
            }
        }
        
        projectile = projectilePooler.GetNotActiveObject();
        
        projectile.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        projectile.transform.position = projectilePosition.transform.position;
        
        projectile.transform.forward = new Vector3( target.transform.position.x, projectile.transform.position.y, target.transform.position.z ) - projectile.transform.position;
        projectile.transform.rotation = Quaternion.Euler( 90f - alpha, projectile.transform.eulerAngles.y, 0 );
        
        projectile.SetActive( true );
        
        projectile.GetComponent<Rigidbody>().AddForce( projectile.transform.up * (speed), ForceMode.VelocityChange );
        
        return projectile;
    }

    /// <summary>
    /// Overload for LaunchNewCombatSystem
    /// Launch a projectile from chosen position
    /// </summary>
    /// <param name="target">Target for the projectile</param>
    /// <param name="projectilePooler">Pooler where the projectile will be taken</param>
    /// <param name="launchPosition">Projectile's start position</param>
    /// <returns>The projectile</returns>
    public GameObject LaunchNewCombatSystem( GameObject target, ObjectsPooler projectilePooler, Vector3 launchPosition)
    {
        float distance = Vector3.Distance( launchPosition, target.transform.position );;
        float alpha = 0;
        
        if (target == null)
            return null;
        
        if ( distance < minDistance || distance > maxDistance )
        {
            return null;
            
            Debug.LogError("Distance error");
        }

        // TODO - Generalize target
        if (target.tag == "Boss" )
        {
            Vector3 targetPosFixed = target.transform.position + new Vector3( 0f, 1f, 0f );
            
            if ( !CalculateAngle( launchPosition, targetPosFixed, out alpha ) )
            {
                return null;
                
                Debug.LogError("Angle error");
            }
        }
        
        projectile = projectilePooler.GetNotActiveObject();
        
        projectile.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        projectile.transform.position = launchPosition;
        
        projectile.transform.forward = new Vector3( target.transform.position.x, projectile.transform.position.y, target.transform.position.z ) - projectile.transform.position;
        projectile.transform.rotation = Quaternion.Euler( 90f - alpha, projectile.transform.eulerAngles.y, 0 );
        
        projectile.SetActive( true );
        
        projectile.GetComponent<Rigidbody>().AddForce( projectile.transform.up * (speed), ForceMode.VelocityChange );
        
        return projectile;
    }
    
    /// <summary>
    /// Returns the angle range (with 0° corresponding to the ground) to launch an object from a position to another with a velocity that is "speed".
    /// </summary>
    /// <param name="from">The start position of the launch.</param>
    /// <param name="to">The final position of the launch.</param>
    /// <param name="angle">The varible that will contain the angle in degree.</param>
    /// <returns>Returns true if it is mathematically possible to have a trajectory, otherwise returns false.</returns>
    private bool CalculateAngle( Vector3 from, Vector3 to, out float angle )
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

    #endregion
}
