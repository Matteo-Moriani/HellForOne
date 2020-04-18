using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Horn : MonoBehaviour
{
    private Renderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        //transform.localPosition = Vector3.up * transform.localScale.y / 2;
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
        foreach (var rend in renderers)
        {
            rend.enabled = false;
        }
    }

    private void OnStopAbility(AbilityAttack stoppedAbility)
    {
        if (stoppedAbility.AbilityOrder == GroupBehaviour.State.Recruit)
        {
            foreach (var rend in renderers)
            {
                rend.enabled = false;
            }

            // TODO - ReSet childrean postion
        }
    }

    private void OnStartAbility(AbilityAttack startedAbility)
    {
        if (startedAbility.AbilityOrder == GroupBehaviour.State.Recruit)
        {
            foreach (var rend in renderers)
            {
                rend.enabled = true;
            }
            
            // Set children position

            // TODO - This is for testing, find better solution
            GameObject arenaCenter = GameObject.FindWithTag("ArenaCenter");

            foreach (Transform child in transform)
            {
                float angle = Random.Range(1f,1000f) * Mathf.PI * 2;
                float x = Mathf.Cos(angle) * 10;
                float z = Mathf.Sin(angle)* 10;
                
                child.position = new Vector3(arenaCenter.transform.position.x + x, child.transform.position.y, arenaCenter.transform.position.z + z);
                
                // TODO - Optimize this
                child.transform.LookAt(GameObject.FindWithTag("Boss").transform);
            }
        }
    }
}
