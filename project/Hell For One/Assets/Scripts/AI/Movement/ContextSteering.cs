using System;
using System.Collections.Generic;
using ActionsBlockSystem;
using AI.Imp;
using CRBT;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using GroupSystem;
using ReincarnationSystem;
using TacticsSystem.Interfaces;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace AI.Movement
{
    public class ContextSteering : MonoBehaviour, IReincarnationObserver, IActionsBlockObserver, IGroupObserver, ITacticsObserver
    {
        #region Fields
        
        [Header("Context Steering Parameters")]
        
        [SerializeField] private ContextMap.Resolution steeringResolution = ContextMap.Resolution.Medium;
        [SerializeField] private float linearSpeed = 5f;
        [SerializeField] private float angularSpeed = 5f;
        [SerializeField] [Range(0f,1f)] private float newDirectionWeight = 0.25f;
        [SerializeField] private float linearTolerance = 0.8f;
        [SerializeField] private bool debug = false;

        private ContextSteeringBehaviour[] _delegates;

        private readonly List<InterestMap> _interestMaps = new List<InterestMap>();
        private readonly List<DangerMap> _dangerMaps = new List<DangerMap>();
        
        private Vector3 _lastFrameDirection;
        
        private AiUtils.TargetData _targetData;
        
        private readonly ActionLock _movementLock = new ActionLock();
        
        private Rigidbody _rigidbody;
        private CombatSystem _combatSystem;

        private float _currentLinearSpeed;

        private bool _onStartMovingRaised;
        private bool _onStopMovingRaised;

        private float linearSpeedBeforeStun;

        private Stun bossStun;
        
        #endregion

        #region Events

        public event Action OnStartMoving;
        public event Action OnStopMoving;

        #endregion
        
        #region Properties

        public ContextMap.Resolution SteeringResolution
        {
            get => steeringResolution;
            private set => steeringResolution = value;
        }

        public AiUtils.TargetData TargetData
        {
            get => _targetData;
            private set => _targetData = value;
        }

        #endregion

        #region Unity methods

        private void Awake()
        {
            _delegates = GetComponents<ContextSteeringBehaviour>();
            _rigidbody = GetComponent<Rigidbody>();
            _combatSystem = GetComponentInChildren<CombatSystem>();
            
            _lastFrameDirection = transform.forward;

            _currentLinearSpeed = linearSpeed;

            if ( gameObject.CompareTag( "Boss" ) )
                bossStun = gameObject.GetComponentInChildren<Stun>();
        }

        private void OnEnable()
        {
            if (bossStun != null )
            {
                
            }
        }

        private void OnDisable()
        {
            
        }

        private void FixedUpdate()
        {
            if (!_movementLock.CanDoAction())
            {
                RaiseOnStopMoving();
                
                return;
            }

            GetMapsFromDelegates();

            ContextMap.Combine(out InterestMap currentInterest, out DangerMap currentDanger, _interestMaps, _dangerMaps);

            Vector3 finalDirection = ContextMap.CalculateDirection(currentInterest, currentDanger);
            Vector3 slerpDirection = Vector3.Slerp(_lastFrameDirection, finalDirection, newDirectionWeight).normalized;

            float dot = Vector3.Dot(slerpDirection, transform.forward);

            if(finalDirection.magnitude > 0)
                _rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(slerpDirection),Time.fixedDeltaTime*angularSpeed));
            else if (_targetData != null && _targetData.Target != null)
                _rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(_targetData.GetDirectionToTarget(transform)),Time.fixedDeltaTime * angularSpeed));
            
            _rigidbody.velocity = dot >= linearTolerance && finalDirection.magnitude > 0f
                ? slerpDirection * _currentLinearSpeed
                : Vector3.zero;

            if (_rigidbody.velocity.magnitude > 0f)
            {
                RaiseOnStartMoving();
            }
            else
            {
                RaiseOnStopMoving();
            }

            _interestMaps.Clear();
            _dangerMaps.Clear();
            
            _lastFrameDirection = slerpDirection;
            
            if(!debug)
                return;
            
            currentDanger.DebugMap(transform.position);
            currentInterest.DebugMap(transform.position);
            Debug.DrawRay(transform.position,_lastFrameDirection * 10,Color.black);
        }

        #endregion

        #region Methods

        public void SetTarget(AiUtils.TargetData data)
        {
            _targetData = data;
        }

        private void GetMapsFromDelegates()
        {
            foreach (ContextSteeringBehaviour t in _delegates)
            {
                if(!t.SteeringBehaviourLock.CanDoAction()) continue;

                t.GetMaps(out DangerMap tempDanger, out InterestMap tempInterest);
                
                _dangerMaps.Add(tempDanger);
                _interestMaps.Add(tempInterest);
            }
        }

        private void RaiseOnStartMoving()
        {
            if(_onStartMovingRaised) return;

            _onStartMovingRaised = true;
            _onStopMovingRaised = false;
            
            OnStartMoving?.Invoke();
        }

        private void RaiseOnStopMoving()
        {
            if(_onStopMovingRaised) return;

            _onStopMovingRaised = true;
            _onStartMovingRaised = false;
            
            OnStopMoving?.Invoke();
        }

        #endregion

        #region Event handlers
        
        #endregion

        #region Interfaces

        public void Reincarnate() => this.enabled = false;

        public void Block()
        {
            if (_movementLock.CanDoAction())
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
            
            _movementLock.AddLock();
        }

        public void Unblock() => _movementLock.RemoveLock();

        public UnitActionsBlockManager.UnitAction GetAction() => UnitActionsBlockManager.UnitAction.Move;

        public void JoinGroup(GroupManager groupManager) => _targetData = groupManager.GetComponent<ImpGroupAi>().Target;

        public void LeaveGroup(GroupManager groupManager) => _targetData = null;

        public void StartTactic(Tactic newTactic) => _currentLinearSpeed = newTactic.GetData().TacticSpeed;

        public void EndTactic(Tactic oldTactic) => _currentLinearSpeed = linearSpeed;

        #endregion

        public void StartLeader() => this.enabled = false;

        public void StopLeader() { }
    }
}