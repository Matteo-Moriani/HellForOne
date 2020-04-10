﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityAttack", menuName = "CombatSystem/AbilityAttack", order = 1)]
public class AbilityAttack : Attack
{
    [SerializeField]
    private int manaCost = 0;

    [SerializeField] 
    private GroupBehaviour.State abilityOrder = GroupBehaviour.State.MeleeAttack;
    
    public int ManaCost
    {
        get => manaCost;
        private set => manaCost = value;
    }

    public GroupBehaviour.State AbilityOrder
    {
        get => abilityOrder;
        private set => abilityOrder = value;
    }
    
    #region Methods

    public override ObjectsPooler GetPooler()
    {
        if (isRanged)
        {
            if (projectilePooler == null)
            {
                projectilePooler = GameObject.FindWithTag("AbilityProjectiles").GetComponent<ObjectsPooler>();
            }

            return projectilePooler;
        }

        return null;
    }

    #endregion
}
