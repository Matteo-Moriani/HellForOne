using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lance : MonoBehaviour
{
    [SerializeField] private float fixedDuration = 5f;
    
    private bool deactivates;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        deactivates = false;
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
    }

    private void FixedUpdate()
    {
        transform.up = rb.velocity;
        if (deactivates)
        {
            gameObject.SetActive(false);
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
