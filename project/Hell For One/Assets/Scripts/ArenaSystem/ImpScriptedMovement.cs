using System;
using System.Collections;
using ActionsBlockSystem;
using AI.Imp;
using GroupSystem;
using ReincarnationSystem;
using UnityEngine;
using UnityEngine.AI;

namespace ArenaSystem
{
    public class ImpScriptedMovement : MonoBehaviour, IArenaObserver, IActionsBlockSubject, IReincarnationObserver
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

            switch (GetComponent<GroupFinder>().Group.ThisGroupName)
            {
                case GroupManager.Group.GroupAzure:
                    StartCoroutine(ScriptedMovement(subject.Group0StartPosition.position, subject));
                    break;
                case GroupManager.Group.GroupPink:
                    StartCoroutine(ScriptedMovement(subject.Group1StartPosition.position, subject));
                    break;
                case GroupManager.Group.GroupGreen:
                    StartCoroutine(ScriptedMovement(subject.Group2StartPosition.position, subject));
                    break;
                case GroupManager.Group.GroupYellow:
                    StartCoroutine(ScriptedMovement(subject.Group3StartPosition.position, subject));
                    break;
                case GroupManager.Group.All:
                    break;
                case GroupManager.Group.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void EnterBattle(ArenaManager subject)
        {
            OnUnblockEvent?.Invoke(actionBlocks);
        }

        public void ExitBattle(ArenaManager subject)
        {
        
        }

        // Leader needs ScriptedMovements
        // This is very ugly, but is fast
        public void Reincarnate()
        {
            gameObject.AddComponent<ScriptedMovements>();
            Destroy(this);
        }

        #endregion
    }
}