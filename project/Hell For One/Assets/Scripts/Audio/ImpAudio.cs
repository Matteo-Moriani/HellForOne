using AI.Movement;
using FactoryBasedCombatSystem;
using Player;
using ReincarnationSystem;
using System;

public class ImpAudio : CharacterAudio, IReincarnationObserver
{
    public override void SubscribeToOtherEvents()
    {
        gameObject.GetComponentInChildren<Dash>().OnStartDash += OnStartDash;
        if(gameObject.tag == "Player")
            StartLeader();
        else
        {
            GetComponent<ContextSteering>().OnStartMoving += OnStartMoving;
            GetComponent<ContextSteering>().OnStopMoving += OnStopMoving;
        }
    }

    public override void UnsubscribeToOtherEvents()
    {
        gameObject.GetComponentInChildren<Dash>().OnStartDash -= OnStartDash;
        if(gameObject.tag == "Player")
            StopLeader();
        else
        {
            GetComponent<ContextSteering>().OnStartMoving -= OnStartMoving;
            GetComponent<ContextSteering>().OnStopMoving -= OnStopMoving;
        }
    }

    public void StartLeader()
    {
        GetComponent<PlayerMovement>().OnStartMoving += OnStartLeaderMoving;
        GetComponent<PlayerMovement>().OnStopMoving += OnStopLeaderMoving;
    }

    public void StopLeader()
    {
        GetComponent<PlayerMovement>().OnStartMoving -= OnStartLeaderMoving;
        GetComponent<PlayerMovement>().OnStopMoving -= OnStopLeaderMoving;
    }

    private void OnStartMoving()
    {
        Play("walk");
    }

    private void OnStopMoving()
    {
        Stop("walk");
    }

    private void OnStartLeaderMoving()
    {
        Play("leaderWalk");
    }

    private void OnStopLeaderMoving()
    {
        Stop("leaderWalk");
    }

    private void OnStartDash()
    {
        Play("dash");
    }
}
