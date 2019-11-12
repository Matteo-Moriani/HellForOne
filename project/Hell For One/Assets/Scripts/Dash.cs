﻿using System.Collections;
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
    private float tollerance = 0.1f;
    [SerializeField]
    private GameObject idleCollider;
    
    private float cooldownCounter;
    private float lerpTimer;
    
    private bool isDashing;
    private bool canDash;
    
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private Vector3 moveDirection;

    private Controller controller;

    private void Awake()
    {
        cooldownCounter = 0.0f;
        lerpTimer = 0.0f;
        
        isDashing = false;
        canDash = true;
    }

    private void Start()
    {
        controller = this.GetComponent<Controller>();
        
        if(idleCollider == null) {
            Debug.Log("Dash.cs - Set idleCollider");   
        }
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {   /*
        if (Input.GetKeyDown("space"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            Vector3 moveDirection = (Input.GetAxis("Vertical") * Camera.main.transform.forward + Input.GetAxis("Horizontal") * Camera.main.transform.right).normalized;
            targetPosition = this.transform.position + (moveDirection * dashSize);
            rb.MovePosition(targetPosition);
            //rb.AddForce((Input.GetAxis("Vertical") * Camera.main.transform.forward + Input.GetAxis("Horizontal") * Camera.main.transform.right).normalized*dashSize,ForceMode.VelocityChange); 
        }
        //DashCycle();
        //DashUsingPhysics();
        */
        DashCycle();
    }
    /*
    private void DashUsingPhysics() { 
        Rigidbody rb = GetComponent<Rigidbody>();

        //get starting velocity
        //reset after
        if (Input.GetKeyDown("space")) {
            Vector3 moveDirection = (Input.GetAxis("Vertical") * Camera.main.transform.forward + Input.GetAxis("Horizontal") * Camera.main.transform.right).normalized;
            targetPosition = this.transform.position + (moveDirection * dashSize);
            rb.MovePosition(targetPosition);
            //rb.AddForce((Input.GetAxis("Vertical") * Camera.main.transform.forward + Input.GetAxis("Horizontal") * Camera.main.transform.right).normalized*dashSize,ForceMode.VelocityChange); 
        }
    }
    */
    
    

    
    /// <summary>
    /// Method that manages the dash cycle
    /// </summary>
    private void DashCycle() {
        // Update the cooldown counter
        if (cooldownCounter <= dashCooldown)
        {
            cooldownCounter += Time.deltaTime;
        }

        if (canDash) {
            Rigidbody rb = GetComponent<Rigidbody>();

            // If we hit Circle or Space and cooldown has expired...
            if ((Input.GetKeyDown("space") || Input.GetButton("Fire3")) && cooldownCounter >= dashCooldown)
            {
                // Move the player of dashSize units into our input axis direction.
                moveDirection = (Input.GetAxis("Vertical") * Camera.main.transform.forward + Input.GetAxis("Horizontal") * Camera.main.transform.right).normalized;
                //moveDirection.y = 0f;

                startPosition = this.transform.position;
                targetPosition = this.transform.position + (moveDirection * dashSize);

                isDashing = true;
                controller.enabled = false;
                cooldownCounter = 0.0f;
                lerpTimer = 0.0f;

                idleCollider.SetActive(false);
            }

            if (isDashing)
            {
                // Update lerp timer.
                lerpTimer += Time.deltaTime;
                
                // Lerp from starting position to target position by the interpolant lerpTimer and by a factor of dashSpeed
                // TODO-subtract lerp delta if player cant move to new position to fix boss bug?
                //this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, lerpTimer * dashSpeed);
                //rb.position = Vector3.Lerp(startPosition, targetPosition, lerpTimer * dashSpeed);

                rb.MovePosition(this.transform.position + moveDirection * dashSize * lerpTimer * dashSpeed);// + moveDirection * dashSize * lerpTimer * dashSpeed);
                // If we reach our destination (with some tollerance) we stop lerping
                //if (Vector3.Distance(this.transform.position, targetPosition) == 0f)
                if (lerpTimer * dashSpeed >= 1)
                {
                    isDashing = false;
                    controller.enabled = true;

                    idleCollider.SetActive(true);
                }
            }
        } 
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        canDash = false;

        if (isDashing)
        {
            isDashing = false;
            controller.enabled = true;

            idleCollider.SetActive(true);

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        canDash = true;  
    }
    */
}
