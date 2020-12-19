using System;
using System.Collections;
using HordeSystem;
using UnityEngine;

namespace CallToArmsSystem
{
    public class CallToArmsManager : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float impSpawnRateo;
        [SerializeField, Range(0f,1f)] private float hordeRefillPercentage;
        
        private static CallToArmsManager _instance;

        private Coroutine _impSpawnCoroutine = null;
        
        public static CallToArmsManager Instance
        {
            get => _instance;
            private set => _instance = value;
        }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this);
        }

        public bool TrySpawnImps(Vector3 position)
        {
            if (_impSpawnCoroutine != null) return false;

            _impSpawnCoroutine = StartCoroutine(SpawnImpsCoroutine(position));

            return true;
        }

        private void StopSpawn()
        {   
            if(_impSpawnCoroutine == null) return;
            
            StopCoroutine(_impSpawnCoroutine);

            _impSpawnCoroutine = null;
        }

        private IEnumerator SpawnImpsCoroutine(Vector3 spawnPosition)
        {
            int toSpawn = (int) (HordeManager.Instance.AvailableSlots() * hordeRefillPercentage);

            for (int i = 0; i < toSpawn; i++)
            {
                HordeManager.Instance.SpawnImp(spawnPosition,Quaternion.identity);

                yield return new WaitForSeconds(impSpawnRateo);
            }
            
            StopSpawn();
        }
    }
}