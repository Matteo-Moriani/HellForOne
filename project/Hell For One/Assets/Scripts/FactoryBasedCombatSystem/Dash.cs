using System;
using System.Collections;
using ActionsBlockSystem;
using CooldownSystem;
using Player;
using ReincarnationSystem;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class Dash : MonoBehaviour, IActionsBlockSubject, IActionsBlockObserver, ICooldown, IReincarnationObserver
    {
        #region Fields

        [SerializeField] private UnitActionsBlockManager.UnitAction[] actionBlocks;
    
        [SerializeField] [Tooltip("")] private float duration = 0.0f;
        [SerializeField] [Tooltip("")] private float size = 0.0f;
        [SerializeField] [Tooltip("")] private float cooldown = 0.0f;

        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _cameraRight = Vector3.zero;
        private Vector3 _cameraForward = Vector3.zero;

        private Transform _mainCamera;

        private Coroutine _dashCoroutine = null;

        private Rigidbody _rigidbody;
        private Cooldowns _cooldowns;
    
        private readonly ActionLock _dashLock = new ActionLock();
    
        #endregion

        #region Events
    
        public event Action OnStartDash;
        public event Action OnStopDash;

        #endregion

        #region Unity methods

        private void Awake()
        {
            _mainCamera = Camera.main.transform;

            _cooldowns = transform.root.GetComponent<Cooldowns>();
            _rigidbody = transform.root.GetComponent<Rigidbody>();
        }

        #endregion

        #region Methods

        private void StartDash()
        {
            if (!_cooldowns.TryAbility(this)) return;
            
            _dashCoroutine = StartCoroutine(DashCoroutine());
            
            OnBlockEvent?.Invoke(actionBlocks);
            OnStartDash?.Invoke();
        }

        private void StopDash()
        {
            if (_dashCoroutine == null) return;

            StopCoroutine(_dashCoroutine);
            _dashCoroutine = null;
            
            OnUnblockEvent?.Invoke(actionBlocks);
            OnStopDash?.Invoke();
        }

        #endregion

        #region Events handlers

        private void OnDashInputDown()
        {
            if(!_dashLock.CanDoAction()) return;
        
            StartDash();
        }

        private void OnMoveInput(Vector2 moveDirection)
        {
            _moveDirection.x = moveDirection.x;
            _moveDirection.z = moveDirection.y;
        }

        #endregion

        #region Coroutines

        private IEnumerator DashCoroutine()
        {
            Vector3 dashDirection;

            // If moveDirection is near 0
            if ((_moveDirection.x <= 0.1f && _moveDirection.x >= -0.1f) &&
                (_moveDirection.z <= 0.1f && _moveDirection.z >= -0.1f))
            {
                dashDirection = transform.root.forward;
            }
            else
            {
                _cameraRight = _mainCamera.right;
                _cameraForward = _mainCamera.forward;

                _cameraForward.y = 0;
                _cameraRight.y = 0;

                _cameraForward.Normalize();
                _cameraRight.Normalize();

                dashDirection = _moveDirection.x * _cameraRight + _moveDirection.z * _cameraForward;
            }

            float dashTimeCounter = 0.0f;

            while (dashTimeCounter < duration)
            {
                // TODO - add gravity
                _rigidbody.velocity = dashDirection * (size / duration);

                dashTimeCounter += Time.fixedDeltaTime;
            
                yield return new WaitForFixedUpdate();
            }

            StopDash();
        }

        #endregion

        #region Interfaces implementations

        public float GetCooldown() => cooldown;

        public void NotifyCooldownStart() { }

        public void NotifyCooldownEnd() { }
    
        public event Action<UnitActionsBlockManager.UnitAction[]> OnBlockEvent;
        public event Action<UnitActionsBlockManager.UnitAction[]> OnUnblockEvent;
    
        public void Block()
        {
            StopDash();

            _dashLock.AddLock();
        }

        public void Unblock() => _dashLock.RemoveLock();

        public UnitActionsBlockManager.UnitAction GetAction() => UnitActionsBlockManager.UnitAction.Dash;

        public void StartLeader()
        {
            PlayerInput.OnDashInputDown += OnDashInputDown;
            PlayerInput.OnMoveInput += OnMoveInput;
        }

        public void StopLeader()
        {
            PlayerInput.OnDashInputDown -= OnDashInputDown;
            PlayerInput.OnMoveInput -= OnMoveInput;
        }
        
        #endregion
    }
}


