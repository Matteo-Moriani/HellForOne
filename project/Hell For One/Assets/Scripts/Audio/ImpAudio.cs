using AI.Movement;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using Player;
using ReincarnationSystem;
using System;
using UnityEngine;

public class ImpAudio : CharacterAudio, IReincarnationObserver
{
    public override void SubscribeToOtherEvents()
    {
        gameObject.GetComponentInChildren<Dash>().OnStartDash += OnStartDash;
        gameObject.GetComponentInChildren<CombatSystem>().OnBlockedHitReceived += OnBlockedHitReceived;
        gameObject.GetComponentInChildren<CombatSystem>().OnDamageHitDealt += OnDamageHitDealt;

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
        gameObject.GetComponentInChildren<CombatSystem>().OnBlockedHitReceived -= OnBlockedHitReceived;
        gameObject.GetComponentInChildren<CombatSystem>().OnDamageHitDealt -= OnDamageHitDealt;

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

    private void OnBlockedHitReceived(Attack a, CombatSystem c, Vector3 v)
    {
        Play("shield");
    }

    private void OnDamageHitDealt(Attack a, CombatSystem c, Vector3 v)
    {
        Play("spear");
    }
}
