namespace CooldownSystem
{
    public interface ICooldown
    {
        float GetCooldown();
        void NotifyCooldownStart();
        void NotifyCooldownEnd();
    }
}