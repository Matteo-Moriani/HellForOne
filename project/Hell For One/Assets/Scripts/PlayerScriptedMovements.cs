using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerScriptedMovements : MonoBehaviour
{
    private NavMeshAgent agent;
    private PlayerInput playerInput;
    private Vector3 target;
    private bool scriptedMovementStarted = false;
    private CombatEventsManager combatEventsManager;
    private bool isMoving = false;
    private int alliesNum;
    private int alliesInPosition = 0;
    private AlliesManager allies;

    public void OnEnable() {
        BattleEventsManager.onBattlePreparation += MoveToScriptedPosition;
        BattleEventsManager.onBossBattleEnter += FreeMovement;
    }

    public void OnDisable() {
        BattleEventsManager.onBattlePreparation -= MoveToScriptedPosition;
        BattleEventsManager.onBossBattleEnter -= FreeMovement;
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
        if(scriptedMovementStarted) {
            agent.SetDestination(target);
            if(!isMoving) {
                combatEventsManager.RaiseOnStartMoving();
                isMoving = true;
            }
            if((gameObject.transform.position - target).magnitude <= 0.1f) {
                NotifyAllies();
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
        scriptedMovementStarted = true;
        playerInput.Playing = false;
    }

    void FreeMovement() {
        scriptedMovementStarted = false;
        playerInput.Playing = true;
        alliesInPosition = 0;
    }

    public void SetTargetPosition(Vector3 position) {
        target = position;
    }

    public void NotifyInPosition() {
        alliesInPosition++;
        Debug.Log("allies in position: " + alliesInPosition);
    }

    private void NotifyAllies() {
        foreach (GameObject ally in allies.AlliesList) {
            ally.GetComponent<DemonMovement>().InScriptedMovement = true;
        }
    }
}

