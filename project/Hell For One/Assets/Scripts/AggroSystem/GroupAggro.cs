using AI.Imp;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace AggroSystem
{
    public class GroupAggro : MonoBehaviour
    {
        private ImpGroupAi _impGroupAi;

        private float _currentAggro = 0f;

        public float CurrentAggro
        {
            get => _currentAggro;
            private set => _currentAggro = value;
        }

        private void Awake() => _impGroupAi = GetComponent<ImpGroupAi>();

        private void OnEnable() => _impGroupAi.OnTacticChanged += OnTacticChanged;

        private void OnDisable() => _impGroupAi.OnTacticChanged -= OnTacticChanged;

        private void OnTacticChanged(TacticFactory newTacticFactory) =>
            _currentAggro = newTacticFactory.GetTactic().GetData().TacticAggro;
    }
}
