﻿using System;
using System.Collections.Generic;
using CRBT;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using ReincarnationSystem;
using UnityEngine;

namespace AI.Movement
{
    public class ContextSteering : MonoBehaviour, IReincarnationObserver
    {
        #region Fields
        
        [Header("Context Steering Parameters")]
        
        [SerializeField] private ContextMap.Resolution steeringResolution = ContextMap.Resolution.Medium;
        [SerializeField] private float linearSpeed = 5f;
        [SerializeField] private float angularSpeed = 5f;
        [SerializeField] [Range(0f,1f)] private float newDirectionWeight = 0.25f;
        [SerializeField] private float linearTolerance = 0.8f;
        [SerializeField] [Range(0f,1f)] private float velocityCutOff = 0.1f;
        [SerializeField] [Range(0, 1f)] private float interestLoseRateo = 0.99f;
        [SerializeField] [Range(0, 1f)] private float dangerLoseRateo = 0.99f;
        [SerializeField] private bool debug = false;

        private ContextSteeringBehaviour[] _delegates;

        private readonly List<InterestMap> _interestMaps = new List<InterestMap>();
        private readonly List<DangerMap> _dangerMaps = new List<DangerMap>();

        private InterestMap _lastFrameInterest;
        private DangerMap _lastFrameDanger;
        private Vector3 _lastFrameDirection;
        
        private AiUtils.TargetData _targetData;
        
        private Rigidbody _rigidbody;

        private int _movementLocks = 0;
        private bool _stop = false;

        private CombatSystem _combatSystem;
        private HitPoints _hitPoints;

        public event Action OnStartMove;
        public event Action OnStopMove;
        
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
            _hitPoints = GetComponent<HitPoints>();

            _lastFrameInterest = new InterestMap(0f,steeringResolution);
            _lastFrameDanger = new DangerMap(0f,steeringResolution);
            _lastFrameDirection = transform.forward;
        }

        private void OnEnable()
        {
            _combatSystem.OnStartAttack += OnStartAttack;
            _combatSystem.OnStopAttack += OnStopAttack;
        }

        private void OnDisable()
        {
            _combatSystem.OnStartAttack -= OnStartAttack;
            _combatSystem.OnStopAttack -= OnStopAttack;
        }

        private void FixedUpdate()
        {
            if (_stop)
                return;

            GetMapsFromDelegates();

            ContextMap.Combine(
                out InterestMap currentInterest, out DangerMap currentDanger, 
                _interestMaps, _dangerMaps,
                _lastFrameInterest,_lastFrameDanger,
                interestLoseRateo,dangerLoseRateo);

            Vector3 finalDirection = ContextMap.CalculateDirection(currentInterest, currentDanger);
            Vector3 slerpDirection = Vector3.Slerp(_lastFrameDirection, finalDirection, newDirectionWeight).normalized;

            float dot = Vector3.Dot(slerpDirection, transform.forward);

            if(finalDirection.magnitude > 0)
                _rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(slerpDirection),Time.fixedDeltaTime*angularSpeed));
            else if(_targetData != null && _targetData.Target != null)
                _rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation,Quaternion.LookRotation((_targetData.Target.position - transform.position).normalized),Time.fixedDeltaTime*angularSpeed));      
            
            _rigidbody.velocity = dot >= linearTolerance && finalDirection.magnitude > 0f
                ? slerpDirection * linearSpeed
                : Vector3.zero;

            _interestMaps.Clear();
            _dangerMaps.Clear();

            _lastFrameInterest = currentInterest;
            _lastFrameDanger = currentDanger;
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

        public void ResetTarget()
        {
            _targetData = null;
        }
        
        private void GetMapsFromDelegates()
        {
            foreach (ContextSteeringBehaviour t in _delegates)
            {
                if(!t.IsActive) continue;

                t.GetMaps(out DangerMap tempDanger, out InterestMap tempInterest);
                
                _dangerMaps.Add(tempDanger);
                _interestMaps.Add(tempInterest);
            }
        }
        
        private void UnlockMovement()
        {
            _movementLocks--;
            
            if(_movementLocks > 0)
                return;
            
            _stop = false;
        }

        private void LockMovement()
        {
            _movementLocks++;
            
            if(_movementLocks > 1)
                return;
            
            _stop = true;

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            OnStopMove?.Invoke();
        }
        
        #endregion

        #region Event handlers

        private void OnStartAttack(Attack attack)
        {
            LockMovement();
        }

        private void OnStopAttack()
        {
            UnlockMovement();
        }
        
        private void OnZeroHp()
        {
            LockMovement();
        }

        #endregion

        #region Interfaces

        public void BecomeLeader() => this.enabled = false;

        #endregion
    }
}