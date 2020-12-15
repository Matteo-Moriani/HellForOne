using System;
using System.Collections.Generic;
using UnityEngine;

namespace HordeSystem
{
    public class HordeManager : MonoBehaviour
    {
        [SerializeField] private GameObject impPrefab;
        
        private readonly List<HordeImp> _currentImps = new List<HordeImp>();

        private int _maxImps = 16;

        private static HordeManager _instance;

        public GameObject ImpPrefab
        {
            get => impPrefab;
            private set => impPrefab = value;
        }

        public static HordeManager Instance
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

        public void RegisterImp(HordeImp imp)
        {
            if(_currentImps.Contains(imp)) return;
            
            _currentImps.Add(imp);
            
            Debug.Log("Adding imp \n Imp in Horde: " + _currentImps.Count);
        }

        public void UnregisterImp(HordeImp imp)
        {
            if(!_currentImps.Contains(imp)) return;

            _currentImps.Remove(imp);
            
            Debug.Log("Removing imp \n Imp in Horde: " + _currentImps.Count);
        }

        public void SpawnImp(Vector3 position, Quaternion rotation)
        {
            if(_currentImps.Count == _maxImps) return;
            
            Instantiate(impPrefab, position, rotation);
        }
    }
}