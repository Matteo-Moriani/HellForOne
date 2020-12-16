using System;
using ActionsBlockSystem;
using GroupAbilitiesSystem.ScriptableObjects;
using UnityEngine;

namespace GroupAbilitiesSystem
{
    public class ImpAbilities : MonoBehaviour, IActionsBlockSubject
    {
        public void StartAbility(GroupAbility ability) => OnBlockEvent?.Invoke(ability.GetData().ActionBlocks);

        public void StopAbility(GroupAbility ability) => OnUnblockEvent?.Invoke(ability.GetData().ActionBlocks);

        public event Action<UnitActionsBlockManager.UnitAction[]> OnBlockEvent;
        public event Action<UnitActionsBlockManager.UnitAction[]> OnUnblockEvent;
    }
}