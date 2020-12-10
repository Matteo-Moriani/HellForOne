namespace ActionsBlockSystem
{
    public interface IActionsBlockObserver
    {
        void Block();
        void Unblock();

        UnitActionsBlockManager.UnitAction GetAction();
    }
}