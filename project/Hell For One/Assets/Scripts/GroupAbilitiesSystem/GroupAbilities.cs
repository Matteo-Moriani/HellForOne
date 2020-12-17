using System;
using ActionsBlockSystem;
using GroupAbilitiesSystem.ScriptableObjects;
using GroupSystem;
using UnityEngine;
using Utils;

namespace GroupAbilitiesSystem
{
    public class GroupAbilities : MonoBehaviour
    {
        public event Action<GroupAbilities,GroupAbility> OnStartGroupAbility;
        public event Action<GroupAbilities> OnStopGroupAbility;

        private GroupManager _groupManager;

        private Coroutine _abilityCr = null;

        private GroupAbility _currentAbility;
        
        private void Awake()
        {
            _groupManager = transform.root.GetComponent<GroupManager>();
        }

        public void StartAbility(GroupAbility ability)
        {
            if(_abilityCr != null) return;
            if(_currentAbility != null) return;

            foreach (Transform impsKey in _groupManager.Imps.Keys)
            {
                impsKey.GetComponent<ImpAbilities>().StartAbility(ability);
            }

            _currentAbility = ability;
            
            _abilityCr = StartCoroutine(ability.DoGroupAbility(transform.root,StopAbility));
            
            OnStartGroupAbility?.Invoke(this,ability);
        }

        private void StopAbility()
        {
            if(_abilityCr == null) return;
            if(_currentAbility == null) return;
            
            foreach (Transform impsKey in _groupManager.Imps.Keys)
            {
                impsKey.GetComponent<ImpAbilities>().StopAbility(_currentAbility);
            }

            _currentAbility = null;
            
            StopCoroutine(_abilityCr);
            _abilityCr = null;
            
            OnStopGroupAbility?.Invoke(this);
        }
    }
}