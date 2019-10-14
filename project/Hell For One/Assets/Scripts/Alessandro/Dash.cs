using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// -TODO- No Damage Frames
// -TODO- Cash position

public class Dash : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How far the palyer will get")]
    private float dashSize = 5.0f;
    [SerializeField]
    [Tooltip("How fast the dash will be")]
    private float dashSpeed = 5.0f;
    [SerializeField]
    [Tooltip("How often we can dash")]
    private float dashCooldown = 1.0f;
    [SerializeField]
    [Tooltip("How close we have to get to target position before we can start moving again")]
    private float tollerance = 0.5f;
    
    private float cooldownCounter;
    private float lerpTimer;
    private bool isDashing;
    private Vector3 targetPosition;

    private Controller controller;

    private void Awake()
    {
        cooldownCounter = 0.0f;
        lerpTimer = 0.0f;
        
        isDashing = false;
    }

    private void Start()
    {
        controller = this.GetComponent<Controller>();
    }

    private void Update()
    {
        DashCycle();
    }

    /// <summary>
    /// Method that manages the dash cycle
    /// </summary>
    private void DashCycle() {
        // Update the cooldown counter
        if (cooldownCounter <= dashCooldown)
        {
            cooldownCounter += Time.deltaTime;
        }

        // If we hit Circle or Space and cooldown has expired...
        if ((Input.GetKeyDown("space") || Input.GetButton("Fire3")) && cooldownCounter >= dashCooldown)
        {
            // Move the player of dashSize units into our input axis direction.
            targetPosition = this.transform.position + (new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")) * dashSize);

            isDashing = true;
            controller.enabled = false;
            cooldownCounter = 0.0f;
            lerpTimer = 0.0f;
        }

        if (isDashing)
        {
            // Update lerp timer.
            lerpTimer += Time.deltaTime;
            
            // Lerp from starting position to target position by the interpolant lerpTimer and by a factor of dashSpeed
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, lerpTimer * dashSpeed);
            
            // If we reach our destination we stop lerping
            if  (    Mathf.Abs(this.transform.position.x ) >= 
                    ( Mathf.Abs(targetPosition.x) - Mathf.Abs(tollerance) ) && 
                    Mathf.Abs(this.transform.position.z) >= 
                    (Mathf.Abs(targetPosition.z) - Mathf.Abs(tollerance))
                )
            {
                isDashing = false;
                controller.enabled = true;
            }
        }
    }
}
