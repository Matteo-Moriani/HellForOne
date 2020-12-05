using System;
using System.Collections;
using ActionsBlockSystem;
using Player;
using UnityEngine;

// TODO - Dash now is active for all allies, optimize this and avoid using FixedUpdate
// TODO - Look in Reincarnation
namespace FactoryBasedCombatSystem
{
    public class Dash : MonoBehaviour, IActionsBlockSubject
    {
        #region fields

        [SerializeField] private UnitActionsBlockManager.UnitAction[] actionBlocks;
        
        private float _dashSize = 2.5f;
        private float _dashCooldown = 1f;
        private float _dashTime = 0.35f;

        private float _cooldownCounter;
        private float _dashTimeCounter;

        private float _verticalDirection;
        private float _horizontalDirection;

        private bool _isDashing;
        private bool _canDash;
        private bool _playerWantsToDash;

        private int _movementNegativeStateEffects = 0;
    
        private Vector3 _startPosition = Vector3.zero;
        private Vector3 _moveDirection;

        Vector3 _startingVelocity;

        private Rigidbody _rb;
        private PlayerMovement _playerMovement;
        private StunReceiver _stunReceiver;
        private KnockbackReceiver _knockbackReceiver;

        private Transform _mainCameraTransform;

        private WaitForFixedUpdate _dashCrWait;

        private Coroutine _dashCr = null;
    
        #endregion

        #region Delegates and events
    
        public event Action OnStartDash;
        public event Action OnStopDash;

        #endregion

        #region Unity methods

        private void Awake()
        {
            _cooldownCounter = 0.0f;
            _dashTimeCounter = 0.0f;

            _isDashing = false;
            _canDash = true;
            _playerWantsToDash = false;
            
            _knockbackReceiver = GetComponentInChildren<KnockbackReceiver>();
            _stunReceiver = GetComponentInChildren<StunReceiver>();
        
            _mainCameraTransform = Camera.main.transform;
        
            _dashCrWait = new WaitForFixedUpdate();
        }

        private void OnEnable()
        {
            _stunReceiver.onStartStun += OnStartStun;
            _stunReceiver.onStopStun += OnStopStun;
            _knockbackReceiver.onStartKnockback += OnStartKnockback;
            _knockbackReceiver.onEndKnockback += OnEndKnockback;
        }

        private void OnDisable()
        {
            _stunReceiver.onStartStun -= OnStartStun;
            _stunReceiver.onStopStun -= OnStopStun;
            _knockbackReceiver.onStartKnockback -= OnStartKnockback;
            _knockbackReceiver.onEndKnockback -= OnEndKnockback;
        }

        private void Start()
        {
            _playerMovement = this.GetComponent<PlayerMovement>();
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            // Update the cooldown counter
            if (_cooldownCounter <= _dashCooldown)
            {
                _cooldownCounter += Time.deltaTime;
            }
        
            if (_playerWantsToDash)
            {
                _playerWantsToDash = false;
                if (_canDash)
                {
                    if (_dashCr == null)
                    {
                        _playerWantsToDash = false;
                        _dashCr = StartCoroutine(DashCoroutine());    
                    }        
                }
            }
        }
    
        #endregion
    
        #region methods
    
        /// <summary>
        /// Try a Dash in HorizontalDirection + VerticalDirection
        /// </summary>
        /// <param name="desiredVerticalDirection"> The y component of the dash </param>
        /// <param name="desiredHorizontalDirection"> The x component of the dash </param>
        public void TryDash(float desiredVerticalDirection, float desiredHorizontalDirection)
        {
            if (!_playerWantsToDash) {
                _playerWantsToDash = true;
                this._verticalDirection = desiredVerticalDirection;
                this._horizontalDirection = desiredHorizontalDirection;
            }
        }

        private void StopDash()
        {
            // We done dashing
            _isDashing = false;
            // TODO - Manage OnDash in PlayerController
            _playerMovement.enabled = true;
                        
            // We don't need the extra precision anymore
            _rb.interpolation = RigidbodyInterpolation.None;

            // We reset the velocity the player had before dashing
            _rb.velocity = _startingVelocity;
            
            // Reset Dash direction
            _verticalDirection = 0f;
            _horizontalDirection = 0f;

            OnStopDash?.Invoke();
        
            _dashCr = null;
        }

        private void DisableDash()
        {
            if (_isDashing)
            {
                StopDash();
            }

            _movementNegativeStateEffects++;
            if (_canDash)
            {
                _canDash = false;
            }
        }

        private void EnableDash()
        {
            if (_movementNegativeStateEffects > 0)
            {
                _movementNegativeStateEffects--;    
            }
            else
            {
                Debug.LogError( this.name + " movementNegativeStateEffects is already zero " );
            }

            if (!_canDash && _movementNegativeStateEffects == 0)
            {
                _canDash = true;
            }
        }

        #region Coroutines

        private IEnumerator DashCoroutine()
        {
            _isDashing = true;

            // Calculate Dash direction
            _moveDirection = (_verticalDirection * _mainCameraTransform.forward + _horizontalDirection * _mainCameraTransform.right);
            _moveDirection.y = 0f;
            _moveDirection = _moveDirection.normalized;

            // TODO - Manage OnDash in PlayerController
            // Disable controller, player can't move if is dashing
            _playerMovement.enabled = false;

            // Reset time counters
            _cooldownCounter = 0.0f;
            _dashTimeCounter = 0.0f;

            // Set rb.interpolation to extrapolate, we need to be precise when moving the player
            _rb.interpolation = RigidbodyInterpolation.Extrapolate;

            // We need to store the starting velocity
            _startingVelocity = _rb.velocity;
            _startPosition = transform.position;
        
            OnStartDash?.Invoke();

            // We are dashing
            while (_dashTimeCounter < _dashTime)
            {
                // Dash, will move the player for dashSize units in dashTime seconds
                _rb.velocity = _moveDirection.normalized * (_dashSize / _dashTime);
            
                // If we count the time down the if condition we round up dash size
                // If we put this up the if condition we can round down
                _dashTimeCounter += Time.fixedDeltaTime;
            
                yield return _dashCrWait;
            }

            StopDash();
        }

        #endregion
    
        #endregion

        #region External events handlers

        private void OnEndKnockback(KnockbackReceiver sender)
        {
            EnableDash();
        }

        private void OnStartKnockback(KnockbackReceiver sender)
        {
            DisableDash();
        }

        private void OnStopStun()
        {
            EnableDash();
        }

        private void OnStartStun()
        {
            DisableDash();
        }

        #endregion

        #region Interfaces

        public event Action<UnitActionsBlockManager.UnitAction[]> OnBlockEvent;
        public event Action<UnitActionsBlockManager.UnitAction[]> OnUnblockEvent;

        #endregion
    }
}


