using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class IdManager : MonoBehaviour
    {
        #region Fields

        // Dynamic list of bool
        // true -> id used
        // false -> id unused
        // if all ids in list are used, we generate a new id
        private static readonly List<bool> Ids = new List<bool>();

        private static IdManager _instance;

        public static IdManager Instance
        {
            get => _instance;
            private set => _instance = value;
        }

        #endregion

        #region Unity methods

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this);
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Get a free id or creates one if id is not present or all ids are occupied.
        /// </summary>
        /// <returns>The id</returns>
        public int GetId()
        {
            for (int i = 0; i < Ids.Count; i++)
            {
                if (Ids[i]) continue;
            
                Ids[i] = true;
                
                return i;
            }

            int id = Ids.Count;
        
            Ids.Add(true);

            return id;
        }

        /// <summary>
        /// Free an id in order to be reused.
        /// </summary>
        /// <param name="id">The id to free</param>
        public void FreeId(int id)
        {
            Ids[id] = false;
        }

        #endregion
    }
}