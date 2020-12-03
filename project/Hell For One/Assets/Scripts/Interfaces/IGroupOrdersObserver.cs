using OrdersSystem.ScriptableObjects;

namespace Interfaces
{
    public interface IGroupOrdersObserver
    {
        void ProcessOrderChanged(Oder newOrder);
    }
}