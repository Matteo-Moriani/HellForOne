using System;
using AI.Imp;
using Ai.MonoBT;
using AI.Movement;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace TacticsSystem.ScriptableObjects
{
    [CreateAssetMenu(menuName = "TacticsSystem/RecruitTactic", fileName = "RecruitTactic", order = 1)]
    public class RecruitTacticFactory : TacticFactory<RecruitTactic,RecruitTacticData>
    {
        
    }

    [Serializable]
    public class RecruitTacticData : TacticData
    {
    }

    public class RecruitTactic : Tactic<RecruitTacticData>
    {
        //private ContextGroupFormation _contextGroupFormation;
        private ImpAi _imp;
        private ImpRecruitBehaviour _impRecruitBehaviour;

        private BehaviourTree _recruitBehaviourTree;

        private bool _isRecruiting = false;
        
        public override void ExecuteTactic(ImpAi imp)
        {
            if (_imp == null)
            {
                _imp = imp;
                //_contextGroupFormation = imp.GetComponent<ContextGroupFormation>();
                _impRecruitBehaviour = imp.GetComponent<ImpRecruitBehaviour>();

                BtAction startRecruit = new BtAction(StartRecruit);
                BtAction stopRecruit = new BtAction(StopRecruit);
                //BtCondition inPositionCondition = new BtCondition(InPosition);
                BtCondition isRecruiting = new BtCondition(() => _isRecruiting);
                BtCondition isNotRecruiting = new BtCondition(() => !_isRecruiting);

                BtSelector startRecruitSelector = new BtSelector(new IBtTask[] {isRecruiting, startRecruit});
                BtSelector stopRecruitSelector = new BtSelector(new IBtTask[] {isNotRecruiting, stopRecruit});

                BtSequence startRecruitSequence =
                    new BtSequence(new IBtTask[] {startRecruitSelector});

                BtSelector recruitTree = new BtSelector(new IBtTask[] {startRecruitSequence, stopRecruitSelector});

                _recruitBehaviourTree = new BehaviourTree(recruitTree);
            }

            _recruitBehaviourTree.Run();
        }

        public override void TerminateTactic(ImpAi imp)
        {
            _impRecruitBehaviour.StopRecruit();
        }

        //private bool InPosition() => _contextGroupFormation.InPosition();

        private bool StartRecruit()
        {
            _impRecruitBehaviour.StartRecruit();

            _isRecruiting = true;
            
            return true;
        }

        private bool StopRecruit()
        {
            _impRecruitBehaviour.StopRecruit();

            _isRecruiting = false;
            
            return true;
        }
    }
}