﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AggroSystem;
using Ai.MonoBT;
using AI.Movement;
using ArenaSystem;
using CRBT;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using GroupSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.MidBoss
{
    public class MidBossAi : MonoBehaviour
    {
         #region Fields

        [Header("Ai")] 
        [SerializeField] private float fsmResolution = 1.0f;
        [SerializeField] private float behaviourTreeResolution = 0.5f;

        [Header("Search")] 
        [SerializeField]private float searchPlayerDuration = 10f;
        
        [Header("Combat")]
        [SerializeField] private BossAttackFactory[] attackFactories;
        [SerializeField] private float targetChangingRateo = 2f;
        [SerializeField] [Range(0.1f,1f)] private float maxFacingTolerance = 0.99f;
        [SerializeField] [Range(0.1f, 1f)] private float minFacingTolerance = 0.1f;
        [SerializeField] private bool adaptiveFacingTolerance = false;
        [SerializeField] [Range(0.0f,1f)] private float facingToleranceDecreasingRateo = 0f;

        private BossAttack[] _attackInstances;
        private BossAttack[] _currentAttacksInRange;
        
        private BossAttack _currentAttack;
        
        private MidBossCombatBehaviour _midBossCombatBehaviour;
        
        // TODO :- Create an aggro manager that stores group aggros and player aggro and manages different operation (e.g get max aggro)
        private List<GroupAggro> _groupAggros = new List<GroupAggro>();
        
        private ContextSeek _contextSeek;
        private ContextSteering _contextSteering;
        private ArenaBoss _arenaBoss;

        private readonly AiUtils.TargetData _targetData = new AiUtils.TargetData();

        private BehaviourTree _combatBehaviourTree;

        private FSM _bossFsm;

        private Coroutine _runBehaviourTreeCoroutine;
        private Coroutine _runFsmCoroutine;
        private Coroutine _searchCoroutine;
        private Coroutine _targetTimerCoroutine;

        private bool _currentTargetStillValid;
        private bool _lastAttackDone = true;
        private bool _inCombat;
        
        private float _currentTolerance;

        #endregion

        #region Unity methods

        private void Awake()
        {
            _contextSteering = GetComponent<ContextSteering>();
            _contextSeek = GetComponent<ContextSeek>();
            _midBossCombatBehaviour = GetComponent<MidBossCombatBehaviour>();
            _arenaBoss = GetComponent<ArenaBoss>();
            
            _attackInstances = new BossAttack[attackFactories.Length];
            
            for (int i = 0; i < attackFactories.Length; i++)
            {
                _attackInstances[i] = attackFactories[i].GetAttack();
            }
        }

        private void OnEnable()
        {
            _midBossCombatBehaviour.OnStartMidBossAttack += OnStartMidBossAttack;
            _midBossCombatBehaviour.OnStopMidBossAttack += OnStopMidBossAttack;

            _arenaBoss.Arena.OnStartBattle += OnStartBattle;
        }

        private void OnDisable()
        {
            _midBossCombatBehaviour.OnStartMidBossAttack -= OnStartMidBossAttack;
            _midBossCombatBehaviour.OnStopMidBossAttack -= OnStopMidBossAttack;
            
            _arenaBoss.Arena.OnStartBattle += OnStartBattle;
        }

        private void Start()
        {
            foreach (GameObject group in GroupsManager.Instance.Groups)
            {
                _groupAggros.Add(group.GetComponent<GroupAggro>());
            }

            #region Behaviour tree

            #region Target selection

            BtCondition isTargetStillValid = new BtCondition(() => _currentTargetStillValid);
            
            BtAction tryChooseByAggro = new BtAction(TryChooseByAggro);

            BtSequence byAggroSequence = new BtSequence(new IBtTask[]{tryChooseByAggro});
            
            BtSelector targetSelection = new BtSelector(new IBtTask[]{isTargetStillValid,byAggroSequence});

            #endregion

            #region Attack

            BtCondition lastAttackDone = new BtCondition(() => _lastAttackDone);
            BtCondition isFacingTarget = new BtCondition(IsFacingTarget);
            BtCondition inAttackRange = new BtCondition(InAttackRange);
            
            BtAction chooseAttack = new BtAction(ChooseAttack);
            BtAction attack = new BtAction(Attack);

            BtSequence attackSequence = new BtSequence(new IBtTask[]
            {
                lastAttackDone,
                isFacingTarget,
                inAttackRange,
                chooseAttack,
                attack
            });

            #endregion
            
            BtSequence combatSequence = new BtSequence(new IBtTask[]{targetSelection,attackSequence});
            
            #endregion

            #region FSM

            FSMState idle = new FSMState();
            FSMState combat = new FSMState();

            combat.enterActions.Add(StartCombat);
            
            FSMTransition battleEnter = new FSMTransition(() => _inCombat);
            
            idle.AddTransition(battleEnter,combat);
            
            #endregion
            
            _combatBehaviourTree = new BehaviourTree(combatSequence);
            
            _bossFsm = new FSM(idle);

            _runFsmCoroutine = StartCoroutine(BossFsmUpdate());
        }

        #endregion
        
        #region Behaviour tree

        #region Actions

        #region Target selection

        /// <summary>
        /// Sort players by aggro
        /// Use aggro as probability
        /// If a player pass probability test, set as target
        /// </summary>
        /// <returns></returns>
        private bool TryChooseByAggro()
        {
            // TODO :- Do a weighted probability like in attack choice
            
            GroupAggro selected = _groupAggros
                .Where(aggro => aggro.CurrentAggro > 0 && Random.Range(1f, 100f) <= aggro.CurrentAggro).
                OrderByDescending(aggro => aggro.CurrentAggro)
                .FirstOrDefault();

            if (selected == null)
                return false;

            _currentTargetStillValid = true;
            
            _targetData.SetTarget(selected.GetComponent<GroupManager>().GetRandomImp());

            _contextSteering.SetTarget(_targetData);

            return true;
        }

        #endregion
        
        #region Attack

        private bool Attack()
        {
            _midBossCombatBehaviour.Attack(_currentAttack);

            _currentTolerance = maxFacingTolerance;

            return true;
        }

        private bool ChooseAttack()
        {
            _currentAttack = null;
            
            float probabilitySum = _currentAttacksInRange.Aggregate(0f, (sum, next) => sum + next.GetBossAttackData().AttackProbability);

            float diceThrow = Random.Range(0f, probabilitySum);

            float temp = 0f;
            
            foreach (BossAttack alienAttack in _currentAttacksInRange.OrderByDescending(attack => attack.GetBossAttackData().AttackProbability))
            {
                float attackProbability = alienAttack.GetBossAttackData().AttackProbability;
                
                if (!(diceThrow >= temp && diceThrow <= temp + attackProbability))
                {
                    temp += attackProbability;
                    
                    continue;
                }

                _currentAttack = alienAttack;
                
                break;
            }

            return _currentAttack != null;
        }

        #endregion

        #endregion

        #region Conditions

        #region Attack

        private bool IsFacingTarget()
        {
            bool isFacing = Vector3.Dot(_targetData.GetDirectionToTarget(transform), transform.forward) >= _currentTolerance;

            if (!isFacing && adaptiveFacingTolerance)
            {
                _currentTolerance = Math.Max(minFacingTolerance, _currentTolerance - facingToleranceDecreasingRateo);
            }

            return isFacing;
        }
        
        private bool InAttackRange()
        {
            _currentAttacksInRange = null;
            
            float distance = _targetData.GetColliderDistanceFromTarget(transform);

            _currentAttacksInRange = _attackInstances.Where(attack => 
                distance >= attack.GetBossAttackData().MinDistance && 
                distance <= attack.GetBossAttackData().MaxDistance).ToArray();

            return _currentAttacksInRange.Length > 0;
        }

        #endregion

        #endregion

        #endregion

        #region FSM

        #region Fsm Actions

        private void StartCombat()
        {
            _contextSteering.enabled = true; 
            
            _contextSeek.ActivateBehaviour();
            
            _runBehaviourTreeCoroutine = StartCoroutine(RunBehaviourTree(_combatBehaviourTree));

            _targetTimerCoroutine = StartCoroutine(TargetSelectionHelper());
        }

        private void StopCombat()
        {
            _contextSteering.enabled = false;
            
            _contextSeek.DeactivateBehaviour();

            StopCoroutine(_runBehaviourTreeCoroutine);
            
            StopCoroutine(_targetTimerCoroutine);
            
            _runBehaviourTreeCoroutine = null;
        }

        #endregion

        #endregion

        #region Event handlers

        // private void OnZeroHp()
        // {
        //     StopAllCoroutines();
        //     
        //     BattleManager.Instance.NotifyBattleExit();
        // }

        private void OnStartBattle() => _inCombat = true;
        private void OnStartMidBossAttack() => _lastAttackDone = false;

        private void OnStopMidBossAttack() => _lastAttackDone = true;

        #endregion
        
        #region Coroutines

        private IEnumerator BossFsmUpdate()
        {
            while (true)
            {
                _bossFsm.Update();
                yield return new WaitForSeconds(fsmResolution);
            }
        }

        private IEnumerator RunBehaviourTree(BehaviourTree tree)
        {
            while (true)
            {
                tree.Run();
                yield return new WaitForSeconds(behaviourTreeResolution);
            }
        }

        private IEnumerator TargetSelectionHelper()
        {
            float timer = 0f;
            
            while (true)
            {
                yield return null;

                timer += Time.deltaTime;

                if (!(timer >= targetChangingRateo)) continue;
                
                _currentTargetStillValid = false;
                timer = 0f;
            }  
        }

        #endregion   
    }
}