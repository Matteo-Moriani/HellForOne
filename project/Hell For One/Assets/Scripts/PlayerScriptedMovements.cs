using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerScriptedMovements : MonoBehaviour
{
    private NavMeshAgent agent;
    private PlayerInput playerInput;
    private Vector3 target;
    private bool inScriptedMovement = false;
    private CombatEventsManager combatEventsManager;
    private bool isMoving = false;
    private int alliesNum;
    private int alliesInPosition = 0;
    private AlliesManager allies;
    private bool alliesNotified = false;

    public void OnEnable() {
        BattleEventsManager.onBattlePreparation += MoveToScriptedPosition;
        BattleEventsManager.onBossBattleEnter += ScriptedMovementEnd;
    }

    public void OnDisable() {
        BattleEventsManager.onBattlePreparation -= MoveToScriptedPosition;
        BattleEventsManager.onBossBattleEnter -= ScriptedMovementEnd;
    }

    void Start()
    {
        allies = AlliesManager.Instance;
        agent = GetComponent<NavMeshAgent>();
        playerInput = GetComponent<PlayerInput>();
        target = gameObject.transform.position;
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
    }

    private void FixedUpdate() {
        if(inScriptedMovement) {
            agent.SetDestination(target);
            if(!isMoving) {
                combatEventsManager.RaiseOnStartMoving();
                isMoving = true;
            }
            if((gameObject.transform.position - target).magnitude <= 0.1f && !alliesNotified) {
                alliesNotified = true;
                NotifyAllies(inScriptedMovement);
            }
            if(alliesInPosition == alliesNum){ 
                combatEventsManager.RaiseOnStartIdle();
                BattleEventsManager.RaiseOnBossBattleEnter();
            }

        } else {
            agent.SetDestination(gameObject.transform.position);
        }
        
    }

    void MoveToScriptedPosition() {
        alliesNum = allies.AlliesList.Count;
        inScriptedMovement = true;
    }

    void ScriptedMovementEnd() {
        inScriptedMovement = false;
        alliesNotified = false;
        NotifyAllies(inScriptedMovement);
        alliesInPosition = 0;
    }

    public void SetTargetPosition(Vector3 position) {
        target = position;
    }

    public void NotifyInPosition() {
        alliesInPosition++;
        Debug.Log("allies in position: " + alliesInPosition);
    }

    private void NotifyAllies(bool scriptedMovement) {
        foreach (GameObject ally in allies.AlliesList) {
            ally.GetComponent<DemonMovement>().InScriptedMovement = scriptedMovement;
            ally.GetComponent<DemonMovement>().PlayerNotified = false;
        }
    }
}

