using System.Collections.Generic;
using UnityEngine;

namespace Utils.ObjectPooling
{
    /// <summary>
    /// Class that manages poolers creation, memorization and referentiation.
    /// Poolers are created dynamically only when needed.
    /// When starting on a new client, all poolers instantiated on the server
    /// will be instantiated on the new client too, together with active pooled objects.
    /// </summary>
    public class PoolersManager : MonoBehaviour
    {
        [SerializeField]
        private int startingPooledObjects = 0;
        
        private static PoolersManager _instance;
        
        private Dictionary<GameObject, Pooler> _poolers;

        public static PoolersManager Instance
        {
            get => _instance;
            private  set => _instance = value;
        }

        /// <summary>
        /// With how many GameObject a pooler will start.
        /// </summary>
        public int StartingPooledObjects
        {
            get => startingPooledObjects;
            private set => startingPooledObjects = value;
        }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this);
            
            _poolers = new Dictionary<GameObject, Pooler>();
        }

        /// <summary>
        /// If it exist, returns a pooler, else instantiate and returns it. 
        /// </summary>
        /// <param name="prefab">The GameObject to pool</param>
        /// <returns>The pooler</returns>
        public Pooler TryGetPooler(GameObject prefab)
        {
            if (!_poolers.ContainsKey(prefab))
            {
                GameObject pooler = new GameObject();
                
                pooler.transform.SetParent(this.transform);
                
                pooler.name = prefab.name + "Pooler";
                
                Pooler poolerComponent = pooler.AddComponent<Pooler>();
                
                poolerComponent.Init(prefab);
                
                _poolers.Add(prefab,poolerComponent);

                return poolerComponent;
            }
            else
            {
                return _poolers[prefab];
            }
        }
    }
}