using System;
using ReincarnationSystem;
using TacticsSystem.Interfaces;
using TacticsSystem.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FactoryBasedCombatSystem
{
    public class Block : MonoBehaviour, IReincarnationObserver, ITacticsObserver
    {
        #region Fields

        [SerializeField, Range(0f, 99f)] private float startingBlockChance;
        
        private float _blockChance;
        private ITacticsObserver _tacticsObserverImplementation;

        #endregion

        #region Unity Methods

        private void Awake() => _blockChance = Mathf.Clamp(startingBlockChance,0f,99f);

        #endregion

        #region Methods
        
        private void SetBlockChance(float newBlockChance) => _blockChance = Mathf.Clamp(newBlockChance,0f,99f);

        public bool TryBlock() => Random.Range(0, 99f) < _blockChance;

        #endregion

        #region Interfaces

        public void StartTactic(Tactic newTactic) => SetBlockChance(newTactic.GetData().TacticBlockChance);
        
        public void EndTactic(Tactic oldTactic) => SetBlockChance(0f);

        public void StartLeader() => SetBlockChance(0f);

        public void StopLeader() { }
        
        #endregion
    }
}