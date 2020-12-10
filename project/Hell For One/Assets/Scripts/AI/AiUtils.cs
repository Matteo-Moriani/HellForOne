using UnityEngine;

namespace CRBT
{
    public class AiUtils
    { 
        public class TargetData
        {
            private Transform _target;

            public TargetData()
            {
                _target = null;
            }

            public Transform Target
            {
                get => _target;
                private set => _target = value;
            }

            public void SetTarget(Transform newTarget)
            {
                _target = newTarget;
            }

            public Vector3 GetDirectionToTarget(Transform agentTransform)
            {
                if (_target == null)
                    return Vector3.zero;
                
                GetPlanePositions(agentTransform,out Vector3 agentPosition, out Vector3 targetPosition);

                return (targetPosition - agentPosition).normalized;
            }

            public float GetTransformDistanceFromTarget(Transform agentTransform)
            {
                if (_target == null)
                    return float.MaxValue;

                GetPlanePositions(agentTransform,out Vector3 agentPosition, out Vector3 targetPosition);
                
                return Vector3.Distance(agentPosition,targetPosition);
            }

            public float GetColliderDistanceFromTarget(Transform agentTransform)
            {
                Collider agentCollider = agentTransform.GetComponent<Collider>();
                Collider targetCollider = _target.GetComponent<Collider>();

                if (agentCollider != null && targetCollider != null)
                    return (agentCollider.ClosestPoint(_target.position) -
                            targetCollider.ClosestPoint(agentTransform.position)).magnitude;
                
                return float.MaxValue;
            }

            public bool IsOccluded(Transform sphereOrigin)
            {
                bool hasHit = Physics.SphereCast(
                    GetPlanePosition(sphereOrigin.position), 
                    0.5f, 
                    GetDirectionToTarget(sphereOrigin), 
                    out RaycastHit hitInfo,
                    GetTransformDistanceFromTarget(sphereOrigin),
                    LayerMask.GetMask("Obstacle", "AlliesAvoidance","InvisibleWall"),
                    QueryTriggerInteraction.Collide
                    );

                return hasHit && !hitInfo.collider.CompareTag("AlliesAvoidancePlayer");
            }

            private Vector3 GetPlanePosition(Vector3 position) => new Vector3(position.x,0f,position.z);

            private void GetPlanePositions(Transform agentTransform, out Vector3 agentPlanePosition, out Vector3 targetPlanePosition)
            {
                agentPlanePosition = GetPlanePosition(agentTransform.position);
                targetPlanePosition = GetPlanePosition(_target.position);
            }
        }
    }
}