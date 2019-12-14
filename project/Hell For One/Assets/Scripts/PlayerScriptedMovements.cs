using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerScriptedMovements : MonoBehaviour
{
    private NavMeshAgent agent;
    private PlayerInput playerInput;
    private Vector3 target;
    private bool scriptedMovement = false;
    private CombatEventsManager combatEventsManager;
    private bool isMoving = false;

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
        agent = GetComponent<NavMeshAgent>();
        playerInput = GetComponent<PlayerInput>();
        target = gameObject.transform.position;
        combatEventsManager = gameObject.GetComponent<CombatEventsManager>();
    }

    private void FixedUpdate() {
        if(scriptedMovement) {
            agent.SetDestination(target);
            if(!isMoving) {
                combatEventsManager.RaiseOnStartMoving();
                isMoving = true;
            }
            // TODO - if tutti in posizione, prossimo evento
            if((gameObject.transform.position - target).magnitude <= 0.1f) {
                combatEventsManager.RaiseOnStartIdle();
                BattleEventsManager.RaiseOnBossBattleEnter();
            }

        } else {
            agent.SetDestination(gameObject.transform.position);
        }
        
    }

    void MoveToScriptedPosition() {
        scriptedMovement = true;
        playerInput.Playing = false;
    }

    void FreeMovement() {
        scriptedMovement = false;
        playerInput.Playing = true;
    }

    public void SetTargetPosition(Vector3 position) {
        target = position;
    }
}
