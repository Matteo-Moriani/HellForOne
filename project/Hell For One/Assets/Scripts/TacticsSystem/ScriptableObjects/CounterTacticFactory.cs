using System;
using AI.Imp;
using Ai.MonoBT;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace TacticsSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "TacticsSystem/CounterTactic", fileName = "CounterTactic", order = 1)]
    public class CounterTacticFactory : TacticFactory<CounterTactic,CounterTacticData>
    {
        
    }

    [Serializable]
    public class CounterTacticData : TacticData
    {
        [SerializeField, Range(0f, 1f)] private float facingTolerance;

        public float FacingTolerance
        {
            get => facingTolerance;
            private set => facingTolerance = value;
        }
    }

    public class CounterTactic : Tactic<CounterTacticData>
    {
        private ImpAi _impAi;
        private BehaviourTree _tacticBehaviourTree;
        private ImpCombatBehaviour _impCombatBehaviour;
        private Attack _attackInstance;

        public override void ExecuteTactic(ImpAi imp)
        {
            if(_impAi != null) return;

            _impAi = imp;
            
            imp.GetComponent<ImpCounterBehaviour>().StartCounter(data.TacticAttack.GetAttack());

            if(_impCombatBehaviour == null)
                _impCombatBehaviour = _impAi.GetComponent<ImpCombatBehaviour>();

            if(_attackInstance == null)
                _attackInstance = data.TacticAttack.GetAttack();

            if(_tacticBehaviourTree == null)
            {
                BtCondition isFacingTarget = new BtCondition(IsFacingTarget);
                BtCondition inAttackRange = new BtCondition(InAttackRange);

                BtAction attack = new BtAction(Attack);

                BtSequence meleeAttack = new BtSequence(new IBtTask[] { isFacingTarget, inAttackRange, attack });

                _tacticBehaviourTree = new BehaviourTree(meleeAttack);
            }

            _tacticBehaviourTree.Run();
        }


        public override void TerminateTactic(ImpAi imp)
        {
            _impAi = null;
            
            imp.GetComponent<ImpCounterBehaviour>().StopCounter();
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
            _impCombatBehaviour.Attack(_attackInstance, _impAi.CurrentTargetData.Target);

            return true;
        }
    }
}