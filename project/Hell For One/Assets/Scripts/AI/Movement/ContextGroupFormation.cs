﻿using System;
using AI.Imp;
using Groups;
using UnityEngine;

namespace AI.Movement
{
    public class ContextGroupFormation : ContextSteeringBehaviour
    {
        [SerializeField, Min(0f)] private float stoppingDistance;
        [SerializeField] private float closeness;

        [SerializeField] [Range(0, 1f)] private float interestLoseRateo = 0.99f;
        [SerializeField] [Range(0, 1f)] private float dangerLoseRateo = 0.99f;

        [SerializeField] private bool debug;
        
        private ContextSteering _contextSteering;
        private GroupManager _groupManager;
        private GroupFinder _groupFinder;

        private InterestMap _lastFrameInterest;
        private DangerMap _lastFrameDanger;
        
        private void Awake()
        {
            _contextSteering = GetComponent<ContextSteering>();
            _groupFinder = GetComponent<GroupFinder>();
        }

        private void OnEnable()
        {
            _groupFinder.OnGroupFound += OnGroupFound;
        }

        private void OnDisable()
        {
            _groupFinder.OnGroupFound -= OnGroupFound;
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

            if(_groupManager == null) return;

            float step = 360f / Enum.GetValues(typeof(GroupManager.Group)).Length;
            Vector3 targetPosition = _contextSteering.TargetData.Target.position + 
                                     Quaternion.Euler(0f, step * (int) _groupManager.ThisGroupName, 0f) *
                                    (_contextSteering.TargetData.Target.forward * closeness);
            
            Vector3 toDesiredPosition = (targetPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPosition);

            for(int i = 0; i < ContextMap.defaultDirections[_contextSteering.SteeringResolution].Length; i++)
            {
                float dot = Vector3.Dot(ContextMap.defaultDirections[_contextSteering.SteeringResolution][i],
                        toDesiredPosition);
                if (dot >= 0)
                    interestMap.InsertValue(i, dot * Mathf.Clamp(distance - stoppingDistance ,0f,1f), (int)_contextSteering.SteeringResolution/8);
            }

            interestMap = (InterestMap) ContextMap.Combine(interestMap, _lastFrameInterest, interestLoseRateo);
            dangerMap = (DangerMap) ContextMap.Combine(dangerMap, _lastFrameDanger, dangerLoseRateo);
            
            _lastFrameInterest = interestMap;
            _lastFrameDanger = dangerMap;

            if(!debug) return;
            
            interestMap.DebugMap(transform.position);
            dangerMap.DebugMap(transform.position);
        }
        
        private void OnGroupFound(Transform groupTransform)
        {
            _groupManager = groupTransform.GetComponent<GroupManager>();
        }
    }
}