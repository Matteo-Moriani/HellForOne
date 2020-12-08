using System;
using Interfaces;
using ReincarnationSystem;
using TacticsSystem.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FactoryBasedCombatSystem
{
    public class Block : MonoBehaviour, IReincarnationObserver, IGroupOrdersObserver
    {
        #region Fields

        [SerializeField, Range(0f, 99f)] private float startingBlockChance;
        
        private float _blockChance;

        #endregion

        #region Unity Methods

        private void Awake() => _blockChance = Mathf.Clamp(startingBlockChance,0f,99f);

        #endregion

        #region Methods
        
        private void SetBlockChance(float newBlockChance) => _blockChance = Mathf.Clamp(newBlockChance,0f,99f);

        public bool TryBlock() => Random.Range(0, 99f) < _blockChance;

        #endregion

        #region Interfaces

        public void BecomeLeader() => SetBlockChance(0f);

        public void OnOrderGiven(Tactic newTactic) { }

        public void OnOrderAssigned(Tactic newTactic) => SetBlockChance(newTactic.GetData().TacticBlockChance);

        #endregion
    }
}