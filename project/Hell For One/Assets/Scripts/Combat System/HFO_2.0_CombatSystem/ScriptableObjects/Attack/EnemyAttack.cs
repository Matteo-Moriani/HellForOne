using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyAttack", menuName = "CombatSystem/EnemyAttack", order = 1)]
public class EnemyAttack : GenericAttack
{
    public override bool IsLegitAttack(GenericIdle targetIdleValues)
    {
        if (typeof(AlliedIdle) == targetIdleValues.GetType())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
