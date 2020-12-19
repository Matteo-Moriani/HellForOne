using System.Collections;
using System.Collections.Generic;
using ArenaSystem;
using HordeSystem;
using ReincarnationSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TacticsSystem
{
    public class RecruitManager : MonoBehaviour
    { 
        [SerializeField] private float randomRay;

        private readonly List<ImpRecruitBehaviour> _recruitingImps = new List<ImpRecruitBehaviour>();

        private static RecruitManager _instance;

        private Coroutine _recruitCr;
        
        public static RecruitManager Instance
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
        
        private void OnEnable()
        {
            ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        }

        private void OnDisable()
        {
            ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;
        }

        public void RegisterRecruiter(ImpRecruitBehaviour recruiter)
        {
            if (_recruitingImps.Contains(recruiter)) return;
            
            _recruitingImps.Add(recruiter);
        }

        public void UnregisterRecruiter(ImpRecruitBehaviour recruiter)
        {
            if(!_recruitingImps.Contains(recruiter)) return;

            _recruitingImps.Remove(recruiter);
        }

        private void OnGlobalStartBattle(ArenaManager arena) => StartCoroutine(RecruitCoroutine(arena));
        
        private void OnGlobalEndBattle(ArenaManager arena) => StopAllCoroutines();

        private IEnumerator RecruitCoroutine(ArenaManager currentArena)
        {
            float timer = 0f;

            while (true)
            {
                yield return null;
                
                if (_recruitingImps.Count == 0)
                {
                    timer = 0f;
                    continue;
                }

                timer += Time.deltaTime;

                if( timer <= -15 / 11 * _recruitingImps.Count + 25) continue;

                timer = 0f;

                HordeManager.Instance.SpawnImp(currentArena.NewImpSpawnAnchors[Random.Range(0,currentArena.NewImpSpawnAnchors.Length)].position,Quaternion.identity);
            }
        }
    }
}