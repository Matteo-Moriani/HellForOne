using System;
using ArenaSystem;
using UnityEngine;

namespace AI.Movement
{
    public class ContextArenaMovement : ContextSteeringBehaviour
    {
        [SerializeField] private float k;
        
        [SerializeField] [Range(0, 1f)] private float interestLoseRateo = 0.99f;
        [SerializeField] [Range(0, 1f)] private float dangerLoseRateo = 0.99f;
        
        [SerializeField] private bool debug;
        
        private InterestMap _lastFrameInterest;
        private DangerMap _lastFrameDanger;
        
        private ContextSteering _contextSteering;
        private ArenaBoss _arenaBoss;

        private void Awake()
        {
            _contextSteering = GetComponent<ContextSteering>();
            _arenaBoss = GetComponent<ArenaBoss>();
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
            
            Vector3 toDesiredPosition = (_arenaBoss.Arena.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, _arenaBoss.Arena.transform.position);

            for(int i = 0; i < ContextMap.defaultDirections[_contextSteering.SteeringResolution].Length; i++)
            {
                float dot = Vector3.Dot(ContextMap.defaultDirections[_contextSteering.SteeringResolution][i],
                    toDesiredPosition);
                if (dot >= 0)
                {
                    //interestMap.InsertValue(i, Mathf.Min((distance * distance) / k,dot), (int)_contextSteering.SteeringResolution/8);
                    dangerMap.InsertValue(dangerMap.GetOppositeDirection(i),Mathf.Min((distance * distance) / k,dot),(int)_contextSteering.SteeringResolution/8);
                }
            }

            //interestMap = (InterestMap) ContextMap.Combine(interestMap, _lastFrameInterest, interestLoseRateo);
            dangerMap = (DangerMap) ContextMap.Combine(dangerMap, _lastFrameDanger, dangerLoseRateo);
            
            _lastFrameInterest = interestMap;
            _lastFrameDanger = dangerMap;

            if(!debug) return;
            
            interestMap.DebugMap(transform.position);
            dangerMap.DebugMap(transform.position);
        }
    }
}