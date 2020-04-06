using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{
    #region Fields

    private bool isProcessingKnockback = false;

    private Coroutine knockBackCR = null;

    private Block block;
    private IdleCombat idleCombat;
    private Stats.Type unitType;
    private Reincarnation reincarnation;

    #endregion
    
    #region Delegates and events

    public delegate void OnStartKnockback(KnockbackReceiver sender);
    public event OnStartKnockback onStartKnockback;

    public delegate void OnEndKnockback(KnockbackReceiver sender);
    public event OnEndKnockback onEndKnockback;

    #region Methods

    private void RaiseOnStartKnockback()
    {
        onStartKnockback?.Invoke(this);
    }

    private void RaiseOnEndKnockback()
    {
        onEndKnockback?.Invoke(this);
    }

    #endregion
    
    #endregion

    #region Unity methods

    private void Awake()
    {
        block = gameObject.GetComponent<Block>();
        idleCombat = gameObject.GetComponent<IdleCombat>();
        unitType = transform.root.GetComponent<Stats>().ThisUnitType;
        reincarnation = transform.root.GetComponent<Reincarnation>();
    }

    private void OnEnable()
    {
        if (block != null)
        {
            block.onBlockFailed += OnBlockFailed;
            block.onBlockSuccess += OnBlockSuccess;
        }
        else
        {
            Debug.LogError(transform.root.name + " " + this.name + " cannot find Block");
        }
        if (idleCombat != null)
        {
            idleCombat.onAttackBeingHit += OnAttackBeingHit;
        }
        else
        {
            Debug.LogError(transform.root.name + " " + this.name + " cannot find IdleCombat");
        }

        if(reincarnation != null)
            reincarnation.onLateReincarnation += OnLateReincarnation;
    }

    private void OnDisable()
    {
        if (block != null)
        {
            block.onBlockFailed -= OnBlockFailed;
            block.onBlockSuccess -= OnBlockSuccess;
        }
        else
        {
            Debug.LogError(transform.root.name + " " + this.name + " cannot find Block");
        }
        if (idleCombat != null)
        {
            idleCombat.onAttackBeingHit -= OnAttackBeingHit;
        }
        else
        {
            Debug.LogError(transform.root.name + " " + this.name + " cannot find IdleCombat");
        }

        if(reincarnation != null)
            reincarnation.onLateReincarnation -= OnLateReincarnation;
    }

    #endregion
    
    #region Methods

    /// <smmary>
    /// Knockback this unit
    /// </summary>
    /// <param name="units">Knockback meters size</param>
    /// <param name="attackerTransform">Transform of the unit that is causing the knockback</param>
    private void TakeKnockBack(KnockbackCaster knockbackCaster)
    {
        if (!isProcessingKnockback && knockBackCR == null)
            knockBackCR = StartCoroutine(TakeKnockBackCR(knockbackCaster));
    }

    #endregion

    #region External events handlers

    private void OnLateReincarnation(GameObject newPlayer)
    {
        unitType = newPlayer.GetComponent<Stats>().ThisUnitType;
    }

    private void OnBlockFailed(Block sender, Attack attack, NormalCombat attackerNormalCombat)
    {
        if (attack.CauseKnockback)
        {
            KnockbackCaster knockbackCaster = attackerNormalCombat.gameObject.GetComponent<KnockbackCaster>();

            if (knockbackCaster != null)
            {
                TakeKnockBack(knockbackCaster);
            }
        }
    }

    private void OnBlockSuccess(Block sender, Attack attack, NormalCombat attackerNormalCombat)
    {
        if (attack.CauseKnockbackWhenBlocked && unitType == Stats.Type.Player)
        {
            KnockbackCaster knockbackCaster = attackerNormalCombat.gameObject.GetComponent<KnockbackCaster>();

            if (knockbackCaster != null)
            {
                TakeKnockBack(knockbackCaster);
            }
        }
    }

    private void OnAttackBeingHit(IdleCombat sender, Attack attack, NormalCombat attackerNormalCombat)
    {
        if (attack.CauseKnockback)
        {
            KnockbackCaster knockbackCaster = attackerNormalCombat.gameObject.GetComponent<KnockbackCaster>();

            if (knockbackCaster != null)
            {
                TakeKnockBack(knockbackCaster);
            }
        }
    }
    

    #endregion
    
    #region Coroutines

    private IEnumerator TakeKnockBackCR(KnockbackCaster knockbackCaster)
    {
        Stats.Type type = transform.root.gameObject.GetComponent<Stats>().ThisUnitType;
        
        RaiseOnStartKnockback();

        // We are processing the KnockBack
        isProcessingKnockback = true;

        float knockBackTimeCounter = 0f;

        Rigidbody rb = transform.root.gameObject.GetComponent<Rigidbody>();

        Vector3 startingVelocity = rb.velocity;

        if (type == Stats.Type.Player)
        {
            rb.interpolation = RigidbodyInterpolation.Extrapolate;
        }

        // Calculate KnocKback direction 
        Vector3 knockBackDirection = transform.root.position - knockbackCaster.transform.root.position;
        knockBackDirection.y = 0.0f;
        knockBackDirection = knockBackDirection.normalized;

        // KnockBack lerp
        do
        {
            // Use FixedDeltaTimeInstead?
            knockBackTimeCounter += Time.fixedDeltaTime;

            rb.velocity = knockBackDirection * (knockbackCaster.Values.KnockBackSize / knockbackCaster.Values.KnockBackTime);

            // We are using physics so we need to wait for FixedUpdate
            yield return new WaitForFixedUpdate();
        } while (knockBackTimeCounter <= knockbackCaster.Values.KnockBackTime);

        rb.velocity = startingVelocity;

        if (type == Stats.Type.Player)
        {
            rb.interpolation = RigidbodyInterpolation.None;
        }

        // We are done processing the KnockBack
        isProcessingKnockback = false;

        RaiseOnEndKnockback();

        yield return null;
        knockBackCR = null;
    }

    #endregion
}
