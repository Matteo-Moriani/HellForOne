using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using UnityEngine;

namespace OrdersSystem.ScriptableObjects
{
    public class Order : ScriptableObject
    {
        [SerializeField] private AttackFactory orderAttack;
        [SerializeField, Min(0f)] private float blockChance;
        [SerializeField, Range(0f, 100f)] private float orderAggro;

        public float BlockChance
        {
            get => blockChance;
            private set => blockChance = value;
        }

        public AttackFactory OrderAttack
        {
            get => orderAttack;
            private set => orderAttack = value;
        }

        public float OrderAggro
        {
            get => orderAggro;
            private set => orderAggro = value;
        }
    }
}