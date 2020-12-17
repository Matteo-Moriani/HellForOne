using AI.Movement;
using FactoryBasedCombatSystem;
using Player;
using ReincarnationSystem;

public class ImpAudio : CharacterAudio, IReincarnationObserver
{
    public override void SubscribeToOtherEvents()
    {
        gameObject.GetComponentInChildren<Dash>().OnStartDash += OnStartDash;
        GetComponent<ContextSteering>().OnStartMoving += OnStartMoving;
        GetComponent<ContextSteering>().OnStopMoving += OnStopMoving;
    }

    public override void UnsubscribeToOtherEvents()
    {
        gameObject.GetComponentInChildren<Dash>().OnStartDash -= OnStartDash;
        GetComponent<ContextSteering>().OnStartMoving -= OnStartMoving;
        GetComponent<ContextSteering>().OnStopMoving -= OnStopMoving;
    }

    public void StartLeader()
    {
        GetComponent<PlayerMovement>().OnStartMoving += OnStartMoving;
        GetComponent<PlayerMovement>().OnStopMoving += OnStopMoving;
    }

    public void StopLeader()
    {
        GetComponent<PlayerMovement>().OnStartMoving -= OnStartMoving;
        GetComponent<PlayerMovement>().OnStopMoving -= OnStopMoving;
    }

    private void OnStartMoving()
    {
        Play("walk");
    }

    private void OnStopMoving()
    {
        Stop("walk");
    }

    private void OnStartDash()
    {
        Play("dash");
    }
}
