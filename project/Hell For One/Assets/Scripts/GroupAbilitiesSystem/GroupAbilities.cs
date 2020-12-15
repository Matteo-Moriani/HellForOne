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
        public event Action OnStartGroupAbility;
        public event Action OnStopGroupAbility;

        private GroupManager _groupManager;

        private Coroutine _abilityCr = null;
        
        private void Awake()
        {
            _groupManager = transform.root.GetComponent<GroupManager>();
        }

        public void StartAbility(GroupAbility ability)
        {
            if(_abilityCr != null) return;
            
            foreach (Transform impsKey in _groupManager.Imps.Keys)
            {
                impsKey.GetComponent<ImpAbilities>().StartAbility();
            }
            
            Debug.Log("Starting Ability");
            
            _abilityCr = StartCoroutine(ability.DoGroupAbility(transform.root,StopAbility));
            
            OnStartGroupAbility?.Invoke();
        }

        private void StopAbility()
        {
            if(_abilityCr == null) return;
            
            foreach (Transform impsKey in _groupManager.Imps.Keys)
            {
                impsKey.GetComponent<ImpAbilities>().StopAbility();
            }
            
            StopCoroutine(_abilityCr);
            _abilityCr = null;
            
            OnStopGroupAbility?.Invoke();
        }
        
    }
}