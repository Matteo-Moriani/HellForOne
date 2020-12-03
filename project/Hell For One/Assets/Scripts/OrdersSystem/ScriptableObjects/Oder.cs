using UnityEngine;

namespace OrdersSystem.ScriptableObjects
{
    public class Oder : ScriptableObject
    {
        [SerializeField, Min(0f)] private float blockChance;

        public float BlockChance
        {
            get => blockChance;
            private set => blockChance = value;
        }
    }
}