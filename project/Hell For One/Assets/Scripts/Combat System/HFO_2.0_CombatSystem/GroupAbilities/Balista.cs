using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

public class Balista : MonoBehaviour
{
    private Renderer[] renderers;
    private float backOffset = 2.0f;
    
    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        transform.localPosition = Vector3.up * transform.localScale.y / 2 + (-Vector3.forward * backOffset);
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
        // if (stoppedAbility.AbilityOrder == GroupBehaviour.State.RangeAttack)
        // {
        //     foreach (var rend in renderers)
        //     {
        //         rend.enabled = false;
        //     }
        //     
        //     transform.localPosition = Vector3.up * transform.localScale.y / 2 + (-Vector3.forward * backOffset);
        // }
    }

    private void OnStartAbility(AbilityAttack startedAbility)
    {
        // if (startedAbility.AbilityOrder == GroupBehaviour.State.RangeAttack)
        // {
        //     foreach (var rend in renderers)
        //     {
        //         rend.enabled = true;
        //     }
        //     
        //     StartCoroutine(AnimationCoroutine(startedAbility));
        // }
    }

    private IEnumerator AnimationCoroutine(AbilityAttack startedAbility)
    {
        yield return new WaitForSeconds(startedAbility.DelayInSeconds);

        foreach (Renderer rend in renderers)
        {
            if (rend.gameObject.name == "Head" || rend.gameObject.name == "Body")
                rend.enabled = false;
        }
    }
}
