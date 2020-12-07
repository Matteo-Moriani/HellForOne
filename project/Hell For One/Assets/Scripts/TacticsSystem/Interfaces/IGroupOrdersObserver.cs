using TacticsSystem.ScriptableObjects;

namespace Interfaces
{
    public interface IGroupOrdersObserver
    {
        /// <summary>
        /// Called when the player assigns an order to a group
        /// </summary>
        /// <param name="newTactic">The order assigned</param>
        void OnOrderGiven(Tactic newTactic);
        
        /// <summary>
        /// Called when a group changes order state
        /// </summary>
        /// <param name="newTactic">The order assigned</param>
        void OnOrderAssigned(Tactic newTactic);
    }
}