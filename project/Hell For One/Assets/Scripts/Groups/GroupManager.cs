using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Groups
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

        [SerializeField]
        [Tooltip("Field that indicate wich group this is")]
        private Group thisGroupName = Group.None;

        [SerializeField]
        [Tooltip("The color of this group")]
        private Color groupColor = Color.white;
    
        private List<Transform> _imps = new List<Transform>();
        
        private int maxImpNumber = 4;
    
        #endregion
    
        #region properties

        /// <summary>
        /// Property that idicates wich group this is
        /// </summary>
        public Group ThisGroupName { get => thisGroupName; private set => thisGroupName = value; }

        /// <summary>
        /// Imps in this group
        /// </summary>
        public List<Transform> Imps { get => _imps; private set => _imps = value; }
        
        public Color GroupColor { get => groupColor; set => groupColor = value; }
        public Material groupColorMat;
        
        #endregion

        #region Delegates and events
        
        public event Action<GroupManager,GameObject> OnImpJoined;
        public event Action<GameObject> OnImpRemoved;

        #endregion

        #region Methods
        
        public bool IsEmpty() => _imps.Count == 0;
        
        public Transform GetRandomImp() => _imps[Random.Range(0, _imps.Count)];
        
        public bool AddDemonToGroup(Transform imp)
        {
            if (_imps.Count >= maxImpNumber) return false;
            
            _imps.Add(imp);

            // TODO :- Check if this is needed
            imp.GetComponent<Reincarnation>().onLateReincarnation += OnLateReincarnation;
            imp.GetComponent<Stats>().onLateDeath += OnLateDeath;

            OnImpJoined?.Invoke(this,imp.gameObject);
            
            return true;
        }

        private void RemoveImp(Transform imp)
        {
            _imps.Remove(imp);

            imp.GetComponent<Reincarnation>().onLateReincarnation -= OnLateReincarnation;
            imp.GetComponent<Stats>().onLateDeath -= OnLateDeath;
        
            OnImpRemoved?.Invoke(imp.gameObject);
        }
    
        #endregion

        #region Event handlers

        private void OnLateDeath(Stats sender)
        {
            RemoveImp(sender.transform);
        }

        private void OnLateReincarnation(GameObject newPlayer)
        {
            RemoveImp(newPlayer.transform);
        }

        #endregion
    }
}
