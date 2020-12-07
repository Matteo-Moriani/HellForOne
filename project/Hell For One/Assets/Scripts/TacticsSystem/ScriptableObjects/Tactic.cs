using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace TacticsSystem.ScriptableObjects
{
    public class Tactic : ScriptableObject
    {
        [SerializeField] private AttackFactory tacticAttack;
        [SerializeField, Min(0f)] private float tacticBlockChance;
        [SerializeField, Range(0f, 100f)] private float tacticAggro;
        [SerializeField, Min(0f)] private float tacticDistance;

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

        public void ExecuteOrder()
        {
        }
    }
}