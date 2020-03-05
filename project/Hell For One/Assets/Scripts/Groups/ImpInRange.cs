using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpInRange : MonoBehaviour
{
    private bool isImpInRange = false;
    private CombatEventsManager combatEventsManager;

    private void Awake()
    {
        combatEventsManager = this.gameObject.GetComponent<CombatEventsManager>();
    }

    private void OnEnable()
    {
        if(combatEventsManager != null) {
            combatEventsManager.onDeath += OnDeath;
            combatEventsManager.onReincarnation += OnReincarnation;
        }
        else {
            Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find CombatEventsManager");
        }
    }

    private void OnDisable()
    {
        if (combatEventsManager != null)
        {
            combatEventsManager.onDeath -= OnDeath;
            combatEventsManager.onReincarnation -= OnReincarnation;
        }
        else
        {
            Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find CombatEventsManager");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "GroupsInRangeDetector") { 
            isImpInRange = true;    
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "GroupsInRangeDetector") { 
            isImpInRange = false;    
        }
    }

    private void OnDeath() {
        if (isImpInRange && (this.gameObject.tag != "Player" && this.gameObject.tag != "DeadPlayer")) { 
            DemonBehaviour demonBehaviour = this.gameObject.GetComponent<DemonBehaviour>();
            GameObject player = GameObject.FindWithTag("Player");
            GroupBehaviour groupBehaviour = null;

            if(demonBehaviour != null) { 
                groupBehaviour = demonBehaviour.groupBelongingTo.GetComponent<GroupBehaviour>();
            }
            else {
                Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find DemonBehaviour");
            }

            if(groupBehaviour != null) {
                if(player != null) {
                    GroupsInRangeDetector groupsInRangeDetector = player.GetComponentInChildren<GroupsInRangeDetector>();
                    
                    if(groupsInRangeDetector != null) {

                        if (groupsInRangeDetector.IsTheGroupInRange(groupBehaviour.ThisGroupName)) { 
                            groupsInRangeDetector.DecreaseImpInRangeCount(groupBehaviour.ThisGroupName);
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

    private void OnReincarnation() { 
        this.enabled = false;    
    }
}
