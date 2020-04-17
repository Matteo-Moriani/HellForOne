using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlliedAttack", menuName = "CombatSystem/AlliedAttack", order = 1)]
public class AlliedAttack : GenericAttack
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
   
   public override bool IsLegitAttack(GenericIdle targetIdleValues)
   {
      if (typeof(EnemyIdle) == targetIdleValues.GetType())
      {
         return true;
      }
      else
      {
         return false;
      }
   }

   public override bool CanRiseAggro(Stats.Type unitType)
   {
      if (unitType == Stats.Type.Ally || unitType == Stats.Type.Player)
      {
         return true;
      }

      else
      {
         return false;
      }
   }
}
