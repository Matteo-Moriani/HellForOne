using System;
using System.Collections.Generic;
using System.Linq;
using FactoryBasedCombatSystem;
using GroupSystem;
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

            foreach (GameObject group in GroupsManager.Instance.Groups)
            foreach (Transform impsKey in group.GetComponent<GroupManager>().Imps.Keys)
            foreach (IArenaObserver componentsInChild in impsKey.GetComponentsInChildren<IArenaObserver>())
                _observersState.Add(componentsInChild,false);   
            
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