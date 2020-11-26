using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunReceiver : MonoBehaviour
{
    #region fields

    private Stats.Type type;

    private bool isStunned = false;
    private float stunDuration = 1.0f;

    private Coroutine stunCr;
    private WaitForSeconds stunDelay;

    #endregion

    #region Delegates and events

    public delegate void OnStartStun();
    public event OnStartStun onStartStun;

    public delegate void OnStopStun();
    public event OnStopStun onStopStun;

    #region Methods

    private void RaiseOnStartStun()
    {
        onStartStun?.Invoke();
    }

    private void RaiseOnStopStun()
    {
        onStopStun?.Invoke();
    }

    #endregion
    
    #endregion
    
    #region Unity methods

    private void Awake()
    {
        stunDelay = new WaitForSeconds(stunDuration);
    }

    private void OnEnable()
    {
        // TODO - Manage this better
        Transform root = transform.root;
        
        root.gameObject.GetComponent<Stats>().onDeath += OnDeath;

        Reincarnation reincarnation = root.gameObject.GetComponent<Reincarnation>();

        if (reincarnation)
        {
            root.gameObject.GetComponent<Reincarnation>().onLateReincarnation += OnLateReincarnation;    
        }
        
        GetComponent<Block>().onBlockSuccess += OnBlockSuccess;

        GetComponent<NormalCombat>().onAttackHit += OnAttackHit;
    }

    private void OnDisable()
    {
        Transform root = transform.root;
        
        
        root.gameObject.GetComponent<Stats>().onDeath -= OnDeath;
        
        Reincarnation reincarnation = root.gameObject.GetComponent<Reincarnation>();

        if (reincarnation)
        {
            root.gameObject.GetComponent<Reincarnation>().onLateReincarnation += OnLateReincarnation;    
        }
        
        GetComponent<Block>().onBlockSuccess -= OnBlockSuccess;
        
        GetComponent<NormalCombat>().onAttackHit -= OnAttackHit;
    }

    private void Start()
    {
        type = transform.root.GetComponent<Stats>().ThisUnitType;
    }

    #endregion

    #region Methods

    // TODO - set here stun duration
    private void StartStun(float stunDuration)
    {
        if (isStunned) return;
        
        isStunned = true;

        stunCr = StartCoroutine(StunCoroutine(stunDuration));
    }

    private void StopStun()
    {
        if (!isStunned) return;
        
        isStunned = false;

        if (stunCr == null) return;
        
        StopCoroutine(stunCr);
        stunCr = null;
    }

    #endregion
    
    #region External events handlers

    private void OnAttackHit(GenericAttack genericAttack, GenericIdle targetidlevalues)
    {
        if (targetidlevalues.CauseStunWhenHit)
        {
            StartStun(targetidlevalues.AttackerStunDuration);
        }
    }
    
    private void OnDeath(Stats sender)
    {    
        StopStun();
    }

    private void OnLateReincarnation(GameObject newPlayer)
    {
        type = newPlayer.GetComponent<Stats>().ThisUnitType;
        StopStun();
    }

    private void OnBlockSuccess(Block sender, GenericAttack genericAttack, NormalCombat attackerNormalCombat)
    {
        if (type == Stats.Type.Player && genericAttack.CauseStunWhenBlocked)
        {
            StartStun(stunDuration);
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator StunCoroutine(float stunDuration)
    {
        RaiseOnStartStun();
        
        yield return new WaitForSeconds(stunDuration);
        
        RaiseOnStopStun();
        
        StopStun();
    }

    #endregion
}
