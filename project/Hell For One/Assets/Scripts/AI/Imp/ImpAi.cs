using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace AI.Imp
{
    public class ImpAi : MonoBehaviour
    {
        private TacticFactory _activeTactic = null;
        private Tactic _tacticInstance = null;
        
        public void ExecuteTactic(TacticFactory tactic)
        {
            if (_activeTactic == null || !_activeTactic.Equals(tactic))
            {
                _activeTactic = tactic;
                _tacticInstance = tactic.GetTactic();
            }

            _tacticInstance.ExecuteTactic(this);
        }
    }
}