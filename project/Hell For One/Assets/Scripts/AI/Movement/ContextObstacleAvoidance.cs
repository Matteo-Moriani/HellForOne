using System;
using AI.Imp;
using UnityEngine;

namespace AI.Movement
{
    public class ContextObstacleAvoidance : ContextSteeringBehaviour
    {
        [SerializeField] private float lookAhead = 5f;
        
        [SerializeField] [Range(0, 1f)] private float interestLoseRateo = 0.99f;
        [SerializeField] [Range(0, 1f)] private float dangerLoseRateo = 0.99f;
        [SerializeField] private float k;
        
        [SerializeField] private bool debug = false;

        private ContextSteering _contextSteering;
        private CapsuleCollider _capsuleCollider;
        private Collider _collider;

        private LayerMask _layerMask;
        
        private InterestMap _lastFrameInterest;
        private DangerMap _lastFrameDanger;

        private void Awake()
        {
            _contextSteering = GetComponent<ContextSteering>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _collider = GetComponent<Collider>();
            
            _layerMask = LayerMask.GetMask("Player", "InvisibleWalls", "AlliesAvoidance");
        }
        
        private void Start()
        {
            _lastFrameInterest = new InterestMap(0f, _contextSteering.SteeringResolution);
            _lastFrameDanger = new DangerMap(0f, _contextSteering.SteeringResolution);
        }
        
        public override void GetMaps(out DangerMap dangerMap, out InterestMap interestMap)
        {
            interestMap = new InterestMap(0f, _contextSteering.SteeringResolution);
            dangerMap = new DangerMap(0f, _contextSteering.SteeringResolution);

            Vector3 forward = transform.forward;
            
            for(int i = 0; i < ContextMap.defaultDirections[_contextSteering.SteeringResolution].Length; i++)
            {
                if (!TestDirection(out RaycastHit hitInfo, i)) continue;

                if(hitInfo.transform.root == transform) continue;

                float dot = Mathf.Abs(Vector3.Dot(ContextMap.defaultDirections[_contextSteering.SteeringResolution][i],
                    forward));
                float distance = Vector3.Distance(hitInfo.point, _collider.ClosestPoint(hitInfo.point));
                
                dangerMap.InsertValue(i,Mathf.Min(k / (distance * distance),dot),(int)_contextSteering.SteeringResolution/8);
                interestMap.InsertValue(interestMap.GetOppositeDirection(i),Mathf.Min(k / (distance * distance),dot),(int)_contextSteering.SteeringResolution/8);
                
                // dangerMap.InsertValue(i,Mathf.Max(dot/distance,1f),(int)_contextSteering.SteeringResolution/8);
                // interestMap.InsertValue(dangerMap.GetOppositeDirection(i),Mathf.Max(dot/distance,1f),(int)_contextSteering.SteeringResolution/8);
            }
            
            interestMap = (InterestMap) ContextMap.Combine(interestMap, _lastFrameInterest, interestLoseRateo);
            dangerMap = (DangerMap) ContextMap.Combine(dangerMap, _lastFrameDanger, dangerLoseRateo);

            _lastFrameInterest = interestMap;
            _lastFrameDanger = dangerMap;
            
            if(!debug) return;
            
            interestMap.DebugMap(transform.position);
            dangerMap.DebugMap(transform.position);
        }

        private bool TestDirection(out RaycastHit hitInfo, int i) => 
            Physics.Raycast(
                transform.position, 
                ContextMap.defaultDirections[_contextSteering.SteeringResolution][i], out hitInfo, 
                lookAhead + _capsuleCollider.radius, 
                _layerMask, 
                QueryTriggerInteraction.Collide
            );
    }
}
