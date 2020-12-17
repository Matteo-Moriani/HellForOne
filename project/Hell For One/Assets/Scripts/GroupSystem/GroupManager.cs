using System;
using System.Collections.Generic;
using System.Linq;
using AI.Imp;
using FactoryBasedCombatSystem;
using ReincarnationSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GroupSystem
{
    /// <summary>
    /// Class that manages the group mechanic
    /// </summary>
    public class GroupManager : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Enum that lists the aviable groups
        /// </summary>
        public enum Group
        {
            GroupAzure,
            GroupPink,
            GroupGreen,
            GroupYellow,
            All,
            None
        }

        [SerializeField] [Tooltip("Field that indicate which group this is")] private Group thisGroupName = Group.None;

        [SerializeField] [Tooltip("The color of this group")] private Color groupColor = Color.white;
    
        private Dictionary<Transform,ImpAi> _imps = new Dictionary<Transform, ImpAi>();

        private int maxImpNumber = 4;
    
        #endregion
    
        #region properties
        
        public Group ThisGroupName { get => thisGroupName; private set => thisGroupName = value; }
        public Dictionary<Transform,ImpAi> Imps { get => _imps; private set => _imps = value; }
        
        public Color GroupColor { get => groupColor; set => groupColor = value; }
        public Material groupColorMat;
        
        #endregion

        #region Delegates and events
        
        public event Action<GroupManager,GameObject> OnImpJoined;
        public event Action<GameObject> OnImpRemoved;

        #endregion

        #region Unity methods

        private void OnEnable()
        {
            ImpDeath.OnImpDeath += OnImpDeath;
            ReincarnationManager.OnLeaderReincarnated += OnLeaderReincarnated;
        }

        private void OnDisable()
        {
            ImpDeath.OnImpDeath -= OnImpDeath;
            ReincarnationManager.OnLeaderReincarnated -= OnLeaderReincarnated;
        }

        #endregion
        
        #region Methods

        public Transform GetRandomImp() => _imps.Keys.ToList()[Random.Range(0, _imps.Keys.Count)];

        public bool IsEmpty() => _imps.Keys.Count == 0;

        public void AddDemonToGroup(Transform imp)
        {
            if (_imps.Count >= maxImpNumber) return;
            
            _imps.Add(imp,imp.GetComponent<ImpAi>());

            foreach (IGroupObserver observer in imp.GetComponentsInChildren<IGroupObserver>())
            {
                observer.JoinGroup(this);
            }
            
            OnImpJoined?.Invoke(this,imp.gameObject);
        }

        private void RemoveImp(Transform imp)
        {
            if(_imps.ContainsKey(imp))
                _imps.Remove(imp);

            foreach (IGroupObserver observer in imp.GetComponentsInChildren<IGroupObserver>())
            {
                observer.LeaveGroup(this);
            }
            
            OnImpRemoved?.Invoke(imp.gameObject);
        }
    
        #endregion

        #region Event handlers

        private void OnLeaderReincarnated(ReincarnableBehaviour newLeader) => RemoveImp(newLeader.transform);

        private void OnImpDeath(Transform deadImp) => RemoveImp(deadImp);

        #endregion
    }
}
