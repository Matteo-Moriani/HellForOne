﻿using System;
using System.Collections;
using AI.Movement;
using ArenaSystem;
using CRBT;
using GroupSystem;
using Managers;
using TacticsSystem;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace AI.Imp
{
    public class ImpGroupAi : MonoBehaviour
    {
        #region Fields
        
        [SerializeField] private float fsmReactionTime = 0.2f;
        [SerializeField] private TacticFactory startTactic;

        private GroupManager _groupManager;
        
        private TacticFactory _activeTactic;
        private readonly AiUtils.TargetData _target = new AiUtils.TargetData();
        private bool _inBattle;

        #endregion

        #region Events

        public event Action<TacticFactory> OnTacticChanged;

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
            
            ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        }

        private void OnDisable()
        {
            TacticsManager.OnTryOrderAssign -= OnTryOrderAssign;
            
            _groupManager.OnImpJoined -= OnImpJoined;
            
            ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;
        }

        private void Start()
        {
            FSMState outOfCombat = new FSMState();
            FSMState inCombat = new FSMState();
            
            FSMTransition battleEnter = new FSMTransition( () => _inBattle);
            FSMTransition battleExit = new FSMTransition( () => !_inBattle);
            
            outOfCombat.AddTransition(battleEnter,inCombat);
            inCombat.AddTransition(battleExit,outOfCombat);
            
            inCombat.enterActions.Add(() => OnTacticChanged?.Invoke(_activeTactic));
            inCombat.stayActions.Add(ExecuteOrder);
            
            outOfCombat.enterActions.Add(SetPlayer);

            FSM groupFsm = new FSM(outOfCombat);
            StartCoroutine(FsmStayAlive(groupFsm));
        }

        #endregion

        #region FSM actions

        private void SetPlayer() => _target.SetTarget(GameObject.FindWithTag("Player").transform);

        private void ExecuteOrder()
        {
            foreach (ImpAi groupManagerImp in _groupManager.Imps.Values)
            {
                groupManagerImp.ExecuteTactic(_activeTactic);
            }
        }

        #endregion
        
        #region Event handlers

        private void OnImpJoined(GroupManager sender, GameObject impJoined)
        {
            impJoined.GetComponent<ContextSteering>().SetTarget(_target);
        }
        
        private void OnTryOrderAssign(TacticFactory newTactic, GroupManager.Group targetGroup)
        {
            if(targetGroup != _groupManager.ThisGroupName) return;

            _activeTactic = newTactic;
            
            OnTacticChanged?.Invoke(_activeTactic);
        }

        private void OnGlobalStartBattle(ArenaManager arenaManager)
        {
            _inBattle = true;
            _target.SetTarget(arenaManager.Boss.transform);    
        }

        private void OnGlobalEndBattle(ArenaManager arenaManager)
        {
            _inBattle = true;
        }

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