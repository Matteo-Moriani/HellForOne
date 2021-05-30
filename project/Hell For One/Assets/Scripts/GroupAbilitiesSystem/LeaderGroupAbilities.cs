using System;
using System.Collections;
using ActionsBlockSystem;
using AggroSystem;
using AI.Imp;
using CrownSystem;
using GroupAbilitiesSystem.ScriptableObjects;
using GroupSystem;
using ManaSystem;
using Player;
using UnityEngine;

namespace GroupAbilitiesSystem
{
    public class LeaderGroupAbilities : MonoBehaviour, IActionsBlockObserver, IPlayerAggroSubject, ICrownObserver
    {
        #region Fields

        [SerializeField] private float useAbilityAggro;
        
        [SerializeField] private GroupAbilityFactory aButtonAbility;
        [SerializeField] private GroupAbilityFactory bButtonAbility;
        [SerializeField] private GroupAbilityFactory xButtonAbility;
        [SerializeField] private GroupAbilityFactory yButtonAbility;
        
        private ImpMana _impMana;

        private readonly ActionLock _abilitiesLock = new ActionLock();

        #endregion

        #region Events

        //public static event Action<GroupAbility, GroupManager.Group> OnTryAbility;
        public static event Action<GroupAbility> OnAbilityFailed;
        public static event Action<GroupAbility> OnAbilitySuccess;
        
        #endregion

        #region Unity Methods

        private void Awake()
        {
            _impMana = GetComponent<ImpMana>();
            
            // One lock for Leader mechanic
            _abilitiesLock.AddLock();
            // One lock because we need to wait for LT press
            _abilitiesLock.AddLock();
        }

        #endregion

        #region Methods

        private void TryAbility(GroupAbilityFactory groupAbilityFactory)
        {
            if (!_abilitiesLock.CanDoAction()) return;
            
            if (GroupsInRangeDetector.MostRepresentedGroupInRange == GroupManager.Group.None ||
                GroupsInRangeDetector.MostRepresentedGroupInRange == GroupManager.Group.All) return;
            
            GroupAbility ability = groupAbilityFactory.GetAbility();

            if (!_impMana.CheckSegments(ability.GetData().ManaSegmentsCost)) return;

            //Debug.Log("Segments OK");
            
            if (!GroupsManager.Instance.Groups[GroupsInRangeDetector.MostRepresentedGroupInRange]
                .GetComponent<ImpGroupAi>().TryAbility(ability)) return;
            
            //Debug.Log("Ability OK");

            _impMana.SpendSegments(ability.GetData().ManaSegmentsCost);
            
            OnAggroActionDone?.Invoke(useAbilityAggro);
        }
        
        #endregion

        #region Event Handlers

        private void OnBButtonDown() => TryAbility(bButtonAbility);

        //private void OnAButtonDown() => TryAbility(aButtonAbility);

        private void OnYButtonDown() => TryAbility(yButtonAbility);

        private void OnXButtonDown() => TryAbility(xButtonAbility);

        private void OnLTButtonUp() => _abilitiesLock.AddLock();

        private void OnLTButtonDown() => _abilitiesLock.RemoveLock();

        #endregion

        #region Interfaces
        
        public event Action<float> OnAggroActionDone;

        public void Block() => _abilitiesLock.AddLock();

        public void Unblock() => _abilitiesLock.RemoveLock();

        public UnitActionsBlockManager.UnitAction GetAction() => UnitActionsBlockManager.UnitAction.UseAbilities;

        public void OnCrownCollected()
        {
            _abilitiesLock.RemoveLock();
            
            PlayerInput.OnLTButtonDown += OnLTButtonDown;
            PlayerInput.OnLTButtonUp += OnLTButtonUp;
            
            PlayerInput.OnXButtonDown += OnXButtonDown;
            PlayerInput.OnYButtonDown += OnYButtonDown;
            //PlayerInput.OnAButtonDown += OnAButtonDown;
            PlayerInput.OnBButtonDown += OnBButtonDown;
        }

        public void OnCrownLost()
        {
            _abilitiesLock.AddLock();
            
            PlayerInput.OnLTButtonDown -= OnLTButtonDown;
            PlayerInput.OnLTButtonUp -= OnLTButtonUp;
            
            PlayerInput.OnXButtonDown -= OnXButtonDown;
            PlayerInput.OnYButtonDown -= OnYButtonDown;
            //PlayerInput.OnAButtonDown -= OnAButtonDown;
            PlayerInput.OnBButtonDown -= OnBButtonDown;
        }

        #endregion
    }
}