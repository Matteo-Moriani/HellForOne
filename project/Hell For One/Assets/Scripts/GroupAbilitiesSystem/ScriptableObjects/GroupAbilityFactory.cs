using System;
using System.Collections;
using TacticsSystem.ScriptableObjects;
using UnityEngine;

namespace GroupAbilitiesSystem.ScriptableObjects
{
    public abstract class GroupAbilityFactory : ScriptableObject
    {
        public abstract GroupAbility GetAbility();
    }

    public abstract class GroupAbilityFactory<TGroupAbility, TData> : GroupAbilityFactory
    where TGroupAbility : GroupAbility<TData>, new()
    where TData : GroupAbilityData
    {
        [SerializeField] private TData data;

        public override GroupAbility GetAbility() => new TGroupAbility
        {
            data = this.data,
            name = this.name
        };
    }

    [Serializable]
    public abstract class GroupAbilityData
    {
        [SerializeField] private int manaSegmentsCost;
        [SerializeField] private  TacticFactory associatedTactic;

        public int ManaSegmentsCost
        {
            get => manaSegmentsCost;
            private set => manaSegmentsCost = value;
        }

        public TacticFactory AssociatedTactic
        {
            get => associatedTactic;
            private set => associatedTactic = value;
        }
    }

    public abstract class GroupAbility
    {
        // TODO :- Wait positioning like animation events
        
        public string name;

        public IEnumerator DoGroupAbility(Transform groupTransform, Action stopAction)
        {
            Setup();

            yield return InnerDoGroupAbility(groupTransform);

            Dispose(stopAction);
        }

        private void Setup()
        {
            InnerSetup();
        }

        private void Dispose(Action stopAction)
        {
            InnerDispose();

            stopAction();
        }

        #region Abstract members

        protected abstract IEnumerator InnerDoGroupAbility(Transform groupTransform);
        public abstract GroupAbilityData GetData();
        protected abstract void InnerSetup();
        protected abstract void InnerDispose();

        #endregion
    }

    public abstract class GroupAbility<TData> : GroupAbility
        where TData : GroupAbilityData
    {
        public TData data;

        public override GroupAbilityData GetData() => data;
    }
}