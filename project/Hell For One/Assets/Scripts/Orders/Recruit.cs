using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Recruit : MonoBehaviour
{
    #region Fields

    private static float recruitReactionTime = 3.0f;
    
    private static int recruitingUnits = 0;
    
    private Coroutine recruitCr;

    private bool isRecruiting = false;

    private GroupFinder groupFinder;
    private GroupBehaviour groupBehaviour;
    private Stats stats;
    private Reincarnation reincarnation;
    
    #endregion

    #region Properties

    public static int RecruitingUnits
    { 
        get => recruitingUnits;
        set => recruitingUnits = value;
    }

    #endregion
    
    #region Delegates and events

    public delegate void OnStartRecruit(Recruit sender);
    public event OnStartRecruit onStartRecruit;

    public delegate void OnStopRecruit(Recruit sender);
    public event OnStopRecruit onStopRecruit;

    public delegate void OnStayRecruitStatic();
    public static event OnStayRecruitStatic onStayRecruitStatic;

    public delegate void OnStayRecruit(Recruit sender);
    public event OnStayRecruit onStayRecruit;
    
    #region Methods

    private static void RaiseOnStayRecruitStatic()
    {    
        onStayRecruitStatic?.Invoke();
    }

    private void RaiseOnStayRecruit()
    {
        onStayRecruit?.Invoke(this);
    }
    
    private void RaiseOnStartRecruit()
    {
        onStartRecruit?.Invoke(this);
    }

    private void RaiseOnStopRecruit()
    {
        onStopRecruit?.Invoke(this);
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
            groupBehaviour.onStartRecruitOrderGiven -= OnStartRecruitOrderGiven;
            groupBehaviour.onStopRecruitOrderGiven -= OnStopRecruitOrderGiven;    
        }
    }

    #endregion
    
    #region Methods

    private void StartRecruit()
    {
        if(isRecruiting) return;
        
        isRecruiting = true;
        
        if(recruitCr == null)
            recruitCr = StartCoroutine(OnStayRecruitCoroutine());

        recruitingUnits++;
        
        RaiseOnStartRecruit();
    }

    private void StopRecruit()
    {
        if (!isRecruiting) return;
        
        isRecruiting = false;

        if (recruitCr != null)
        {
            StopCoroutine(recruitCr);
            recruitCr = null;
        }
        
        if(recruitingUnits > 0)
            recruitingUnits--;
        else
        {
            Debug.LogError(this.name + " " + this.gameObject.name + " is trying to decrease recruiting units but it is already zero");
        }

        RaiseOnStopRecruit();
    }

    #endregion

    #region Event handlers

    private void OnGroupFound(GroupFinder sender)
    {
        groupBehaviour = sender.GroupBelongingTo.GetComponent<GroupBehaviour>();
    
        groupBehaviour.onStartRecruitOrderGiven += OnStartRecruitOrderGiven;
        groupBehaviour.onStopRecruitOrderGiven += OnStopRecruitOrderGiven;
        
        if(groupBehaviour.currentState == GroupBehaviour.State.Recruit)
            StartRecruit();
    }
    
    private void OnDeath(Stats sender)
    {
        if (isRecruiting)
        {
            StopRecruit();   
        }

        enabled = false;
    }
    
    private void OnReincarnation(GameObject newplayer)
    {
        if (isRecruiting)
        {
            StopRecruit();  
        }

        enabled = false;
    }
    
    private void OnStartRecruitOrderGiven(GroupBehaviour sender)
    {
        StartRecruit();
    }
    
    private void OnStopRecruitOrderGiven(GroupBehaviour sender)
    {
        StopRecruit();
    }

    #endregion
    
    #region Coroutines

    // TODO - At the moment used only to send a message to ImpAggro, decouple this
    private IEnumerator OnStayRecruitCoroutine()
    {
        while (true)
        {
            RaiseOnStayRecruit();
            yield return new WaitForSeconds(recruitReactionTime);
            //yield return new WaitForSeconds( -15 / 11 * recruitingUnits + 25);
        }
    }

    #endregion
}
