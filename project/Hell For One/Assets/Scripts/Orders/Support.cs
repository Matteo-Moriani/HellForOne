using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Support : MonoBehaviour
{
    #region Fields

    private static float supportReactionTime = 1.0f;

    private static int supportingUnits = 0;

    private Coroutine staySupportCr;
    
    private bool isSupporting = false;
    
    private GroupFinder groupFinder;
    private GroupBehaviour groupBehaviour;
    private Stats stats;
    private Reincarnation reincarnation;

    #endregion

    #region Properties

    public static int SupportingUnits
        {
            get => supportingUnits;
            private set => supportingUnits = value;
        }

    #endregion
    
    #region Delegates and events

    public delegate void OnStartSupport(Support sender);
    public event OnStartSupport onStartSupport;

    public delegate void OnStopSupport(Support sender);
    public event OnStopSupport onStopSupport;

    public delegate void OnStaySupport(Support sender);
    public event OnStaySupport onStaySupport;

    #region Methods

    private void RaiseOnStaySupport()
    {
        onStaySupport?.Invoke(this);
    }

    private void RaiseOnStartSupport()
    {
        onStartSupport?.Invoke(this);
    }
    
    private void RaiseOnStopSupport()
    {
        onStopSupport?.Invoke(this);
    }

    #endregion
    
    #endregion

    #region Unity methods

    private void Awake()
    {
        groupFinder = GetComponent<GroupFinder>();
        stats = GetComponent<Stats>();
        reincarnation = GetComponent<Reincarnation>();
    }

    private void OnEnable()
    {
        if(groupFinder != null)
            groupFinder.onGroupFound += OnGroupFound;
        
        if(stats != null)
            stats.onDeath += OnDeath;
        
        if(reincarnation != null)
            reincarnation.onReincarnation += OnReincarnation;
    }

    private void OnDisable()
    {
        if(groupFinder != null)
            groupFinder.onGroupFound -= OnGroupFound;
        
        if(stats != null)
            stats.onDeath -= OnDeath;
        
        if(reincarnation != null)
            reincarnation.onReincarnation -= OnReincarnation;
        
        if (groupBehaviour != null)
        {
            groupBehaviour.onStartSupportOrderGiven -= OnStartSupportOrderGiven;
            groupBehaviour.onStopSupportOrderGiven -= OnStopSupportOrderGiven;    
        }
    }

    #endregion
    
    #region Methods

    private void StartSupport()
    {
        if(isSupporting) return;
        
        isSupporting = true;
        
        if(staySupportCr == null)
            staySupportCr = StartCoroutine(OnStaySupportCoroutine());

        supportingUnits++;
        
        RaiseOnStartSupport();
    }

    private void StopSupport()
    {
        if (!isSupporting) return;
        
        isSupporting = false;

        if (staySupportCr != null)
        {
            StopCoroutine(staySupportCr);
            staySupportCr = null;
        }

        if(supportingUnits > 0)
            supportingUnits--;
        else
        {
            Debug.LogError(this.name + " " + this.gameObject.name + " is trying to decrease supporting units but it is already zero");
        }
        
        RaiseOnStopSupport();
    }

    #endregion

    #region Event handlers

    private void OnGroupFound(GroupFinder sender)
    {
        groupBehaviour = sender.GroupBelongingTo.GetComponent<GroupBehaviour>();

        groupBehaviour.onStartSupportOrderGiven += OnStartSupportOrderGiven;
        groupBehaviour.onStopSupportOrderGiven += OnStopSupportOrderGiven;
    }

    private void OnDeath(Stats sender)
    {
        StopAllCoroutines();

        groupBehaviour.onStartSupportOrderGiven -= OnStartSupportOrderGiven;
        groupBehaviour.onStopSupportOrderGiven -= OnStopSupportOrderGiven;

        supportingUnits--;
        
        enabled = false;
    }

    private void OnReincarnation(GameObject newplayer)
    {
        StopAllCoroutines();

        groupBehaviour.onStartSupportOrderGiven -= OnStartSupportOrderGiven;
        groupBehaviour.onStopSupportOrderGiven -= OnStopSupportOrderGiven;

        supportingUnits--;
        
        enabled = false;
    }

    private void OnStartSupportOrderGiven(GroupBehaviour sender)
    {
        StartSupport();
    }

    private void OnStopSupportOrderGiven(GroupBehaviour sender)
    {
        StopSupport();
    }

    #endregion

    #region Coroutines

    private IEnumerator OnStaySupportCoroutine()
    {
        while (true)
        {
            RaiseOnStaySupport();
            yield return new WaitForSeconds(supportReactionTime);
        }
    }

    #endregion
}
