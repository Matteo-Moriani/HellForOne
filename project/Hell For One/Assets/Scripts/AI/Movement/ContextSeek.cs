using UnityEngine;

namespace AI.Movement
{
    public class ContextSeek : ContextSteeringBehaviour
    {
        [SerializeField, Min(0f)] private float stoppingDistance = 5f;

        [SerializeField] [Range(0, 1f)] private float interestLoseRateo = 0.99f;
        [SerializeField] [Range(0, 1f)] private float dangerLoseRateo = 0.99f;
        
        [SerializeField] private bool debug = false;

        private Transform _mTransform;

        private ContextSteering _contextSteering;

        private InterestMap _lastFrameInterest;
        private DangerMap _lastFrameDanger;
        
        private void Awake()
        {
            _contextSteering = GetComponent<ContextSteering>();
            
            _mTransform = transform;
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
            
            if (_contextSteering.TargetData == null) return;

            Vector3 toTarget = _contextSteering.TargetData.GetDirectionToTarget(_mTransform);
            float distance = _contextSteering.TargetData.GetTransformDistanceFromTarget(transform);

            for (int i = 0; i < ContextMap.defaultDirections[_contextSteering.SteeringResolution].Length; i++)
            {
                float dot = Vector3.Dot(ContextMap.defaultDirections[_contextSteering.SteeringResolution][i], toTarget);

                if (dot >= 0)
                    interestMap.InsertValue(i, dot * Mathf.Clamp(distance - stoppingDistance ,0f,1f), (int)_contextSteering.SteeringResolution/8);
            }
            
            interestMap = (InterestMap) ContextMap.Combine(interestMap, _lastFrameInterest, interestLoseRateo);

            _lastFrameInterest = interestMap;
            
            if (!debug) return;
            
            interestMap.DebugMap(transform.position);
            dangerMap.DebugMap(transform.position);
        }
    }
}