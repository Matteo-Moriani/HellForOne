using System;
using AI.Imp;
using Ai.MonoBT;
using AI.Movement;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace TacticsSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "TacticsSystem/MeleeAttackTactic", fileName = "MeleeAttackTactic", order = 1)]
    public class MeleeAttackTacticFactory : TacticFactory<MeleeAttackTactic,MeleeAttackTacticData>
    {
        
    }

    [Serializable]
    public class MeleeAttackTacticData : TacticData
    {
        [SerializeField] private float stoppingDistance;

        public float StoppingDistance
        {
            get => stoppingDistance;
            private set => stoppingDistance = value;
        }
    }

    public class MeleeAttackTactic : Tactic<MeleeAttackTacticData>
    {
        private ContextSeek _contextSeek;
        private ContextGroupFormation _contextGroupFormation;
        private ContextObstacleAvoidance _contextObstacleAvoidance;        
        
        private CombatSystem _combatSystem;
        private ImpAi _impAi;
        private Attack _attackInstance;
        private BehaviourTree _tacticBehaviourTree;
        
        public override void ExecuteTactic(ImpAi imp)
        {
            if (_impAi == null)
                _impAi = imp;

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
            // if (_contextSeek == null)
            //     _contextSeek = _impAi.GetComponent<ContextSeek>();
            //
            // if(!_contextSeek.SteeringBehaviourLock.CanDoAction())
            //     _contextSeek.ActivateBehaviour();

            if (_contextGroupFormation == null)
                _contextGroupFormation = _impAi.GetComponent<ContextGroupFormation>();
            
            // if(_contextGroupFormation.SteeringBehaviourLock.CanDoAction())
            //     _contextGroupFormation.DeactivateBehaviour();
            //
            // if (_contextObstacleAvoidance == null)
            //     _contextObstacleAvoidance = _impAi.GetComponent<ContextObstacleAvoidance>();
            //
            // _contextObstacleAvoidance.SetLayerMask(LayerMask.GetMask("Player","InvisibleWalls","AlliesAvoidance"));
            
            //_contextSeek.SetStoppingDistance(data.TacticDistance);
            
            _contextGroupFormation.SetCloseness(data.TacticDistance);
            _contextGroupFormation.SetStoppingDistance(data.StoppingDistance);
            
            return true;
        }

        private bool Attack()
        {
            if (_combatSystem == null)
                _combatSystem = _impAi.GetComponentInChildren<CombatSystem>();
            if (_attackInstance == null)
                _attackInstance = data.TacticAttack.GetAttack();
            
            _combatSystem.StartAttack(_attackInstance, GameObject.FindWithTag("Boss").transform);

            return true;
        }
    }
}