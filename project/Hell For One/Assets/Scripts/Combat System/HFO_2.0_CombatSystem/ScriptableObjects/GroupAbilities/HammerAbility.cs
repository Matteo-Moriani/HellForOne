using System.Collections;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityAttack", menuName = "CombatSystem/GroupAbilities/Hammer", order = 1)]
public class HammerAbility : AbilityAttack
{
    public override IEnumerator PerformAbility(NormalCombat normalCombat, GameObject target, Action<AbilityAttack> stopAction)
    {
        normalCombat.StartAttack(this);
        
        yield return new WaitForSeconds(delayInSeconds + durationInSeconds);

        stopAction(this);
    }
}
