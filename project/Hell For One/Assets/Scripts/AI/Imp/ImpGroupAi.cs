using System;
using System.Collections;
using OrdersSystem;
using OrdersSystem.ScriptableObjects;
using UnityEngine;

namespace AI.Imp
{
    public class ImpGroupAi : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GroupManager.Group group;
        [SerializeField] private float fsmReactionTime = 0.2f;
        [SerializeField] private Order startOrder;

        private GroupManager _groupManager;
        
        private Order _activeOrder;
        private Transform _target;
        private bool _inBattle;

        #endregion

        #region Events

        public event Action<Order> OnOrderChanged;

        #endregion
        
        #region Unity Methods

        private void Awake()
        {
            _groupManager = GetComponent<GroupManager>();
            
            _activeOrder = startOrder;
        }

        private void OnEnable()
        {
            TacticsManager.OnTryOrderAssign += OnTryOrderAssign;
            
            BattleEventsManager.onBattleEnter += OnBattleEnter;
            BattleEventsManager.onBattleExit += OnBattleExit;
        }

        private void OnDisable()
        {
            TacticsManager.OnTryOrderAssign -= OnTryOrderAssign;
            
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
            
            inCombat.enterActions.Add( () => _target = GameObject.FindWithTag("Boss").transform);
            inCombat.stayActions.Add(ExecuteOrder);
            inCombat.exitActions.Add(() => _target = null);
            
            FSM groupFsm = new FSM(outOfCombat);
            StartCoroutine(FsmStayAlive(groupFsm));
        }

        #endregion

        #region FSM actions

        private void ExecuteOrder()
        {
            foreach (GameObject groupManagerImp in _groupManager.Imps)
            {
                AllyImpMovement allyImpMovement = groupManagerImp.GetComponent<AllyImpMovement>();
                
                if(!allyImpMovement.CanAct()) return;
                
                // TODO :- Execute order call
            }
        }

        #endregion
        
        #region Event handlers

        private void OnTryOrderAssign(Order newOrder, GroupManager.Group targetGroup)
        {
            if(targetGroup != this.group) return;

            _activeOrder = newOrder;
            
            OnOrderChanged?.Invoke(_activeOrder);
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