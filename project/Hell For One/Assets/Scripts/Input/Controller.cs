using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class Controller : MonoBehaviour
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

    private ImpAnimator animator;

    private void Start()
    {
        combatEventsManager = this.gameObject.GetComponent<CombatEventsManager>();
        animator = GetComponent<ImpAnimator>();
    }

    private void FixedUpdate()
    {
        if(!animator.IsAnimating) {

            if((zMovement != 0 || xMovement != 0)) {
                // Player is walking so we need to raise the event
                if(combatEventsManager != null) {
                    if(!walkEventRaised) {
                        combatEventsManager.RaiseOnStartMoving();
                        walkEventRaised = true;
                    }
                }

                Vector3 vertical = zMovement * Camera.main.transform.forward;
                Vector3 horizontal = xMovement * Camera.main.transform.right;

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

                Quaternion tr = Quaternion.LookRotation(targetDir);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, moveAmount * rotateSpeed);

                transform.rotation = targetRotation;
            }
            else {
                // Player is not walking so we need to raise the event
                if(combatEventsManager != null) {
                    if(walkEventRaised) {
                        combatEventsManager.RaiseOnStartIdle();
                        walkEventRaised = false;
                    }
                }
            }
        }
    }

    public void PassXZValues(float xMovement, float zMovement) { 
        this.zMovement = zMovement;
        this.xMovement = xMovement;
    }
}
