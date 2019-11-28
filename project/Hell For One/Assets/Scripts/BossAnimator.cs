using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    private bool isAnimating = false;
    private Animator animator;

    public bool IsAnimating { get => isAnimating; set => isAnimating = value; }
    public Animator Animator { get => animator; set => animator = value; }

    public enum Animations
    {
        Death,
        Attack,
        Run,
        Idle
    }

    public void PlayAnimation(Animations animation )
    {
        switch (animation)
        {
            case Animations.Death:
                Animator.SetBool( "isDying", true );
                break;
            case Animations.Attack:
                Animator.SetBool( "isAttacking", true );
                break;
            case Animations.Run:
                Animator.SetBool( "isRunning", true );
                break;
            case Animations.Idle:
                Animator.SetBool( "isidle", true );
                break;
        }

        IsAnimating = true;
    }

    public void StopAnimations()
    {
        Animator.SetBool( "isDying", false );
        Animator.SetBool( "isAttacking", false );
        Animator.SetBool( "isRunning", false );
        Animator.SetBool( "isidle", false );

        IsAnimating = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) && !IsAnimating )
        //{
        //    Animator.SetBool( "isDying", true );
        //    IsAnimating = true;
        //}
        //else if ( Input.GetKeyDown( KeyCode.Space ) && IsAnimating )
        //{
        //    Animator.SetBool( "isDying", false );
        //    IsAnimating = false;
        //}
    }
}
