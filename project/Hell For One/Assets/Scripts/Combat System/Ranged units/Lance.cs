using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lance : MonoBehaviour
{
    [SerializeField, Tooltip("The collisions with objects with a tag in this list don't desable the lance.")]
    private List<string> ignoredTags;

    [SerializeField, Range(-1, 5), Tooltip("The lance remains active after a valid collision for this frame number.")]
    private int numberFrames;

    [SerializeField] private float fixedDuration = 5f;
    
    private int actualFrame;
    private bool deactivates;
    private Stats stats;

    private void OnEnable()
    {
        deactivates = false;
        actualFrame = numberFrames;
        StartCoroutine(DisableCoroutine());
    }

    private void OnDisable()
    {
        // TODO - if you manage to do the effect in OnTriggerEnter, reset Lance here or in Coroutine
        
        StopAllCoroutines();
    }

    private void Start()
    {
        deactivates = false;
        if (ignoredTags == null)
        {
            ignoredTags = new List<string>();
        }
    }

    private void FixedUpdate()
    {
        transform.up = GetComponent<Rigidbody>().velocity;
        if (deactivates)
        {
            actualFrame--;
            if (actualFrame < 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO - put here an effect to stick lances into target
    }
    

    private IEnumerator DisableCoroutine()
    {
        yield return new WaitForSeconds(fixedDuration);
        
        deactivates = true;
        
        // TODO - if you manage to do the effect in OnTriggerEnter, reset Lance here or in OnDisable
    }
}
