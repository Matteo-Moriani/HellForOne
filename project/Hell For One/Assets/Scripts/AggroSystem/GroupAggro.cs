using System.Collections.Generic;
using AI.Imp;
using GroupAbilitiesSystem;
using GroupAbilitiesSystem.ScriptableObjects;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace AggroSystem
{
    public class GroupAggro : MonoBehaviour
    {
        public static readonly List<GroupAggro> ForcedTargets = new List<GroupAggro>();
        
        private ImpGroupAi _impGroupAi;
        private GroupAbilities _groupAbilities;

        private float _currentAggro = 0f;

        public float CurrentAggro
        {
            get => _currentAggro;
            private set => _currentAggro = value;
        }
        
        private void Awake()
        {
            _impGroupAi = GetComponent<ImpGroupAi>();
            _groupAbilities = GetComponentInChildren<GroupAbilities>();
        }

        private void OnEnable()
        {
            _impGroupAi.OnTacticChanged += OnTacticChanged;
            _groupAbilities.OnStartGroupAbility += OnStartGroupAbility;
            _groupAbilities.OnStopGroupAbility += OnStopGroupAbility;
        }
        
        private void OnDisable()
        {
            _impGroupAi.OnTacticChanged -= OnTacticChanged;
            _groupAbilities.OnStartGroupAbility -= OnStartGroupAbility;
            _groupAbilities.OnStopGroupAbility -= OnStopGroupAbility;
        }

        private void OnStartGroupAbility(GroupAbilities arg1, GroupAbility arg2)
        {
            if(!arg2.GetData().ForceTargeting) return;
            if(ForcedTargets.Contains(this)) return;
            
            ForcedTargets.Add(this);
        }

        private void OnStopGroupAbility(GroupAbilities obj)
        {
            if(!ForcedTargets.Contains(this)) return;

            ForcedTargets.Remove(this);
        }

        private void OnTacticChanged(TacticFactory newTacticFactory) =>
            _currentAggro = newTacticFactory.GetTactic().GetData().TacticAggro;

        public void SetAggro(float newAggro) => _currentAggro = newAggro;
    }
}
