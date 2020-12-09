using System.Collections.Generic;
using ArenaSystem;
using UnityEngine;

namespace Managers
{
    public class EnemiesManager : MonoBehaviour
    {
        private ArenaBoss _currentBoss;

        private static EnemiesManager _instance;
        
        public ArenaBoss CurrentBoss 
        {   
            get => _currentBoss; 
            private set => _currentBoss = value; 
        }

        public static EnemiesManager Instance
        {
            get => _instance;
            private set => _instance = value;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        private void OnEnable()
        {
            ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        }

        private void OnDisable()
        {
            ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;
        }

        private void OnGlobalStartBattle(ArenaManager instance) => _currentBoss = instance.Boss;
    
        private void OnGlobalEndBattle(ArenaManager instance) => _currentBoss = null;
    }
}
