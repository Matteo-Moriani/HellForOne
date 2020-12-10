using System;
using System.Collections;
using System.Collections.Generic;
using ActionsBlockSystem;
using ArenaSystem;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.AI;

public class PlayerScriptedMovements : MonoBehaviour, IActionsBlockSubject, IArenaObserver
{
    [SerializeField] private UnitActionsBlockManager.UnitAction[] actionBlocks;
    
    private NavMeshAgent _agent;

    #region Unity Methods

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    #endregion

    #region Coroutines

    private IEnumerator ScriptedMovement(Vector3 targetPosition, ArenaManager arenaManager)
    {
        _agent.enabled = true;
        _agent.SetDestination(targetPosition);

        while (Vector3.Distance(transform.position, targetPosition) >= 1.5f) yield return null;
        
        arenaManager.NotifyBattlePrepared(this);
        _agent.enabled = false;
    }

    #endregion
    
    #region Interfaces

    public event Action<UnitActionsBlockManager.UnitAction[]> OnBlockEvent;
    public event Action<UnitActionsBlockManager.UnitAction[]> OnUnblockEvent;
    
    public void PrepareBattle(ArenaManager subject)
    {
        OnBlockEvent?.Invoke(actionBlocks);
        
        StartCoroutine(ScriptedMovement(subject.PlayerStartPosition.position, subject));
    }

    public void EnterBattle(ArenaManager subject)
    {
        OnUnblockEvent?.Invoke(actionBlocks);
    }

    public void ExitBattle(ArenaManager subject)
    {
        
    }

    #endregion
}

