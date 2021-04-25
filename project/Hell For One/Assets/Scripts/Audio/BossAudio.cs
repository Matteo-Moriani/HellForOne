using AI.Movement;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

public class BossAudio : CharacterAudio
{
    public override void SubscribeToOtherEvents()
    {
        GetComponent<ContextSteering>().OnStartMoving += OnStartMoving;
        GetComponent<ContextSteering>().OnStopMoving += OnStopMoving;
        GetComponentInChildren<CombatSystem>().OnDamageHitDealt += OnDamageHitDealt;
        GetComponentInChildren<CombatSystem>().OnDamageHitReceived += OnDamageHitReceived;
    }

    public override void UnsubscribeToOtherEvents()
    {
        GetComponent<ContextSteering>().OnStartMoving -= OnStartMoving;
        GetComponent<ContextSteering>().OnStopMoving -= OnStopMoving;
        GetComponentInChildren<CombatSystem>().OnDamageHitDealt -= OnDamageHitDealt;
        GetComponentInChildren<CombatSystem>().OnDamageHitReceived -= OnDamageHitReceived;
    }

    private void OnStartMoving()
    {
        Play("walk");
    }

    private void OnStopMoving()
    {
        Stop("walk");
    }

    private void OnDamageHitDealt(Attack attack, CombatSystem c, Vector3 v)
    {
        switch(attack.name)
        {
            case "MidBossSingleAttack":
                Play("singleAttack");
                break;
            case "MidBossGroupAttack":
                Play("groupAttack");
                break;
            case "BossSingleAttack":
                Play("singleAttack");
                break;
            case "BossGroupAttack":
                Play("groupAttack");
                break;
            default:
                Debug.Log("ATTACK "+ attack.name +" NOT FOUND");
                break;
        }
    }

    private void OnDamageHitReceived(Attack attack, CombatSystem c, Vector3 v)
    {
        switch(attack.name)
        {
            case "HammerMeleeAttack":
                Play("hammer");
                break;
            default:
                //Debug.Log("ATTACK " + attack.name + " NOT FOUND");
                break;
        }
    }
}
