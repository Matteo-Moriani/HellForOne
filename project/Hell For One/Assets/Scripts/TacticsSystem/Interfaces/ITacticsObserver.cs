using TacticsSystem.ScriptableObjects;

namespace TacticsSystem.Interfaces
{
    public interface ITacticsObserver
    {
        void StartTactic(Tactic newTactic);
        void EndTactic();
    }
}