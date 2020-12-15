using System;
using AI.Imp;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace TacticsSystem.ScriptableObjects
{
    public abstract class TacticFactory : ScriptableObject
    {
        public abstract Tactic GetTactic();
    }

    public abstract class TacticFactory<TTactic, TTacticData> : TacticFactory
        where TTactic : Tactic<TTacticData>, new()
        where TTacticData : TacticData
    {
        [SerializeField]
        private TTacticData data;

        public override Tactic GetTactic() => new TTactic
        {
            data = this.data, 
            name = this.name
        };
    }
    
    [Serializable]
    public abstract class TacticData
    {
        [SerializeField] private AttackFactory tacticAttack;
        [SerializeField, Min(0f)] private float tacticBlockChance;
        [SerializeField, Range(0f, 100f)] private float tacticAggro;
        [SerializeField, Min(0f)] private float tacticDistance;
        [SerializeField] private float stoppingDistance;
        
        public float TacticBlockChance
        {
            get => tacticBlockChance;
            private set => tacticBlockChance = value;
        }

        public AttackFactory TacticAttack
        {
            get => tacticAttack;
            private set => tacticAttack = value;
        }

        public float TacticAggro
        {
            get => tacticAggro;
            private set => tacticAggro = value;
        }

        public float TacticDistance
        {
            get => tacticDistance;
            private set => tacticDistance = value;
        }

        public float StoppingDistance
        {
            get => stoppingDistance;
            private set => stoppingDistance = value;
        }
    }

    public abstract class Tactic
    {
        public string name;
        
        public abstract void ExecuteTactic(ImpAi imp);
        public abstract void TerminateTactic(ImpAi imp);
        public abstract TacticData GetData();
    }

    public abstract class Tactic<TData> : Tactic
    where TData : TacticData
    {
        public TData data;

        public override TacticData GetData() => data;
    }
}