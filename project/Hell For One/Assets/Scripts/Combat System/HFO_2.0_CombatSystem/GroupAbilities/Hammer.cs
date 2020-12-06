using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    private Renderer[] renderers;
    private NormalCombat parentNormalCombat;
    
    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        transform.localPosition = Vector3.up * transform.localScale.y / 2;
        transform.parent.GetComponent<NormalCombat>();
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

    private void OnStopAbility(AbilityAttack stoppedAbility)
    {
        if (stoppedAbility.AbilityOrder == GroupBehaviour.State.MeleeAttack)
        {
            foreach (var rend in renderers)
            {
                rend.enabled = false;
            }
            
            transform.localPosition = Vector3.up * transform.localScale.y / 2;
        }
    }

    private void OnStartAbility(AbilityAttack startedAbility)
    {
        if (startedAbility.AbilityOrder == GroupBehaviour.State.MeleeAttack)
        {
            foreach (var rend in renderers)
            {
                rend.enabled = true;

                StartCoroutine(AnimationCoroutine(startedAbility));
            }
        }
    }

    private IEnumerator AnimationCoroutine(AbilityAttack startedAbility)
    {
        yield return new WaitForSeconds(startedAbility.DelayInSeconds);

        transform.localPosition = Vector3.forward * startedAbility.Range +  Vector3.up * transform.localScale.y / 2;
    }
}
