using System;

namespace AggroSystem
{
    public interface IPlayerAggroSubject
    {
        event Action<float> OnAggroActionDone;
    }
}