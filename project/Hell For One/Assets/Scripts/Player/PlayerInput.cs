using System;
using System.Collections;
using ActionsBlockSystem;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.Interfaces;
using ReincarnationSystem;
using UnityEngine;

namespace Player
{
    public class PlayerInput : MonoBehaviour, IReincarnationObserver
    {
        #region Fields

        private bool _rtPressed;
        private bool _ltPressed;
        
        private Vector2 _currentMoveDirection = Vector2.zero;
        private Vector2 _lastMoveDirection = Vector2.zero;

        private Vector2 _currentLookDirection = Vector2.zero;
        private Vector2 _lastLookDirection = Vector2.zero;

        private readonly ActionLock _inputLock = new ActionLock();
        
        #endregion
        
        #region Delegates and events

        public static event Action OnYButtonDown;
        public static event Action OnXButtonDown;
        public static event Action OnBButtonDown;
        public static event Action OnAButtonDown;
        
        public static event Action OnYButtonUp;
        public static event Action OnXButtonUp;
        public static event Action OnBButtonUp;
        public static event Action OnAButtonUp;
        
        public static event Action OnLTButtonHeldDown;
        public static event Action OnLTButtonUp;
        public static event Action OnLTButtonDown;
        public static event Action OnLT_YButtonDown;
        public static event Action OnLT_XButtonDown;
        public static event Action OnLT_BButtonDown;
        public static event Action OnLT_AButtonDown;

        public static event Action OnCallToArmsInputDown;
        public static event Action OnCallToArmsInputUp;
        
        public static event Action<Vector2> OnMoveInput;
        public static event Action<Vector2> OnRotateInput;
        public static event Action OnDashInputDown;

        #endregion

        private void Awake()
        {
            _inputLock.AddLock();
        }

        private void Update()
        {
            if(!_inputLock.CanDoAction()) return;
            
            ProcessMovementInput();
            
            ProcessRotationInput();
            
            if ( Input.GetButtonDown( "XBoxY" ) )
                OnYButtonDown?.Invoke();
            
            if ( Input.GetButtonDown( "XBoxX" ) )
                OnXButtonDown?.Invoke();
            
            if ( Input.GetButtonDown( "XBoxB" ) )
                OnBButtonDown?.Invoke();
            
            if ( Input.GetButtonDown( "XBoxA" ) )
                OnAButtonDown?.Invoke();
            
            if(Input.GetButtonUp("XBoxY"))
                OnYButtonUp?.Invoke();

            if(Input.GetButtonUp("XBoxX"))
                OnXButtonUp?.Invoke();
            
            if(Input.GetButtonUp("XBoxB"))
                OnBButtonUp?.Invoke();
            
            if(Input.GetButtonUp("XBoxA"))
                OnAButtonUp?.Invoke();
            
            if ( Input.GetAxisRaw( "XBoxLT" ) <= 0.1f && _ltPressed )
            {
                _ltPressed = false;
                OnLTButtonUp?.Invoke();
            }

            if (Input.GetAxisRaw("XBoxLT") >= 0.9f && !_ltPressed)
            {
                _ltPressed = true;
                OnLTButtonDown?.Invoke();
            }
            
            if(Input.GetButtonDown("XBoxRb"))
                OnDashInputDown?.Invoke();

            if(Input.GetButtonDown("XBoxLb"))
                OnCallToArmsInputDown?.Invoke();
            
            if(Input.GetButtonUp("XBoxLb"))
                OnCallToArmsInputUp?.Invoke();
            
            // TODO non funge per ora
            if ( Input.GetAxisRaw( "XBoxLT" ) >= 0.9f && _ltPressed )
            {
                OnLTButtonHeldDown?.Invoke();

                if ( Input.GetButtonDown( "XBoxY" ) )
                {
                    OnLT_YButtonDown?.Invoke();
                }

                if ( Input.GetButtonDown( "XBoxX" ) )
                {
                    OnLT_XButtonDown?.Invoke();
                }

                if ( Input.GetButtonDown( "XBoxB" ) )
                {
                    OnLT_BButtonDown?.Invoke();
                }

                if ( Input.GetButtonDown( "XBoxA" ) )
                {
                    OnLT_AButtonDown?.Invoke();
                }
            }
        }
        
        private void ProcessMovementInput()
        {
            _currentMoveDirection.x = Input.GetAxisRaw("Horizontal");
            _currentMoveDirection.y = Input.GetAxisRaw("Vertical");
        
            if(_currentMoveDirection != _lastMoveDirection)
                OnMoveInput?.Invoke(_currentMoveDirection);

            _lastMoveDirection = _currentMoveDirection;
        }
        
        private void ProcessRotationInput()
        {
            _currentLookDirection.x = Input.GetAxisRaw("XBoxRightStickHorizontal");
            _currentLookDirection.y = Input.GetAxisRaw("XBoxRightStickVertical");

            if(_currentLookDirection != _lastLookDirection)
                OnRotateInput?.Invoke(_currentLookDirection);

            _lastLookDirection = _currentLookDirection;
        }
        
        public void StartLeader() => _inputLock.RemoveLock();

        public void StopLeader() => _inputLock.AddLock();
    }
}
