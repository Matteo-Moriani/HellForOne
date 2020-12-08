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
        
        private void FixedUpdate() => transform.up = _rigidbody.velocity;

        #endregion

        #region Methods

        public bool Launch(Transform target, Transform projectileAnchor, float minDistance, float maxDistance,
            float speed)
        {
            _rigidbody.velocity = Vector3.zero;
            
            float distance = Vector3.Distance(target.position, projectileAnchor.position);

            if (distance < minDistance || distance > maxDistance) return false;

            Vector3 targetPosFixed = target.transform.position + new Vector3(0f, 1f, 0f);

            float alpha = CalculateAngle(Vector3.Distance(projectileAnchor.position, targetPosFixed), speed);

            transform.rotation = Quaternion.Euler(90 - alpha,projectileAnchor.rotation.eulerAngles.y, projectileAnchor.rotation.eulerAngles.z);

            _rigidbody.AddForce(transform.up * (speed), ForceMode.VelocityChange);

            return true;
        }

        // TODO :- Implement stop logic
        public void Stop()
        {
        }

        //_rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        private float CalculateAngle(float distance, float speed) => 
            0.5f * Mathf.Rad2Deg * Mathf.Asin(Mathf.Clamp((Physics.gravity.magnitude * distance) / (speed * speed), -1f, 1f));

        #endregion

    }
}