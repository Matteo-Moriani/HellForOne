using System;
using AI.Imp;
using Ai.MonoBT;
using AI.Movement;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
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
        [SerializeField, Range(0f, 1f)] private float facingTolerance;
        [SerializeField] private float attackRateo;

        public float AttackRateo
        {
            get => attackRateo;
            private set => attackRateo = value;
        }

        public float FacingTolerance
        {
            get => facingTolerance;
            private set => facingTolerance = value;
        }
    }

    public class OffensiveTactic : Tactic<OffensiveTacticData>
    {
        private ImpCombatBehaviour _impCombatBehaviour;
        private ImpAi _impAi;
        private Attack _attackInstance;
        private BehaviourTree _tacticBehaviourTree;

        public override void ExecuteTactic(ImpAi imp)
        {
            if (_impAi == null)
                _impAi = imp;

            if (_impCombatBehaviour == null)
                _impCombatBehaviour = _impAi.GetComponent<ImpCombatBehaviour>();

            if (_attackInstance == null)
                _attackInstance = data.TacticAttack.GetAttack();

            if (_tacticBehaviourTree == null)
            {
                BtCondition isFacingTarget = new BtCondition(IsFacingTarget);
                BtCondition inAttackRange = new BtCondition(InAttackRange);
                
                BtAction attack = new BtAction(Attack);
                
                BtSequence meleeAttack = new BtSequence(new IBtTask[]{isFacingTarget,inAttackRange,attack});
                
                _tacticBehaviourTree = new BehaviourTree(meleeAttack);
            }

            _tacticBehaviourTree.Run();
        }

        private bool IsFacingTarget() => 
            Vector3.Dot(_impAi.CurrentTargetData.GetDirectionToTarget(_impAi.transform), _impAi.transform.forward) >= data.FacingTolerance;

        private bool InAttackRange()
        {
            float distance = _impAi.CurrentTargetData.GetColliderDistanceFromTarget(_impAi.transform);

            return distance >= _attackInstance.GetData().MinDistance &&
                   distance <= _attackInstance.GetData().MaxDistance;
        }

        private bool Attack()
        {
            _impCombatBehaviour.Attack(_attackInstance,_impAi.CurrentTargetData.Target);

            return true;
        }

        public OffensiveTacticData GetOffensiveTacticData() => data;
    }
}