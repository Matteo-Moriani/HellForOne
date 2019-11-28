using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    private bool isAnimating = false;
    private Animator animator;

    public bool IsAnimating { get => isAnimating; set => isAnimating = value; }
    public Animator Animator { get => animator; set => animator = value; }

    public void PlayDeath()
    {

    }

    public void PlayAttack()
    {

    }

    public void PlayRun()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsAnimating )
        {
            Animator.SetBool( "isDying", true );
            IsAnimating = true;
        }
        else if ( Input.GetKeyDown( KeyCode.Space ) && IsAnimating )
        {
            Animator.SetBool( "isDying", false );
            IsAnimating = false;
        }
    }
}
