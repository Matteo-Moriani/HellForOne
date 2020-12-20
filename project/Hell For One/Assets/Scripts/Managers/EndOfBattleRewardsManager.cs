using System;
using System.Collections;
using ArenaSystem;
using HordeSystem;
using ReincarnationSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class EndOfBattleRewardsManager : MonoBehaviour
    {
        [SerializeField] private int battleEndImps;
        [SerializeField] private float ray;
        
        private EndOfBattleRewardsManager _instance;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this);
        }

        private void OnEnable()
        {
            ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        }

        private void OnDisable()
        {
            ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        }

        private void OnGlobalEndBattle(ArenaManager obj)
        {
            StartCoroutine(SpawnCoroutine());
        }

        private IEnumerator SpawnCoroutine()
        {
            for (int i = 0; i < battleEndImps; i++)
            {
                Vector2 random = Random.insideUnitCircle * ray;
                
                HordeManager.Instance.SpawnImp(ReincarnationManager.Instance.CurrentLeader.transform.position + new Vector3(random.x,0f,random.y),Quaternion.identity);

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}