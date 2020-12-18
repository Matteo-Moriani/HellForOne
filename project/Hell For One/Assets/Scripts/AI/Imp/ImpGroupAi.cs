using System;
using System.Collections;
using System.Linq;
using ActionsBlockSystem;
using Ai.MonoBT;
using AI.Movement;
using ArenaSystem;
using CRBT;
using FactoryBasedCombatSystem.Interfaces;
using GroupAbilitiesSystem;
using GroupAbilitiesSystem.ScriptableObjects;
using GroupSystem;
using ReincarnationSystem;
using TacticsSystem;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace AI.Imp
{
    public class ImpGroupAi : MonoBehaviour
    {
        #region Fields
        
        [SerializeField] private TacticFactory startTactic;

        private GroupManager _groupManager;
        private GroupAbilities _groupAbilities;
        
        private TacticFactory _activeTactic;
        private GroupAbility _activeAbility;
        
        private readonly AiUtils.TargetData _target = new AiUtils.TargetData();
        private bool _inBattle;

        private BehaviorTree _combatBhBehaviorTree;
        
        #endregion

        #region Events

        public event Action<TacticFactory> OnTacticChanged;
        public static event Action<TacticFactory,GroupManager.Group> OnTacticChangedGlobal;

        public AiUtils.TargetData Target => _target;

        #endregion
        
        #region Unity Methods

        private void Awake()
        {
            _groupManager = GetComponent<GroupManager>();
            _groupAbilities = GetComponentInChildren<GroupAbilities>();
            
            _activeTactic = startTactic;
        }

        private void OnEnable()
        {
            LeaderTactics.OnTryTacticAssign += OnTryTacticAssign;

            _groupAbilities.OnStopGroupAbility += OnStopGroupAbility;
            
            ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        }

        private void OnDisable()
        {
            LeaderTactics.OnTryTacticAssign -= OnTryTacticAssign;

            _groupAbilities.OnStopGroupAbility -= OnStopGroupAbility;
            
            ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;
        }

        private void Start()
        {
            BtAction executeTactic = new BtAction(ExecuteTactic);
            BtAction ability = new BtAction(() => _activeAbility != null);
            BtAction allInPosition = new BtAction(() => 
                !_activeAbility.GetData().InPositionBeforeActivation ||
                _groupManager.Imps.All(pair => pair.Key.GetComponent<ContextGroupFormation>().InPosition()));
            BtAction doAbility = new BtAction(ExecuteAbility);

            BtSequence combatSequence =
                new BtSequence(new IBtTask[] {executeTactic, ability, allInPosition, doAbility});
            
            FSMState outOfCombat = new FSMState();
            FSMState inCombat = new FSMState();
            
            FSMTransition battleEnter = new FSMTransition( () => _inBattle);
            FSMTransition battleExit = new FSMTransition( () => !_inBattle);
            
            outOfCombat.AddTransition(battleEnter,inCombat);
            inCombat.AddTransition(battleExit,outOfCombat);
            
            inCombat.enterActions.Add(() => OnTacticChanged?.Invoke(_activeTactic));
            inCombat.stayActions.Add(() => combatSequence.Run());
            
            outOfCombat.stayActions.Add(SetPlayer);

            FSM groupFsm = new FSM(outOfCombat);
            StartCoroutine(FsmStayAlive(groupFsm));
        }

        #endregion

        #region Methods

        private void AssignTactic(TacticFactory newTactic)
        {
            if(_activeAbility != null) return;
            
            if(newTactic == _activeTactic) return;
            
            _activeTactic = newTactic;
            
            OnTacticChanged?.Invoke(_activeTactic);
            OnTacticChangedGlobal?.Invoke(newTactic,_groupManager.ThisGroupName);
        }

        public bool TryAbility(GroupAbility ability)
        {
            if(_activeAbility != null) return false;

            AssignTactic(ability.GetData().AssociatedTactic);

            _activeAbility = ability;

            return true;   
        }

        #endregion

        #region Behaviour tree actions

        private bool ExecuteTactic()
        {
            foreach (ImpAi groupManagerImp in _groupManager.Imps.Values)
            {
                groupManagerImp.ExecuteTactic(_activeTactic);
            }

            return true;
        }

        private bool ExecuteAbility()
        {
            _groupAbilities.StartAbility(_activeAbility);

            return true;
        }

        #endregion
        
        #region FSM actions

        private void SetPlayer()
        {
            ReincarnableBehaviour leader = ReincarnationManager.Instance.CurrentLeader;
            
            if(leader == null || _target.Target == leader.transform) return;
            
            _target.SetTarget(ReincarnationManager.Instance.CurrentLeader.transform);
        }
        
        #endregion
        
        #region Event handlers

        private void OnTryTacticAssign(TacticFactory newTactic, GroupManager.Group targetGroup)
        {
            if(targetGroup != _groupManager.ThisGroupName && targetGroup != GroupManager.Group.All) return;

            AssignTactic(newTactic);
        }

        private void OnStopGroupAbility(GroupAbilities groupAbilities)
        {
            _activeAbility = null;
        }
        
        private void OnGlobalStartBattle(ArenaManager arenaManager)
        {
            _inBattle = true;
            _target.SetTarget(arenaManager.Boss.transform);    
        }

        private void OnGlobalEndBattle(ArenaManager arenaManager)
        {
            _inBattle = false;
        }

        #endregion

        // #region Interfaces
        //
        // public void Block() => _changeTacticLock.AddLock();
        //
        // public void Unblock() => _changeTacticLock.RemoveLock();
        //
        // public UnitActionsBlockManager.UnitAction GetAction() => UnitActionsBlockManager.UnitAction.ChangeTactic;
        //
        // #endregion
        
        #region Coroutines

        private IEnumerator FsmStayAlive(FSM fsm)
        {
            while (true)
            {
                fsm.Update();
                yield return null;
            }
        }

        #endregion
    }
}