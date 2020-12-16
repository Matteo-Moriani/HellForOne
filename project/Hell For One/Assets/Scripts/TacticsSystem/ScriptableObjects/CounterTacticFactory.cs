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
        
    }

    public class CounterTactic : Tactic<CounterTacticData>
    {
        private ImpAi _impAi;

        public override void ExecuteTactic(ImpAi imp)
        {
            if(_impAi != null) return;

            _impAi = imp;
            
            imp.GetComponent<ImpCounterBehaviour>().StartCounter(data.TacticAttack.GetAttack());
        }


        public override void TerminateTactic(ImpAi imp)
        {
            _impAi = null;
            
            imp.GetComponent<ImpCounterBehaviour>().StopCounter();
        }
    }
}