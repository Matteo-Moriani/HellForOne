using System;
using System.Collections.Generic;
using System.Linq;
using FactoryBasedCombatSystem;
using UnityEngine;

namespace ArenaSystem
{
    public class ArenaManager : MonoBehaviour
    {
        [SerializeField] private ArenaBoss arenaBoss;
        [SerializeField] private Transform playerStartPosition;
        
        private readonly Dictionary<IArenaObserver,bool> _observersState = new Dictionary<IArenaObserver, bool>();
        
        public static event Action<ArenaManager> OnGlobalSetupBattle;
        public static event Action<ArenaManager> OnGlobalStartBattle;
        public static event Action<ArenaManager> OnGlobalEndBattle;

        public event Action OnSetupBattle;
        public event Action OnStartBattle;
        public event Action OnEndBattle;

        public ArenaBoss Boss
        {
            get => arenaBoss;
            private set => arenaBoss = value;
        }

        public Transform PlayerStartPosition
        {
            get => playerStartPosition;
            private set => playerStartPosition = value;
        }

        private void Awake()
        {
            foreach (IArenaObserver observer in GetComponentsInChildren<IArenaObserver>())
            {
                _observersState.Add(observer,false);
            }
        }

        private void OnEnable() => arenaBoss.OnBossDeath += OnBossDeath;

        private void OnDisable() => arenaBoss.OnBossDeath += OnBossDeath;

        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag("Player")) return;
            
            foreach (IArenaObserver observer in other.GetComponentsInChildren<IArenaObserver>())
                _observersState.Add(observer,false);

            for (int i = 0; i < _observersState.Keys.Count; i++)
                _observersState.Keys.ToArray()[i].PrepareBattle(this);

            OnSetupBattle?.Invoke();
            OnGlobalSetupBattle?.Invoke(this);
        }

        public void NotifyBattlePrepared(IArenaObserver observer)
        {
            _observersState[observer] = true;
            
            if(_observersState.Any(item => item.Value != true)) return;

            foreach (IArenaObserver observersStateKey in _observersState.Keys)
            {
                observersStateKey.EnterBattle(this);
            }
            
            OnStartBattle?.Invoke();
            OnGlobalStartBattle?.Invoke(this);
        }
        
        private void OnBossDeath()
        {
            foreach (IArenaObserver observersStateKey in _observersState.Keys)
            {
                observersStateKey.ExitBattle(this);
            }
            
            OnEndBattle?.Invoke();
            OnGlobalEndBattle?.Invoke(this);

            gameObject.SetActive(false);
        }
    }
}