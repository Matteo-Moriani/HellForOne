using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCaster : MonoBehaviour
{
    #region Fields

    [SerializeField, Min( 0 ), Tooltip( "The minimum distance of attack of the ranged unit." )]
    private float minDistance;
    [SerializeField, Min( 0 ), Tooltip( "The maximum distance of attack of the ranged unit." )]
    private float maxDistance;
    
    [Space]
    
    [SerializeField, Tooltip( "If the lance follow a direct trajectory or not (false is recommended)." )]
    private bool direct;
    
    [SerializeField, Tooltip("Projectile default start position")] 
    private GameObject projectilePosition;

    #endregion

    #region Methods
    
    public void Launch(GameObject projectile, Transform target, Transform projectileAnchor, float speed)
    {
        float distance = Vector3.Distance(target.position, projectileAnchor.position);

        if(distance < minDistance || distance > maxDistance) return;
        
        Vector3 targetPosFixed = target.transform.position + new Vector3( 0f, 1f, 0f );
            
        if (!CalculateAngle( projectilePosition.transform.position, targetPosFixed, speed, out float alpha)) return;

        projectile.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        projectile.transform.position = projectilePosition.transform.position;
        
        projectile.transform.forward = new Vector3( target.transform.position.x, projectile.transform.position.y, target.transform.position.z ) - projectile.transform.position;
        projectile.transform.rotation = Quaternion.Euler( 90f - alpha, projectile.transform.eulerAngles.y, 0 );

        projectile.GetComponent<Rigidbody>().AddForce( projectile.transform.up * (speed), ForceMode.VelocityChange );
    }

    private bool CalculateAngle( Vector3 from, Vector3 to, float speed, out float angle )
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
        
        return true;
    }

    #endregion
}
