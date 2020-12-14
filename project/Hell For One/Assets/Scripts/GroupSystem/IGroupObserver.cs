using AI.Imp;

namespace GroupSystem
{
    public interface IGroupObserver
    {
        void JoinGroup(GroupManager groupManager);

        void LeaveGroup(GroupManager groupManager);
    }
}