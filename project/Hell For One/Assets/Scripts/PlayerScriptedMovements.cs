﻿using System;
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
    public int alliesInPosition = 0;
    private AlliesManager allies;
    private bool alliesNotified = false;
    private float rotSpeed = 0.1f;
    private GameObject enemy;

    public event Action OnScriptedMovementStart;
    public event Action OnScriptedMovementEnd;

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

    // TODO - a questo punto conviene far partire l'evento da qualche altra parte a attivare questo script dall'evento
    private void FixedUpdate() {
        if(inScriptedMovement) {
            if(!enemy)
                enemy = GameObject.FindGameObjectWithTag("Boss");
            Face(enemy);
            agent.SetDestination(target);
            if(!isMoving) {
                combatEventsManager.RaiseOnStartMoving();
                isMoving = true;
            }
            if(gameObject.transform.position.x - target.x < 0.1f && gameObject.transform.position.z - target.z < 0.1f && !alliesNotified)
            {
                alliesNotified = true;
                NotifyAllies(inScriptedMovement);
                OnScriptedMovementEnd?.Invoke();
            }
            if(alliesInPosition == AlliesManager.Instance.AlliesList.Count){ 
                combatEventsManager.RaiseOnStopMoving();
                BattleEventsManager.RaiseOnBossBattleEnter();
            }

        }        
    }

    void MoveToScriptedPosition() {
        agent.enabled = true;
        alliesNum = allies.AlliesList.Count;
        inScriptedMovement = true;
        OnScriptedMovementStart?.Invoke();
        playerInput.InCutscene = true;
    }

    void ScriptedMovementEnd() {
        agent.enabled = false;
        inScriptedMovement = false;
        alliesNotified = false;
        NotifyAllies(inScriptedMovement);
        alliesInPosition = 0;
        playerInput.InCutscene = false;
    }

    public void SetTargetPosition(Vector3 position) {
        target = position;
    }

    public void NotifyInPosition() {
        alliesInPosition++;
        //Debug.Log("allies in position: " + alliesInPosition);
    }

    private void NotifyAllies(bool scriptedMovement) {
        foreach (GameObject ally in allies.AlliesList) {
            ally.GetComponent<AllyImpMovement>().InScriptedMovement = scriptedMovement;
            ally.GetComponent<AllyImpMovement>().PlayerNotified = false;
        }
    }

    public void Face(GameObject target) {
        Vector3 targetPosition = target.transform.position;
        Vector3 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.y = 0f;
        Quaternion facingDir = Quaternion.LookRotation(vectorToTarget);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, facingDir, rotSpeed);
        transform.rotation = newRotation;
    }
}

