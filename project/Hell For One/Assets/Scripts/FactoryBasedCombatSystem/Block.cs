using System;
using AI.Imp;
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
        
        [SerializeField] private float _blockChance;
        private ITacticsObserver _tacticsObserverImplementation;

        #endregion

        #region Unity Methods

        private void Awake() => _blockChance = Mathf.Clamp(startingBlockChance,0f,100f);

        #endregion

        #region Methods

        private void SetBlockChance(float newBlockChance)
        {
            _blockChance = newBlockChance;
            if(_blockChance > 100f)
                _blockChance = 100f;
        }

        //=> _blockChance = Mathf.Clamp(newBlockChance, 0f, 100f);

        public bool TryBlock()
        {
            // questa cosa orribile e' perche' a volte contro igni gli imp non facevano lo StartTactic e la block chance rimaneva zero
            if(_blockChance == 0f && transform.root.gameObject.layer != LayerMask.NameToLayer("Player"))
                _blockChance = transform.root.gameObject.GetComponent<ImpAi>().TacticInstance.GetData().TacticBlockChance;

            float random = Random.Range(0f, 100f);
            //Debug.Log(random + " " + _blockChance);
            return random <= _blockChance;
        }

        //=> Random.Range(0f, 100f) <= _blockChance;

        #endregion

        #region Interfaces

        public void StartTactic(Tactic newTactic)
        {
            //Debug.Log("started tactic " + newTactic.name);
            SetBlockChance(newTactic.GetData().TacticBlockChance);
        }
        
        public void EndTactic(Tactic oldTactic) => SetBlockChance(0f);

        public void StartLeader() => SetBlockChance(0f);

        public void StopLeader() { }
        
        #endregion
    }
}