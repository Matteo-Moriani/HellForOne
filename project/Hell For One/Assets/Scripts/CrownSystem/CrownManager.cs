using System;
using ReincarnationSystem;
using UnityEngine;
using Utils.ObjectPooling;

namespace CrownSystem
{
    public class CrownManager : MonoBehaviour
    {
        [SerializeField] private GameObject groundCrownPrefab;

        private GameObject _crownInstance;
        
        private void Start()
        {
            _crownInstance = Instantiate(groundCrownPrefab);

            groundCrownPrefab.SetActive(false);
            groundCrownPrefab.transform.position = new Vector3(0f, 0f, 0f);
        }

        private void OnEnable()
        {
            ReincarnationManager.OnLeaderDeath += OnLeaderDeath;
        }

        private void OnDisable()
        {
            ReincarnationManager.OnLeaderDeath += OnLeaderDeath;
        }

        private void OnLeaderDeath(ReincarnableBehaviour deadLeader)
        {
            _crownInstance.transform.position = deadLeader.transform.position + new Vector3(0f, 2f, 0f);
            _crownInstance.SetActive(true);
        }
    }
}
