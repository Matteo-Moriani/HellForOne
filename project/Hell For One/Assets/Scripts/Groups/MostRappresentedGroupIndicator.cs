using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MostRappresentedGroupIndicator : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private DemonBehaviour demonBehaviour;
    private GroupBehaviour groupBehaviour;
    private CombatEventsManager combatEventsManager;

    private void Awake()
    {
        combatEventsManager = this.transform.root.gameObject.GetComponent<CombatEventsManager>();
    }

    private void OnEnable()
    {
        GroupsInRangeDetector.RegisterOnMostRappresentedGroupChanged(ActivateIndicator);
        combatEventsManager.onReincarnation += DeactivateIndicator;
    }

    private void OnDisable()
    {
        GroupsInRangeDetector.UnregisterOnMostRappresentedGroupChanged(ActivateIndicator);
        combatEventsManager.onReincarnation -= DeactivateIndicator;
    }

    private void Start()
    {
        meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

        demonBehaviour = this.transform.root.gameObject.GetComponent<DemonBehaviour>();

        /*
        if (demonBehaviour != null)
        {
            groupBehaviour = demonBehaviour.groupBelongingTo.GetComponent<GroupBehaviour>();
        }
        else
        {
            Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find DemonBehaviour");
        }
        */
    }

    private void ActivateIndicator()
    {
        if (this.transform.root.gameObject.tag != "Player")
        {
            if (groupBehaviour == null)
            {
                if (demonBehaviour != null)
                {
                    groupBehaviour = demonBehaviour.groupBelongingTo.GetComponent<GroupBehaviour>();
                }
                else
                {
                    Debug.LogError(this.transform.root.gameObject.name + " " + this.name + " cannot find DemonBehaviour");
                }
            }

            if (groupBehaviour.ThisGroupName == GroupsInRangeDetector.MostRappresentedGroupInRange)
            {
                meshRenderer.enabled = true;
            }
            else
            {
                if (meshRenderer.enabled)
                {
                    meshRenderer.enabled = false;
                }
            }
        }
    }

    private void DeactivateIndicator()
    {
        if (meshRenderer.enabled)
        {
            meshRenderer.enabled = false;
        }
    }
}
