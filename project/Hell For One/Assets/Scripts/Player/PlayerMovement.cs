using ActionsBlockSystem;
using ReincarnationSystem;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour, IActionsBlockObserver, IReincarnationObserver
    {
        #region Fields
        
        [SerializeField] private float speed = 2.5f;
        
        private Rigidbody _rb;
        
        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _cameraForward;
        private Vector3 _cameraRight;

        private Transform _mainCameraTransform;

        private readonly ActionLock _movementLock = new ActionLock();

        #endregion

        #region Unity methods

        private void Awake()
        {
            _mainCameraTransform = Camera.main.transform;
    
            _rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            PlayerInput.OnMoveInput += OnMoveInput;
        }

        private void OnDisable()
        {
            PlayerInput.OnMoveInput -= OnMoveInput;
        }

        private void Update()
        {
            // Move direction
            _cameraForward = _mainCameraTransform.forward;
            _cameraRight = _mainCameraTransform.right;

            // Project to horizontal plane
            _cameraForward.y = 0;
            _cameraRight.y = 0;

            _cameraForward = _cameraForward.normalized;
            _cameraRight = _cameraRight.normalized;
        }

        private void FixedUpdate()
        {
            if (!_movementLock.CanDoAction()) return;

            float xComponent = _moveDirection.normalized.x * speed;
            float zComponent = _moveDirection.normalized.z * speed;
            float yComponent = Mathf.Clamp(_rb.velocity.y - 9.8f * Time.fixedDeltaTime,-53,0) ;
    
            _rb.velocity = new Vector3(xComponent,yComponent,zComponent);
        }

        #endregion

        #region Methods

        private void BlockMovement()
        {
            // At first block, set velocity to zero to reset movement
            if(_movementLock.CanDoAction())
                _rb.velocity = Vector3.zero;
    
            _movementLock.AddLock();
        }

        private void UnlockMovement() => _movementLock.RemoveLock();

        #endregion

        #region Events handlers

        private void OnMoveInput(Vector2 moveInput)
        {
            _moveDirection = _cameraRight * moveInput.x + _cameraForward * moveInput.y;
        }

        #endregion

        #region Interface Implementation

        public void Block() => BlockMovement();

        public void Unblock() => UnlockMovement();
        UnitActionsBlockManager.UnitAction IActionsBlockObserver.GetAction() => UnitActionsBlockManager.UnitAction.Move;

        public void BecomeLeader()
        {
            throw new System.NotImplementedException();
        }
        
        #endregion
        
    }
}
