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
        root.gameObject.GetComponent<Reincarnation>().onLateReincarnation += OnLateReincarnation;
        GetComponent<Block>().onBlockSuccess += OnBlockSuccess;
    }

    private void OnDisable()
    {
        Transform root = transform.root;
        
        root.gameObject.GetComponent<Stats>().onDeath -= OnDeath;
        root.gameObject.GetComponent<Reincarnation>().onLateReincarnation -= OnLateReincarnation;
        GetComponent<Block>().onBlockSuccess -= OnBlockSuccess;
    }

    private void Start()
    {
        type = transform.root.GetComponent<Stats>().ThisUnitType;
    }

    #endregion

    #region Methods

    private void StartStun()
    {
        if (isStunned) return;
        
        isStunned = true;

        stunCr = StartCoroutine(StunCoroutine());
        
        // TODO - remove after testing
        Debug.Log("Player " + transform.root.gameObject.name + " start stun");
    }

    private void StopStun()
    {
        if (!isStunned) return;
        
        isStunned = false;

        if (stunCr == null) return;
        
        StopCoroutine(stunCr);
        stunCr = null;
        
        // TODO - remove after testing
        Debug.Log("Player " + transform.root.gameObject.name + " stop stun");
    }

    #endregion
    
    #region External events handlers

    private void OnDeath(Stats sender)
    {    
        StopStun();
    }

    private void OnLateReincarnation(GameObject newPlayer)
    {
        type = newPlayer.GetComponent<Stats>().ThisUnitType;
        StopStun();
    }

    private void OnBlockSuccess(Block sender, Attack attack, NormalCombat attackerNormalCombat)
    {
        if (type == Stats.Type.Player && attack.CauseStunWhenBlocked)
        {
            StartStun();
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator StunCoroutine()
    {
        RaiseOnStartStun();
        
        yield return stunDelay;
        
        RaiseOnStopStun();
        
        StopStun();
    }

    #endregion
}
