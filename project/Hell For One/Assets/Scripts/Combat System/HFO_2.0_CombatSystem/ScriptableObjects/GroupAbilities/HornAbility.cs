using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityAttack", menuName = "CombatSystem/GroupAbilities/Horn", order = 1)]
public class HornAbility : AbilityAttack
{
    public override ObjectsPooler GetPooler()
    {
        if (isRanged)
        {
            if (projectilePooler == null)
            {
                projectilePooler = GameObject.FindWithTag("NormalProjectiles").GetComponent<ObjectsPooler>();
            }

            return projectilePooler;
        }

        return null;
    }
    
    // TODO - optimize this.
    // Remember: values in scriptable objects are permanent
    // An idea could be pick random spectators from a list
    // Or pooling them
    public override IEnumerator PerformAbility(NormalCombat normalCombat, GameObject target, Action<AbilityAttack> stopAction)
    {
        yield return new WaitForSeconds(delayInSeconds);
        
        foreach (NormalCombat tempNormalCombat in normalCombat.gameObject.GetComponentsInChildren<NormalCombat>())
        {
            if(tempNormalCombat != normalCombat)
                tempNormalCombat.StartAttackRanged(this, target);
            
            // TODO - check timing
            yield return new WaitForSeconds(0.2f);
        }

        stopAction(this);
    }
    
}
