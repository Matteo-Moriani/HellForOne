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
            
            Debug.Log("Imp register");
        }

        public void UnregisterRecruiter(ImpRecruitBehaviour recruiter)
        {
            if(!_recruitingImps.Contains(recruiter)) return;

            _recruitingImps.Remove(recruiter);
            
            Debug.Log("Imp unregister");
        }

        private void OnGlobalStartBattle(ArenaManager obj) => StartCoroutine(RecruitCoroutine());
        
        private void OnGlobalEndBattle(ArenaManager obj) => StopAllCoroutines();

        private IEnumerator RecruitCoroutine()
        {
            float timer = 0f;

            while (true)
            {
                yield return null;

                Debug.Log("Recruiting Imps" + _recruitingImps.Count);
                
                if (_recruitingImps.Count == 0)
                {
                    timer = 0f;
                    continue;
                }

                timer += Time.deltaTime;
                
                Debug.Log("Timer : " + timer);
                Debug.Log("Time for new Imp: " + (-15 / 11 * _recruitingImps.Count + 25));
                
                if( timer <= -15 / 11 * _recruitingImps.Count + 25) continue;

                timer = 0f;
                
                Vector2 random = Random.insideUnitCircle * randomRay;
                Vector3 leaderPosition = ReincarnationManager.Instance.CurrentLeader.transform.position;
                
                HordeManager.Instance.SpawnImp(new Vector3(leaderPosition.x + random.x, leaderPosition.y,leaderPosition.z + random.y),Quaternion.identity);
            }
        }
    }
}