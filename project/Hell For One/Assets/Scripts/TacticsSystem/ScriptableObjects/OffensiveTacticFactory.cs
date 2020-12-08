using System;
using AI.Imp;
using Ai.MonoBT;
using AI.Movement;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using TacticsSystem.Interfaces;
using UnityEngine;

namespace TacticsSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "TacticsSystem/OffensiveTactic", fileName = "OffensiveTactic", order = 1)]
    public class OffensiveTacticFactory : TacticFactory<OffensiveTactic,OffensiveTacticData>
    {
        
    }

    [Serializable]
    public class OffensiveTacticData : TacticData
    {
        [SerializeField] private float stoppingDistance;
        [SerializeField] private float attackRateo;

        public float StoppingDistance
        {
            get => stoppingDistance;
            private set => stoppingDistance = value;
        }

        public float AttackRateo
        {
            get => attackRateo;
            private set => attackRateo = value;
        }
    }

    public class OffensiveTactic : Tactic<OffensiveTacticData>
    {
        private ContextSeek _contextSeek;
        private ContextGroupFormation _contextGroupFormation;
        private ContextObstacleAvoidance _contextObstacleAvoidance;        
        
        private CombatSystem _combatSystem;
        private ImpAi _impAi;
        private Attack _attackInstance;
        private BehaviourTree _tacticBehaviourTree;

        private bool _setUp = false;
        
        public override void ExecuteTactic(ImpAi imp)
        {
            if (_impAi == null)
                _impAi = imp;

            if (!_setUp)
            {
                foreach (ITacticsObserver tacticsObserver in imp.GetComponentsInChildren<ITacticsObserver>())
                {
                    tacticsObserver.StartTactic(this);
                }

                _setUp = true;
            }

            if (_tacticBehaviourTree == null)
            {
                BtAction setMovement = new BtAction(SetMovement);
                BtAction attack = new BtAction(Attack);
                
                BtSequence meleeAttack = new BtSequence(new IBtTask[]{setMovement,attack});
                
                _tacticBehaviourTree = new BehaviourTree(meleeAttack);
            }

            _tacticBehaviourTree.Run();
        }

        private bool SetMovement()
        {
            if (_contextGroupFormation == null)
                _contextGroupFormation = _impAi.GetComponent<ContextGroupFormation>();

            _contextGroupFormation.SetCloseness(data.TacticDistance);
            _contextGroupFormation.SetStoppingDistance(data.StoppingDistance);
            
            return true;
        }

        private bool Attack()
        {
            _impAi.GetComponent<ImpCombatBehaviour>().Attack(GameObject.FindWithTag("Boss").transform);

            return true;
        }

        public OffensiveTacticData GetOffensiveTacticData() => data;
    }
}