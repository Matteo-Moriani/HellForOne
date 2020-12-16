using System;
using System.Collections;
using AggroSystem;
using UnityEngine;

namespace GroupAbilitiesSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "GroupAbilities/TauntAbility", fileName = "TauntAbility", order = 1)]
    public class TauntFactory : GroupAbilityFactory<TauntAbility,TauntAbilityData>
    {
        
    }

    [Serializable]
    public class TauntAbilityData : GroupAbilityData
    {
        
    }

    public class TauntAbility : GroupAbility<TauntAbilityData>
    {
        protected override IEnumerator InnerDoGroupAbility(Transform groupTransform)
        {
            GroupAggro groupAggro = groupTransform.GetComponent<GroupAggro>();
            
            groupAggro.SetAggro(float.MaxValue);

            yield return new WaitForSeconds(data.ActivatedDuration);
            
            groupAggro.SetAggro(data.AssociatedTactic.GetTactic().GetData().TacticAggro);
        }

        protected override void InnerSetup()
        {
        }

        protected override void InnerDispose()
        {
        }
    }
}