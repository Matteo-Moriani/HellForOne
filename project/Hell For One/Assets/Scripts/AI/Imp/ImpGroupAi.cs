using System;
using System.Collections;
using AI.Movement;
using CRBT;
using Groups;
using TacticsSystem;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace AI.Imp
{
    public class ImpGroupAi : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GroupManager.Group group;
        [SerializeField] private float fsmReactionTime = 0.2f;
        [SerializeField] private Tactic startTactic;

        private GroupManager _groupManager;
        
        private Tactic _activeTactic;
        private readonly AiUtils.TargetData _target = new AiUtils.TargetData();
        private bool _inBattle;

        #endregion

        #region Events

        public event Action<Tactic> OnOrderChanged;

        #endregion
        
        #region Unity Methods

        private void Awake()
        {
            _groupManager = GetComponent<GroupManager>();
            
            _activeTactic = startTactic;
        }

        private void OnEnable()
        {
            TacticsManager.OnTryOrderAssign += OnTryOrderAssign;

            _groupManager.OnImpJoined += OnImpJoined;
            
            BattleEventsManager.onBattleEnter += OnBattleEnter;
            BattleEventsManager.onBattleExit += OnBattleExit;
        }

        private void OnDisable()
        {
            TacticsManager.OnTryOrderAssign -= OnTryOrderAssign;
            
            _groupManager.OnImpJoined -= OnImpJoined;
            
            BattleEventsManager.onBattleEnter -= OnBattleEnter;
            BattleEventsManager.onBattleExit -= OnBattleExit;
        }

        private void Start()
        {
            FSMState outOfCombat = new FSMState();
            FSMState inCombat = new FSMState();
            
            FSMTransition battleEnter = new FSMTransition( () => _inBattle);
            FSMTransition battleExit = new FSMTransition( () => !_inBattle);
            
            outOfCombat.AddTransition(battleEnter,inCombat);
            inCombat.AddTransition(battleExit,outOfCombat);
            
            // TODO :- Replace FindWithTag!
            
            inCombat.enterActions.Add(SetBoss);
            inCombat.stayActions.Add(ExecuteOrder);
            
            outOfCombat.enterActions.Add(SetPlayer);

            FSM groupFsm = new FSM(outOfCombat);
            StartCoroutine(FsmStayAlive(groupFsm));
        }

        #endregion

        #region FSM actions

        private void SetPlayer() => _target.SetTarget(Reincarnation.player.transform);

        private void SetBoss() => _target.SetTarget(GameObject.FindWithTag("Boss").transform);

        private void ExecuteOrder()
        {
            foreach (Transform groupManagerImp in _groupManager.Imps)
            {
                _activeTactic.ExecuteOrder();
            }
        }

        #endregion
        
        #region Event handlers

        private void OnImpJoined(GroupManager sender, GameObject impJoined)
        {
            impJoined.GetComponent<ContextSteering>().SetTarget(_target);
        }
        
        private void OnTryOrderAssign(Tactic newTactic, GroupManager.Group targetGroup)
        {
            if(targetGroup != this.group) return;

            _activeTactic = newTactic;
            
            OnOrderChanged?.Invoke(_activeTactic);
        }

        private void OnBattleEnter() => _inBattle = true;

        private void OnBattleExit() => _inBattle = false;
        
        #endregion

        #region Coroutines

        private IEnumerator FsmStayAlive(FSM fsm)
        {
            while (true)
            {
                fsm.Update();
                yield return new WaitForSeconds(fsmReactionTime);
            }
        }

        #endregion
    }
}