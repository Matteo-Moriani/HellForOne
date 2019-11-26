using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// -TODO- No Damage Frames
// -TODO- Cash position
public class Dash : MonoBehaviour
{
    #region fields

    [SerializeField]
    [Tooltip("How far the palyer will get")]
    private float dashSize = 5.0f;

    [SerializeField]
    [Tooltip("How often we can dash")]
    private float dashCooldown = 1.0f;

    [SerializeField]
    private GameObject idleCollider;

    [SerializeField]
    private float dashTime = 0.25f;

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

    #endregion

    #region methods

    private void Awake()
    {
        cooldownCounter = 0.0f;
        dashTimeCounter = 0.0f;

        isDashing = false;
        canDash = true;
        playerWantsToDash = false;
    }

    private void Start()
    {
        controller = this.GetComponent<Controller>();

        if (idleCollider == null)
        {
            Debug.Log("Dash.cs - Set idleCollider");
        }

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
        playerWantsToDash = true;
        this.verticalDirection = verticalDirection;
        this.horizontalDirection = horizontalDirection;
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

                        // Disable idleCollider, player can't receive damage if is dashing
                        idleCollider.SetActive(false);

                        // Set rb.interpolation to extrapolate, we need to be precise when moving the player
                        rb.interpolation = RigidbodyInterpolation.Extrapolate;

                        // We need to store the starting velocity
                        startingVelocity = rb.velocity;
                    }
                    
                    // Player desire to dash processed
                    playerWantsToDash = false;
                }


                if (isDashing)
                {
                    /*
                    // TODO - Remove this after testing
                    // Used for logging
                    if (startPosition == Vector3.zero) {
                        startPosition = this.transform.position;
                        Debug.Log("You started dashing at: " + this.transform.position);
                    }
                    */

                    // Dash, will move the player for dashSize units in dashTime seconds
                    rb.velocity = moveDirection.normalized * (dashSize / dashTime);

                    // We done Dashing
                    if (dashTimeCounter >= dashTime)
                    {
                        isDashing = false;
                        controller.enabled = true;

                        // Now player can receive damage
                        idleCollider.SetActive(true);


                        // We don't need the extra precision anymore
                        rb.interpolation = RigidbodyInterpolation.None;

                        // We reset the velocity the player had before dashing
                        rb.velocity = startingVelocity;

                        // Reset Dash direction
                        verticalDirection = 0f;
                        horizontalDirection = 0f;

                        // TODO - Remove this after testing
                        // Used for logging
                        Debug.Log("You ended dashing at. " + this.transform.position);
                        Debug.Log("You moved: " + Vector3.Distance(startPosition, this.transform.position));
                        Debug.Log("You should have moved: " + dashSize);
                        startPosition = Vector3.zero;
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
