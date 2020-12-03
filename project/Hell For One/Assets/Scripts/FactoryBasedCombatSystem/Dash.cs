using System;
using System.Collections;
using UnityEngine;

// TODO - Dash now is active for all allies, optimize this and avoid using FixedUpdate
// TODO - Look in Reincarnation
namespace FactoryBasedCombatSystem
{
    public class Dash : MonoBehaviour
    {
        #region fields
    
        private float dashSize = 2.5f;
        private float dashCooldown = 1f;
        private float dashTime = 0.35f;

        private float cooldownCounter;
        private float dashTimeCounter;

        private float verticalDirection;
        private float horizontalDirection;

        private bool isDashing;
        private bool canDash;
        private bool playerWantsToDash;

        private int movementNegativeStateEffects = 0;
    
        private Vector3 startPosition = Vector3.zero;
        private Vector3 moveDirection;

        Vector3 startingVelocity;

        private Rigidbody rb;
        private PlayerController playerController;
        private Block block;
        private StunReceiver stunReceiver;
        private KnockbackReceiver knockbackReceiver;

        private Transform mainCameraTransform;

        private WaitForFixedUpdate dashCrWait;

        private Coroutine dashCr = null;
    
        #endregion

        #region Delegates and events
    
        public event Action OnStartDash;
        public event Action OnStopDash;

        #endregion

        #region Unity methods

        private void Awake()
        {
            cooldownCounter = 0.0f;
            dashTimeCounter = 0.0f;

            isDashing = false;
            canDash = true;
            playerWantsToDash = false;
        
            block = GetComponentInChildren<Block>();
            knockbackReceiver = GetComponentInChildren<KnockbackReceiver>();
            stunReceiver = GetComponentInChildren<StunReceiver>();
        
            mainCameraTransform = Camera.main.transform;
        
            dashCrWait = new WaitForFixedUpdate();
        }

        private void OnEnable()
        {
            block.onStartBlock += OnStartBlock;
            block.onStopBlock += OnStopBlock;
            stunReceiver.onStartStun += OnStartStun;
            stunReceiver.onStopStun += OnStopStun;
            knockbackReceiver.onStartKnockback += OnStartKnockback;
            knockbackReceiver.onEndKnockback += OnEndKnockback;
        }

        private void OnDisable()
        {    
            block.onStartBlock -= OnStartBlock;
            block.onStopBlock -= OnStopBlock;
            stunReceiver.onStartStun -= OnStartStun;
            stunReceiver.onStopStun -= OnStopStun;
            knockbackReceiver.onStartKnockback -= OnStartKnockback;
            knockbackReceiver.onEndKnockback -= OnEndKnockback;
        }

        private void Start()
        {
            playerController = this.GetComponent<PlayerController>();
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            // Update the cooldown counter
            if (cooldownCounter <= dashCooldown)
            {
                cooldownCounter += Time.deltaTime;
            }
        
            if (playerWantsToDash)
            {
                playerWantsToDash = false;
                if (canDash)
                {
                    if (dashCr == null)
                    {
                        playerWantsToDash = false;
                        dashCr = StartCoroutine(DashCoroutine());    
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
            if (!playerWantsToDash) {
                playerWantsToDash = true;
                this.verticalDirection = desiredVerticalDirection;
                this.horizontalDirection = desiredHorizontalDirection;
            }
        }

        private void StopDash()
        {
            // We done dashing
            isDashing = false;
            // TODO - Manage OnDash in PlayerController
            playerController.enabled = true;
                        
            // We don't need the extra precision anymore
            rb.interpolation = RigidbodyInterpolation.None;

            // We reset the velocity the player had before dashing
            rb.velocity = startingVelocity;
            
            // Reset Dash direction
            verticalDirection = 0f;
            horizontalDirection = 0f;

            OnStopDash?.Invoke();
        
            dashCr = null;
        }

        private void DisableDash()
        {
            if (isDashing)
            {
                StopDash();
            }

            movementNegativeStateEffects++;
            if (canDash)
            {
                canDash = false;
            }
        }

        private void EnableDash()
        {
            if (movementNegativeStateEffects > 0)
            {
                movementNegativeStateEffects--;    
            }
            else
            {
                Debug.LogError( this.name + " movementNegativeStateEffects is already zero " );
            }

            if (!canDash && movementNegativeStateEffects == 0)
            {
                canDash = true;
            }
        }

        #region Coroutines

        private IEnumerator DashCoroutine()
        {
            isDashing = true;

            // Calculate Dash direction
            moveDirection = (verticalDirection * mainCameraTransform.forward + horizontalDirection * mainCameraTransform.right);
            moveDirection.y = 0f;
            moveDirection = moveDirection.normalized;

            // TODO - Manage OnDash in PlayerController
            // Disable controller, player can't move if is dashing
            playerController.enabled = false;

            // Reset time counters
            cooldownCounter = 0.0f;
            dashTimeCounter = 0.0f;

            // Set rb.interpolation to extrapolate, we need to be precise when moving the player
            rb.interpolation = RigidbodyInterpolation.Extrapolate;

            // We need to store the starting velocity
            startingVelocity = rb.velocity;
            startPosition = transform.position;
        
            OnStartDash?.Invoke();

            // We are dashing
            while (dashTimeCounter < dashTime)
            {
                // Dash, will move the player for dashSize units in dashTime seconds
                rb.velocity = moveDirection.normalized * (dashSize / dashTime);
            
                // If we count the time down the if condition we round up dash size
                // If we put this up the if condition we can round down
                dashTimeCounter += Time.fixedDeltaTime;
            
                yield return dashCrWait;
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

        private void OnStopBlock(Block sender)
        {
            EnableDash();
        }

        private void OnStartBlock(Block sender)
        {
            DisableDash();
        }

        #endregion
    }
}


