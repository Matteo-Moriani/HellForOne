using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

// TODO - Merge this into GroupManager?
public class GroupFinder : MonoBehaviour
{
    #region Fields

    private Stats stats;

    private Reincarnation reincarnation;
    
    private GameObject groupBelongingTo;
    private bool groupFound = false;

    #endregion

    #region Properties

    public GameObject GroupBelongingTo
        {
            get => groupBelongingTo;
            private set => groupBelongingTo = value;
        }
    
        public bool GroupFound
        {
            get => groupFound;
            private set => groupFound = value;
        }

    #endregion

    #region Delegates and Events

    public delegate void OnGroupFound(GroupFinder sender);
    public event OnGroupFound onGroupFound;
    
    #region Methods

    private void RaiseOnGroupFound()
    {
        onGroupFound?.Invoke(this);
    }

    #endregion
    
    #endregion

    #region Unity methods

    private void Awake()
    {
        stats = GetComponent<Stats>();
        reincarnation = GetComponent<Reincarnation>();
    }

    private void Start()
    {
        StartCoroutine(FindGroupCoroutine());
    }

    private void OnEnable()
    {
        stats.onDeath += OnDeath;
        reincarnation.onReincarnation += OnReincarnation;
    }

    private void OnDisable()
    {
        stats.onDeath -= OnDeath;
        reincarnation.onReincarnation -= OnReincarnation;
    }

    #endregion

    #region Methods

    // Balances group entering too
    private void FindGroup()
    {
        GameObject bestGroup = null;
        int maxFreeSlots = 0;

        foreach ( GameObject group in GroupsManager.Instance.Groups )
        {
            int freeSlots = 0;

            GroupManager groupManager = group.GetComponent<GroupManager>();

            GameObject[] demonsArray = groupManager.Imps;
            
            freeSlots = groupManager.MaxImpNumber - groupManager.ImpsInGroupNumber;

            if ( freeSlots > maxFreeSlots )
            {
                maxFreeSlots = freeSlots;
                bestGroup = group;
            }
        }
        
        if ( bestGroup )
        {
            if (bestGroup.GetComponent<GroupManager>().AddDemonToGroup(this.gameObject)) {
                groupFound = true;
                groupBelongingTo = bestGroup;
            }
        }

        gameObject.GetComponent<ChildrenObjectsManager>().ActivateCircle();
        
        RaiseOnGroupFound();
    }

    #endregion

    #region Event handlers

    private void OnReincarnation(GameObject newplayer)
    {
        enabled = false;
        StopAllCoroutines();
    }

    private void OnDeath(Stats sender)
    {
        enabled = false;
        StopAllCoroutines();
    }

    #endregion

    #region Coroutines

    private IEnumerator FindGroupCoroutine()
    {
        while (!groupFound)
        {
            FindGroup();
            yield return null;
        }
    }

    #endregion
}
