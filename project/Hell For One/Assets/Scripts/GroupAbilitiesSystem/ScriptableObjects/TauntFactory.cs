using System;
using System.Collections;
using AggroSystem;
using UnityEngine;
using Utils.ObjectPooling;

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
            GameObject toons = PoolersManager.Instance.TryGetPooler(data.AbilityPrefab).GetPooledObject();

            toons.transform.position = groupTransform.transform.position + new Vector3(0f, 1f, 0f);
            
            toons.transform.SetParent(groupTransform);
            
            GroupAggro groupAggro = groupTransform.GetComponent<GroupAggro>();
            
            groupAggro.SetAggro(float.MaxValue);

            yield return new WaitForSeconds(data.ActivatedDuration);
            
            groupAggro.SetAggro(data.AssociatedTactic.GetTactic().GetData().TacticAggro);

            PoolersManager.Instance.TryGetPooler(data.AbilityPrefab).DeactivatePooledObject(toons);
        }

        protected override void InnerSetup()
        {
        }

        protected override void InnerDispose()
        {
        }
    }
}