using AI.Movement;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;

public class BossAudio : CharacterAudio
{
    public override void SubscribeToOtherEvents()
    {
        GetComponent<ContextSteering>().OnStartMoving += OnStartMoving;
        GetComponent<ContextSteering>().OnStopMoving += OnStopMoving;
    }

    public override void UnsubscribeToOtherEvents()
    {
        GetComponent<ContextSteering>().OnStartMoving -= OnStartMoving;
        GetComponent<ContextSteering>().OnStopMoving -= OnStopMoving;
    }

    private void OnStartMoving()
    {
        Play("walk");
    }

    private void OnStopMoving()
    {
        Stop("walk");
    }
}
