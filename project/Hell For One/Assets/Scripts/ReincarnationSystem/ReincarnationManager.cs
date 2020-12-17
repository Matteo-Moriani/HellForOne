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

        [SerializeField] private ReincarnableBehaviour startLeader;

        private ReincarnableBehaviour _currentLeader;

        private readonly List<ReincarnableBehaviour> _reincarnables = new List<ReincarnableBehaviour>();
        
        private static ReincarnationManager _instance;

        #endregion

        #region Events

        public static event Action<ReincarnableBehaviour> OnLeaderDeath;
        public static  event Action<ReincarnableBehaviour> OnLeaderReincarnated;

        #endregion
        
        #region Properties

        public static ReincarnationManager Instance
        {
            get => _instance;
            private set => _instance = value;
        }

        public ReincarnableBehaviour CurrentLeader
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

        private void AssignLeadership(ReincarnableBehaviour reincarnable)
        {
            _currentLeader = reincarnable;
            
            _currentLeader.GainLeadership();
            
            OnLeaderReincarnated?.Invoke(_currentLeader);
        }

        public void RegisterReincarnable(ReincarnableBehaviour reincarnable)
        {
            if(_reincarnables.Contains(reincarnable)) return;

            reincarnable.OnReincarnableDeath += OnReincarnableDeath;
            
            _reincarnables.Add(reincarnable);
        }

        private void UnregisterReincarnable(ReincarnableBehaviour reincarnable)
        {
            if(!_reincarnables.Contains(reincarnable)) return;
            
            reincarnable.OnReincarnableDeath -= OnReincarnableDeath;
            
            _reincarnables.Remove(reincarnable); 
        }

        private void OnReincarnableDeath(ReincarnableBehaviour reincarnable)
        {
            UnregisterReincarnable(reincarnable);
            
            if(reincarnable != _currentLeader) return;
            
            reincarnable.LoseLeadership();
            
            OnLeaderDeath?.Invoke(_currentLeader);
            
            AssignLeadership(_reincarnables[Random.Range(0,_reincarnables.Count)]);
        }
    }
}