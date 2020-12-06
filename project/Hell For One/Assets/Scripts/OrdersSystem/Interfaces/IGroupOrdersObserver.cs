using OrdersSystem.ScriptableObjects;

namespace Interfaces
{
    public interface IGroupOrdersObserver
    {
        /// <summary>
        /// Called when the player assigns an order to a group
        /// </summary>
        /// <param name="newOrder">The order assigned</param>
        void OnOrderGiven(Order newOrder);
        
        /// <summary>
        /// Called when a group changes order state
        /// </summary>
        /// <param name="newOrder">The order assigned</param>
        void OnOrderAssigned(Order newOrder);
    }
}