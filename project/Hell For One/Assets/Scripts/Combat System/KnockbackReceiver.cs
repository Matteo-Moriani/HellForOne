using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(Stats))]
public class KnockbackReceiver : MonoBehaviour
{
    private bool isProcessingKnockback = false;

    private Coroutine knockBackCR = null;

    private Action onStartKnockback;

    private Action onEndKnockback;

    /// <summary>
    /// Knockback this unit
    /// </summary>
    /// <param name="units">Knockback meters size</param>
    /// <param name="attackerTransform">Transform of the unit that is causing the knockback</param>
    public void TakeKnockBack(float units, Transform attackerTransform, float knockBackSpeed)
    {
        if (!isProcessingKnockback && knockBackCR == null)
            knockBackCR = StartCoroutine(TakeKnockBackCR(units, attackerTransform, knockBackSpeed));
    }

    private IEnumerator TakeKnockBackCR(float AttackerKnockBackSize, Transform attackerTransform, float attackerKnockBackTime)
    {
        RaiseOnStartKnockback();

        // We are processing the KnockBack
        isProcessingKnockback = true;

        float knockBackTimeCounter = 0f;

        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 startingVelocity = rb.velocity;

        if (this.gameObject.GetComponent<Stats>().ThisUnitType == Stats.Type.Player)
        {
            rb.interpolation = RigidbodyInterpolation.Extrapolate;
        }

        // Calculate KnocKback direction 
        Vector3 knockBackDirection = this.transform.position - attackerTransform.position;
        knockBackDirection.y = 0.0f;
        knockBackDirection = knockBackDirection.normalized;

        // KnockBack lerp
        do
        {
            // Use FixedDeltaTimeInstead?
            knockBackTimeCounter += Time.fixedDeltaTime;

            rb.velocity = knockBackDirection * (AttackerKnockBackSize / attackerKnockBackTime);

            // We are using physics so we need to wait for FixedUpdate
            yield return new WaitForFixedUpdate();
        } while (knockBackTimeCounter <= attackerKnockBackTime);

        rb.velocity = startingVelocity;

        if (this.gameObject.GetComponent<Stats>().ThisUnitType == Stats.Type.Player)
        {
            rb.interpolation = RigidbodyInterpolation.None;
        }

        // We are done processing the KnockBack
        isProcessingKnockback = false;

        RaiseOnEndKnockback();

        yield return null;
        knockBackCR = null;
    }

    /// <summary>
    /// Register method to OnStartKnockback event
    /// </summary>
    /// <param name="method">The method to register</param>
    public void RegisterOnStartKnockback(Action method) { 
        onStartKnockback += method;
    }

    /// <summary>
    /// Unregister method to OnStartKnockback event
    /// </summary>
    /// <param name="method">The method to unregister</param>
    public void UnRegisterOnStartKnockback(Action method) { 
        onStartKnockback -= method;    
    }

    /// <summary>
    /// Register method to OnEndKnockback event
    /// </summary>
    /// <param name="method">The method to register</param>
    public void RegisterOnEndKnockback(Action method) { 
        onEndKnockback += method;    
    }

    /// <summary>
    /// Unregister method to OnEndKnockback event
    /// </summary>
    /// <param name="method">The method to unregister</param>
    public void UnRegisterOnEndKnockback(Action method) { 
        onEndKnockback -= method;    
    }

    private void RaiseOnStartKnockback() { 
        if(onStartKnockback != null)
        {
            onStartKnockback();
        }    
    }

    private void RaiseOnEndKnockback() { 
        if(onEndKnockback != null) { 
            onEndKnockback();    
        }    
    }
}
