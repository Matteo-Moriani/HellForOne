using Managers;
using Managers.OnUnitManagers;

namespace Interfaces.ActionBlocksSystem
{
    public interface IActionsBlockObserver
    {
        void Block();
        void Unblock();

        UnitActionsBlockManager.UnitAction GetAction();
    }
}