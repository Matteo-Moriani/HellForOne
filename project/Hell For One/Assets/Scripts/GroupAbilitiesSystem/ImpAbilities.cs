using System;
using ActionsBlockSystem;
using UnityEngine;

namespace GroupAbilitiesSystem
{
    public class ImpAbilities : MonoBehaviour, IActionsBlockSubject
    {
        [SerializeField] private UnitActionsBlockManager.UnitAction[] actionBlocks;
        
        public void StartAbility() => OnBlockEvent?.Invoke(actionBlocks);

        public void StopAbility() => OnUnblockEvent?.Invoke(actionBlocks);

        public event Action<UnitActionsBlockManager.UnitAction[]> OnBlockEvent;
        public event Action<UnitActionsBlockManager.UnitAction[]> OnUnblockEvent;
    }
}