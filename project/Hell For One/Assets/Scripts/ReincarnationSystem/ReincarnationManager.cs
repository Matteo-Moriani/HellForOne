using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ReincarnationSystem
{
    public class ReincarnationManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Reincarnation startLeader;

        private Reincarnation _currentLeader;

        private readonly List<Reincarnation> _reincarnables = new List<Reincarnation>();
        
        private static ReincarnationManager _instance;

        #endregion

        #region Events

        public static  event Action<Reincarnation> OnLeaderChanged;

        #endregion
        
        #region Properties

        public static ReincarnationManager Instance
        {
            get => _instance;
            private set => _instance = value;
        }

        public Reincarnation CurrentLeader
        {
            get => _currentLeader;
            private set => _currentLeader = value;
        }

        #endregion

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this);
        }

        private void Start()
        {
            AssignLeadership(startLeader);   
        }

        private void AssignLeadership(Reincarnation reincarnable)
        {
            _currentLeader = reincarnable;
            
            _currentLeader.GainLeadership();
            
            OnLeaderChanged?.Invoke(_currentLeader);
        }

        public void RegisterReincarnable(Reincarnation reincarnable)
        {
            if(_reincarnables.Contains(reincarnable)) return;

            reincarnable.OnReincarnableDeath += OnReincarnableDeath;
            
            _reincarnables.Add(reincarnable);
        }

        private void UnregisterReincarnable(Reincarnation reincarnable)
        {
            if(!_reincarnables.Contains(reincarnable)) return;
            
            reincarnable.OnReincarnableDeath -= OnReincarnableDeath;
            
            _reincarnables.Remove(reincarnable); 
        }

        private void OnReincarnableDeath(Reincarnation reincarnable)
        {
            UnregisterReincarnable(reincarnable);
            
            if(reincarnable != _currentLeader) return;
            
            reincarnable.LoseLeadership();
            AssignLeadership(_reincarnables[Random.Range(0,_reincarnables.Count)]);
        }
    }
}