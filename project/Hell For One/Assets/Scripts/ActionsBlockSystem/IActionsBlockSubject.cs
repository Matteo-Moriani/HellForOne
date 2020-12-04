using System;

namespace ActionsBlockSystem
{
    public interface IActionsBlockSubject
    {
        event Action<UnitActionsBlockManager.UnitAction[]> OnBlockEvent;
        event Action<UnitActionsBlockManager.UnitAction[]> OnUnblockEvent;
    }
}