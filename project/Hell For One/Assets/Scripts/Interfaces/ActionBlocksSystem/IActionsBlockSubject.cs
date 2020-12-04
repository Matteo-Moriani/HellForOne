using System;
using Managers.OnUnitManagers;

namespace Interfaces.ActionBlocksSystem
{
    public interface IActionsBlockSubject
    {
        event Action<UnitActionsBlockManager.UnitAction[]> OnBlockEvent;
        event Action<UnitActionsBlockManager.UnitAction[]> OnUnblockEvent;
    }
}