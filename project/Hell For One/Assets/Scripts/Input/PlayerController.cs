using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class PlayerController : MonoBehaviour
{
    [Header( "Input" )]
    [SerializeField]
    private float zMovement, xMovement;
    private float moveAmount, moveDir;

    [Header( "Stats" )]
    [SerializeField]
    private float rotateSpeed = 5f;
    [SerializeField]
    private float movingSpeed = 10f;

    private CombatEventsManager combatEventsManager;
    private bool walkEventRaised = false;

    public float ZMovement { get => zMovement; set => zMovement = value; }
    public float XMovement { get => xMovement; set => xMovement = value; }

    #region Fields

    private bool hasAlreadyMoved = false;
    private bool hasAlreadyStopped = false;
    private bool canMove = true;

    private int movementNegativeStateEffects = 0;
    
    private ImpAnimator animator;
    private StunReceiver stunReceiver;
    private KnockbackReceiver knockbackReceiver;

    private Transform mainCameraTransform;
    
    private Quaternion tr;
    private Quaternion targetRotation;

    #endregion
    
    #region Delegates and events

    public delegate void OnPlayerStartMoving();
    public event OnPlayerStartMoving onPlayerStartMoving;

    public delegate void OnPlayerEndMoving();
    public event OnPlayerEndMoving onPlayerEndMoving;

    #region Methods

    private void RaiseOnPlayerStartMoving()
    {
        onPlayerStartMoving?.Invoke();
    }

    private void RaiseOnPlayerEndMoving()
    {
        onPlayerEndMoving?.Invoke();
    }

    #endregion
    
    #endregion
    
    #region Unity methods

    private void Awake()
    {
        stunReceiver = GetComponentInChildren<StunReceiver>();
        knockbackReceiver = GetComponentInChildren<KnockbackReceiver>();
        mainCameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        stunReceiver.onStartStun += OnStartStun;
        stunReceiver.onStopStun += OnStopStun;

        knockbackReceiver.onStartKnockback += OnStartKnockback;
        knockbackReceiver.onEndKnockback += OnEndKnockback;
    }
    
    private void OnDisable()
    {    
        stunReceiver.onStartStun -= OnStartStun;
        stunReceiver.onStopStun -= OnStopStun;

        knockbackReceiver.onStartKnockback -= OnStartKnockback;
        knockbackReceiver.onEndKnockback -= OnEndKnockback;
    }

    private void Start()
    {
        combatEventsManager = this.gameObject.GetComponent<CombatEventsManager>();
        animator = GetComponent<ImpAnimator>();
    }

    private void FixedUpdate()
    {
        // TODO - Check this
        if (animator.IsAnimating) return;

        if (!canMove) return;
        
        if(!zMovement.Equals(0) || !xMovement.Equals(0)) {

            if (!hasAlreadyMoved)
            {
                hasAlreadyMoved = true;
                hasAlreadyStopped = false;
                RaiseOnPlayerStartMoving();    
            }
                
            Vector3 vertical = zMovement * mainCameraTransform.forward;
            Vector3 horizontal = xMovement * mainCameraTransform.transform.right;

            Vector3 moveDir = (vertical + horizontal).normalized;

            // Problem of inabyssing
            moveDir.y = 0f;

            float m = Mathf.Abs(zMovement) + Mathf.Abs(xMovement);
            moveAmount = Mathf.Clamp01(m);

            transform.position += moveDir.normalized * movingSpeed * Time.deltaTime;

            Vector3 targetDir = moveDir;
            targetDir.y = 0f;

            if(targetDir == Vector3.zero)
                targetDir = Vector3.forward;

            tr = Quaternion.LookRotation(targetDir);
            targetRotation = Quaternion.Slerp(transform.rotation, tr, moveAmount * rotateSpeed);

            transform.rotation = targetRotation;
        }
        else {
            if (!hasAlreadyStopped)
            {
                hasAlreadyMoved = false;
                hasAlreadyStopped = true;
                RaiseOnPlayerEndMoving();
            }
        }
    }
    
    #endregion

    #region External events handlers

    private void OnEndKnockback(KnockbackReceiver sender)
    {
        movementNegativeStateEffects--;
        if (!canMove && movementNegativeStateEffects == 0)
        {
            canMove = true;
        }
    }

    private void OnStartKnockback(KnockbackReceiver sender)
    {
        movementNegativeStateEffects++;
        if (canMove)
        {
            canMove = false;
        }
    }

    private void OnStopStun()
    {
        movementNegativeStateEffects--;
        if (!canMove && movementNegativeStateEffects == 0)
        {
            canMove = true;
        }
    }

    private void OnStartStun()
    {
        movementNegativeStateEffects++;
        if (canMove)
        {
            canMove = false;
        }
    }


    #endregion
    
    public void PassXZValues(float xMovement, float zMovement) { 
        this.zMovement = zMovement;
        this.xMovement = xMovement;
    }
}
