using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownArrow : MonoBehaviour
{

    private Animation animation;

    void Start()
    {
        animation = GetComponentInChildren<Animation>();
        animation.Play();
    }
}
