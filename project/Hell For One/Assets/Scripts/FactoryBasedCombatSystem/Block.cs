using System;
using Interfaces;
using OrdersSystem.ScriptableObjects;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class Block : MonoBehaviour, IGroupOrdersObserver
    {
        [SerializeField, Min(0f)] private float startBlockChance;

        private float _currentBlockChance;

        public bool TryBlock()
        {
            return false;
        }

        private void Awake()
        {
            _currentBlockChance = startBlockChance;
        }

        public void ProcessOrderChanged(Oder newOrder)
        {
            _currentBlockChance = newOrder.BlockChance;
        }
    }
}