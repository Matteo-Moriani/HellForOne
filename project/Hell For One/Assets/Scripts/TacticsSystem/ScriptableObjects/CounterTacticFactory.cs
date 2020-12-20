using System;
using AI.Imp;
using Ai.MonoBT;
using FactoryBasedCombatSystem;
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
        private ImpCounterBehaviour _impCounterBehaviour;
        private CombatSystem _impCombatSystem;
        private Attack _attackInstance;

        private bool _needsToAttack;

        public override void ExecuteTactic(ImpAi imp)
        {
            if (_impAi == null)
            {
                _impAi = imp;

                _impCounterBehaviour = imp.GetComponent<ImpCounterBehaviour>();
                
                _impCounterBehaviour.StartCounter();

                _impCombatSystem = imp.GetComponentInChildren<CombatSystem>();

                _impCombatSystem.OnBlockedHitReceived += OnBlockedHitReceived;

                _impCombatBehaviour = _impAi.GetComponent<ImpCombatBehaviour>();
                
                _attackInstance = data.TacticAttack.GetAttack();

                BtCondition needsToAttack = new BtCondition(() => _needsToAttack);
                BtCondition isFacingTarget = new BtCondition(IsFacingTarget);
                BtCondition inAttackRange = new BtCondition(InAttackRange);
                
                BtAction attack = new BtAction(Attack);

                BtSequence meleeAttack = new BtSequence(new IBtTask[] {needsToAttack, isFacingTarget, inAttackRange, attack});

                _tacticBehaviourTree = new BehaviourTree(meleeAttack);
            }

            _tacticBehaviourTree.Run();
        }


        public override void TerminateTactic(ImpAi imp)
        {
            imp.GetComponent<ImpCounterBehaviour>().StopCounter();
            _impCombatSystem.OnBlockedHitReceived -= OnBlockedHitReceived;
        }

        private void OnBlockedHitReceived(Attack arg1, CombatSystem arg2, Vector3 arg3) => _needsToAttack = true;

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

            _needsToAttack = false;
            
            return true;
        }
    }
}