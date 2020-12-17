namespace CrownSystem
{
    public interface ICrownObserver
    {
        void OnCrownCollected();
        void OnCrownLost();
    }
}