using System;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class ProjectileMovement : MonoBehaviour
    {
        #region Fields

        private Rigidbody _rigidbody;

        #endregion
        
        #region Unity Methods

        private void Awake() => _rigidbody = GetComponent<Rigidbody>();

        // Lippolis' code had this
        private void FixedUpdate() => transform.up = _rigidbody.velocity;

        #endregion
        
        #region Methods

        public bool TryLaunch(Transform target, Transform projectileAnchor, float minDistance, float maxDistance, float speed)
        {
            float distance = Vector3.Distance(target.position, projectileAnchor.position);

            if(distance < minDistance || distance > maxDistance) return false;
        
            Vector3 targetPosFixed = target.transform.position + new Vector3( 0f, 1f, 0f );
            
            if (!CalculateAngle( projectileAnchor.position, targetPosFixed, speed, out float alpha)) return false;

            _rigidbody.velocity = Vector3.zero;
        
            transform.position = projectileAnchor.position;
        
            transform.forward = new Vector3( target.transform.position.x, transform.position.y, target.transform.position.z ) - transform.position;
            transform.rotation = Quaternion.Euler( 90f - alpha, transform.eulerAngles.y, 0 );

            _rigidbody.AddForce( transform.up * (speed), ForceMode.VelocityChange );

            return true;
        }

        // TODO :- Improve stop logic
        public void Stop() => _rigidbody.constraints = RigidbodyConstraints.FreezeAll;

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
            tempResult = v * v + tempResult;
            tempResult = Mathf.Atan2( tempResult, g * x );

            angle = tempResult * Mathf.Rad2Deg;
        
            return true;
        }

        #endregion
    }
}