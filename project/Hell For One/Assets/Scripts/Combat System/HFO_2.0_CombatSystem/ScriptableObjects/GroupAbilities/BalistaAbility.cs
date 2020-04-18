using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityAttack", menuName = "CombatSystem/GroupAbilities/Balista", order = 1)]
public class BalistaAbility : AbilityAttack
{
    public override IEnumerator PerformAbility(NormalCombat normalCombat, GameObject target, Action<AbilityAttack> stopAction)
    {
        normalCombat.StartAttackRanged(this,target);
        
        yield return new WaitForSeconds(delayInSeconds + durationInSeconds);

        stopAction(this);
    }
}
