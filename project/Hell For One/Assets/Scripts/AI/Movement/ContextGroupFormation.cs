using System;
using ArenaSystem;
using GroupSystem;
using TacticsSystem.Interfaces;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace AI.Movement
{
    // TODO :- Implement ITacticsObserver
    public class ContextGroupFormation : ContextSteeringBehaviour, IGroupObserver, ITacticsObserver
    {
        [SerializeField, Min(0f)] private float stoppingDistance;
        [SerializeField] private float closeness;
        [SerializeField] private float outOfBattleDecenter = 5f;

        [SerializeField] [Range(0, 1f)] private float interestLoseRateo = 0.99f;
        [SerializeField] [Range(0, 1f)] private float dangerLoseRateo = 0.99f;

        [SerializeField] private bool debug;
        
        private ContextSteering _contextSteering;
        private GroupManager _groupManager;

        private InterestMap _lastFrameInterest;
        private DangerMap _lastFrameDanger;

        private float _currentDistance = float.MaxValue;

        private bool _needsUpdate = true;

        private float _currentStoppingDistance;
        private float _currentCloseness;
        private float _currentDecenter;

        private void Awake()
        {
            _contextSteering = GetComponent<ContextSteering>();

            _currentStoppingDistance = stoppingDistance;
            _currentCloseness = closeness;
            _currentDecenter = outOfBattleDecenter;
        }

        private void OnEnable()
        {
            ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        }

        private void OnDisable()
        {
            ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;
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
            
            if(_contextSteering.TargetData.Target == null) return;
            
            // Group Number (4) hardcoded.
            // Group enum has 6 values so we can't use it
            float step = 360f / 4;
            
            Vector3 targetPosition = _contextSteering.TargetData.Target.position + _contextSteering.TargetData.Target.forward * _currentDecenter + 
                                     Quaternion.Euler(0f, step * (int) _groupManager.ThisGroupName, 0f) *
                                    (Vector3.forward * _currentCloseness);
            
            Vector3 toDesiredPosition = (targetPosition - transform.position).normalized;
            _currentDistance = Vector3.Distance(transform.position, targetPosition);

            // TODO :- Find better solution
            _needsUpdate = false;
            
            for(int i = 0; i < ContextMap.defaultDirections[_contextSteering.SteeringResolution].Length; i++)
            {
                float dot = Vector3.Dot(ContextMap.defaultDirections[_contextSteering.SteeringResolution][i],
                        toDesiredPosition);
                if (dot >= 0)
                    interestMap.InsertValue(i, dot * Mathf.Clamp(_currentDistance - _currentStoppingDistance ,0f,1f), (int)_contextSteering.SteeringResolution/8);
            }

            interestMap = (InterestMap) ContextMap.Combine(interestMap, _lastFrameInterest, interestLoseRateo);
            dangerMap = (DangerMap) ContextMap.Combine(dangerMap, _lastFrameDanger, dangerLoseRateo);
            
            _lastFrameInterest = interestMap;
            _lastFrameDanger = dangerMap;

            if(!debug) return;
            
            interestMap.DebugMap(transform.position);
            dangerMap.DebugMap(transform.position);
        }

        public bool InPosition() => !_needsUpdate && _currentDistance - _currentStoppingDistance <= 1f;

        public void JoinGroup(GroupManager groupManager) => _groupManager = groupManager;

        public void LeaveGroup(GroupManager groupManager) => _groupManager = null;
        
        private void OnGlobalEndBattle(ArenaManager obj) => _currentDecenter = outOfBattleDecenter;

        private void OnGlobalStartBattle(ArenaManager obj) => _currentDecenter = 0f;
        
        public void StartTactic(Tactic newTactic)
        {
            _needsUpdate = true;
            
            _currentStoppingDistance = newTactic.GetData().StoppingDistance;
            _currentCloseness = newTactic.GetData().TacticDistance;
        }

        public void EndTactic(Tactic oldTactic)
        {
            _currentStoppingDistance = stoppingDistance;
            _currentCloseness = closeness;
        }
    }
}