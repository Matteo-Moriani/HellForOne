using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        StartCoroutine(RandomDelay());
    }

    private IEnumerator RandomDelay() {
        yield return new WaitForSeconds(Random.Range(0f, 2f));
        StartAnimation();
    }

    private void StartAnimation() {
        AnimationClip animation = GetComponent<AnimationsManager>().GetRandomAnimation();
        //Debug.Log(animation.name);

        switch(animation.name) {
            case "Clapping":
                animator.SetBool("clap", true);
                break;
            case "Excited":
                animator.SetBool("excited", true);
                break;
            case "Rallying":
                animator.SetBool("rally", true);
                break;
            default:
                //Debug.Log("animation missing");
                break;
        }
    }
}
