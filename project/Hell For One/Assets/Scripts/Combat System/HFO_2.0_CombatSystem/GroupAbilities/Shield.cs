using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private Renderer[] renderers;
    private float forwardOffset = 0.8f;
    private IdleCollider idleCollider;
    
    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnEnable()
    {
        GroupAbilities groupAbilities = GetComponentInParent<GroupAbilities>();
        
        groupAbilities.onStartAbility += OnStartAbility;
        groupAbilities.onStopAbility += OnStopAbility;
    }

    private void OnDisable()
    {
        GroupAbilities groupAbilities = GetComponentInParent<GroupAbilities>();
        
        groupAbilities.onStartAbility -= OnStartAbility;
        groupAbilities.onStopAbility -= OnStopAbility;
    }
    
    private void Start()
    {
        idleCollider = GetComponentInChildren<IdleCollider>();
        idleCollider.gameObject.layer = LayerMask.NameToLayer("IgnoreAll");
    }

    private void OnStopAbility(AbilityAttack stoppedAbility)
    {
        if (stoppedAbility.AbilityOrder == GroupBehaviour.State.Tank)
        {
            foreach (var rend in renderers)
            {
                rend.enabled = false;
            }
            
            transform.localPosition = Vector3.zero;
            
            idleCollider.gameObject.layer = LayerMask.NameToLayer("IgnoreAll");
        }
    }

    private void OnStartAbility(AbilityAttack startedAbility)
    {
        if (startedAbility.AbilityOrder == GroupBehaviour.State.Tank)
        {
            foreach (var rend in renderers)
            {
                rend.enabled = true;
            }

            transform.localPosition =  Vector3.forward * forwardOffset;
            
            idleCollider.gameObject.layer = LayerMask.NameToLayer("CombatSystem");
        }
    }
}
