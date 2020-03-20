using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO - Dash now is active for all allies, optimize this and avoid using FixedUpdate
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

    private Vector3 startPosition = Vector3.zero;
    private Vector3 moveDirection;

    Vector3 startingVelocity;

    private Rigidbody rb;

    private Controller controller;

    private CombatEventsManager combatEventsManager;

    private bool eventAlreadyCalled = false;

    #endregion

    #region Delegates and events

    public delegate void OnDashStart();
    public event OnDashStart onDashStart;

    public delegate void OnDashStop();
    public event OnDashStop onDashStop;

    #region Methods

    private void RaiseOnDashStart()
    {
        onDashStart?.Invoke();
    }

    private void RaiseOnDashStop()
    {
        onDashStop?.Invoke();
    }
    
    #endregion
    
    #endregion
    
    #region methods

    private void Awake()
    {
        cooldownCounter = 0.0f;
        dashTimeCounter = 0.0f;

        isDashing = false;
        canDash = true;
        playerWantsToDash = false;

        combatEventsManager = this.gameObject.GetComponent<CombatEventsManager>();
    }
    
    private void Start()
    {
        controller = this.GetComponent<Controller>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        DashCycle();
    }

    /// <summary>
    /// Try a Dash in HorizontalDirection + VerticalDirection
    /// </summary>
    /// <param name="verticalDirection"> The y component of the dash </param>
    /// <param name="horizontalDirection"> The x component of the dash </param>
    public void TryDash(float verticalDirection, float horizontalDirection)
    {
        if (!playerWantsToDash) {
            playerWantsToDash = true;
            this.verticalDirection = verticalDirection;
            this.horizontalDirection = horizontalDirection;
        }
    }

    private void DashCycle()
    {
        if (rb != null)
        {
            // Update the cooldown counter
            if (cooldownCounter <= dashCooldown)
            {
                cooldownCounter += Time.deltaTime;
            }

            if (canDash)
            {
                if (playerWantsToDash)
                {
                    // If cooldown has expired...
                    if (cooldownCounter >= dashCooldown)
                    {
                        isDashing = true;

                        // Calculate Dash direction
                        moveDirection = (verticalDirection * Camera.main.transform.forward + horizontalDirection * Camera.main.transform.right);
                        moveDirection.y = 0f;
                        moveDirection = moveDirection.normalized;

                        // Disable controller, player can't move if is dashing
                        controller.enabled = false;

                        // Reset time counters
                        cooldownCounter = 0.0f;
                        dashTimeCounter = 0.0f;

                        // Set rb.interpolation to extrapolate, we need to be precise when moving the player
                        rb.interpolation = RigidbodyInterpolation.Extrapolate;

                        // We need to store the starting velocity
                        startingVelocity = rb.velocity;
                        
                        RaiseOnDashStart();
                    }
                    // Player desire to dash processed
                    playerWantsToDash = false;
                }
                
                if (isDashing)
                {
                    // Dash, will move the player for dashSize units in dashTime seconds
                    rb.velocity = moveDirection.normalized * (dashSize / dashTime);

                    // We done Dashing
                    if (dashTimeCounter >= dashTime)
                    {
                        isDashing = false;
                        controller.enabled = true;
                        
                        // We don't need the extra precision anymore
                        rb.interpolation = RigidbodyInterpolation.None;

                        // We reset the velocity the player had before dashing
                        rb.velocity = startingVelocity;

                        // Reset Dash direction
                        verticalDirection = 0f;
                        horizontalDirection = 0f;
                        
                        RaiseOnDashStop();
                    }

                    // If we count the time down the if condition we round up dash size
                    // If we put this up the if condition we can round down
                    dashTimeCounter += Time.fixedDeltaTime;
                }
            }
        }
    }

    #endregion
}
