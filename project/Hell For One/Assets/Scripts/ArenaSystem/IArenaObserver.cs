namespace ArenaSystem
{
    public interface IArenaObserver
    {
        void PrepareBattle(ArenaManager subject);
        void EnterBattle(ArenaManager subject);
        void ExitBattle(ArenaManager subject);
    }
}