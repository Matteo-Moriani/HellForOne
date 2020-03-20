using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpInRange : MonoBehaviour
{
    #region Fields

    private bool isImpInRange = false;
    
    private CombatEventsManager combatEventsManager;
    private Stats stats;
    private Reincarnation reincarnation;

    #endregion

    #region Unity methods

    private void Awake()
    {
        combatEventsManager = this.gameObject.GetComponent<CombatEventsManager>();
        reincarnation = this.gameObject.GetComponent<Reincarnation>();
        stats = GetComponent<Stats>();
    }

    private void OnEnable()
    {
        if(combatEventsManager != null) {
            stats.onDeath += OnDeath;
        }
        else {
            Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find CombatEventsManager");
        }
        if (reincarnation != null)
        {
            reincarnation.onReincarnation += OnReincarnation;
        }
        else
        {
            Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find Reincarnation");
        }
    }

    private void OnDisable()
    {
        if (combatEventsManager != null)
        {
            stats.onDeath -= OnDeath;
        }
        else
        {
            Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find CombatEventsManager");
        }
        if (reincarnation != null)
        {
            reincarnation.onReincarnation -= OnReincarnation;
        }
        else
        {
            Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find Reincarnation");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("GroupsInRangeDetector")) { 
            isImpInRange = true;    
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("GroupsInRangeDetector")) { 
            isImpInRange = false;    
        }
    }
    
    #endregion
    
    #region Event handlers

    private void OnDeath(Stats sender) {
        if (isImpInRange && (this.gameObject.tag != "Player" && this.gameObject.tag != "DeadPlayer")) { 
            GroupFinder groupFinder = this.gameObject.GetComponent<GroupFinder>();
            GameObject player = GameObject.FindWithTag("Player");
            GroupManager groupManager = null;

            if(groupFinder != null) {
                groupManager = groupFinder.GroupBelongingTo.GetComponent<GroupManager>();
            }
            else {
                Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find DemonBehaviour");
            }

            if(groupManager != null) {
                if(player != null) {
                    GroupsInRangeDetector groupsInRangeDetector = player.GetComponentInChildren<GroupsInRangeDetector>();
                    
                    if(groupsInRangeDetector != null) {

                        if (groupsInRangeDetector.IsTheGroupInRange(groupManager.ThisGroupName)) { 
                            groupsInRangeDetector.DecreaseImpInRangeCount(groupManager.ThisGroupName);
                            Debug.Log(this.transform.root.gameObject.name + " is dead before leaving the trigger and his group ImpsInRannge count is decreased");
                        }
                    }
                    else { 
                        Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find GroupsInRangeDetector" );    
                    }
                }
                else { 
                    Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find Player");    
                }
            }
            else { 
                Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find GroupBehaviour");    
            }
        }    
    }

    private void OnReincarnation(GameObject player) { 
        this.enabled = false;    
    }

    #endregion
}
