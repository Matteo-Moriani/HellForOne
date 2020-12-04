using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.ObjectPooling
{
    /// <summary>
    /// Class that manages generic object pooling.
    /// A pooler will expand when needed.
    /// </summary>
    public class Pooler : MonoBehaviour
    {
        private GameObject _prefab;

        private List<GameObject> _pooledObjects;

        public delegate void OnGetPooledObject(Pooler pooler,GameObject pooled);
        /// <summary>
        /// Event raise after getting an inactive pooled object 
        /// </summary>
        public event OnGetPooledObject onGetPooledObject;

        private void RaiseOnGetPooledObject(Pooler pooler,GameObject pooled)
        {
            onGetPooledObject?.Invoke(pooler,pooled);
        }
        
        /// <summary>
        /// Initialize this pooler
        /// </summary>
        /// <param name="prefab">The object to pool</param>
        public void Init(GameObject prefab)
        {
            _prefab = prefab;

            _pooledObjects = new List<GameObject>();
            
            GenerateObjects();
        }

        private void GenerateObjects()
        {
            for (int i = 0; i < PoolersManager.Instance.StartingPooledObjects; i++)
            {
                GameObject pooled = Instantiate(_prefab, this.transform);
                
                _pooledObjects.Add(pooled);
                
                pooled.SetActive(false);
            }
        }

        public List<GameObject> GetPooledObjects() => _pooledObjects;

        /// <summary>
        /// Returns an inactive object in this pooler.
        /// If there is none inactive objects, it expands the pooler
        /// </summary>
        /// <returns>The inactive object</returns>
        public GameObject GetPooledObject()
        {
            GameObject pooled = FindInactive();

            if (pooled == null)
            {
                GenerateObjects();

                pooled = FindInactive();
            }
            
            RaiseOnGetPooledObject(this,pooled);
                
            return pooled;
        }
        
        /// <summary>
        /// Get a pooled object and starts a coroutine that
        /// deactivate the pooled object after timeToLive seconds
        /// </summary>
        /// <param name="timeToLive">Liftetime of the object</param>
        /// <returns>The pooled object</returns>
        public GameObject GetPooledObject(float timeToLive)
        {
            GameObject pooled = FindInactive();

            if (pooled == null)
            {
                GenerateObjects();

                pooled = FindInactive();
            }
            
            RaiseOnGetPooledObject(this,pooled);
            
            StartCoroutine(PooledObjectLifetime(pooled, timeToLive));
            
            pooled.SetActive(true);
            
            return pooled;
        }

        public void DeactivatePooledObject(GameObject pooled)
        {
            if(!_pooledObjects.Contains(pooled))
                return;
            
            pooled.transform.position = Vector3.zero;
            pooled.transform.SetParent(transform);
            pooled.SetActive(false);
        }

        private GameObject FindInactive()
        {
            foreach (GameObject pooled in _pooledObjects)
            {
                if (!pooled.activeInHierarchy)
                    return pooled;
            }

            return null;
        }

        private IEnumerator PooledObjectLifetime(GameObject pooled,float duration)
        {
            yield return new WaitForSeconds(duration);
            
            DeactivatePooledObject(pooled);
        }
    }
}