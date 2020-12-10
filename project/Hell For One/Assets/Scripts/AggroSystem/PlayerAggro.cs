using System;
using UnityEngine;

namespace AggroSystem
{
    public class PlayerAggro : MonoBehaviour
    {
        private IPlayerAggroSubject[] _subjects;
        
        private static float _currentAggro;

        private void Awake() => _subjects = GetComponentsInChildren<IPlayerAggroSubject>();

        private void OnEnable()
        {
            foreach (IPlayerAggroSubject playerAggroSubject in _subjects)
            {
                playerAggroSubject.OnAggroActionDone += OnAggroActionDone;
            }
        }
        
        private void OnDisable()
        {
            foreach (IPlayerAggroSubject playerAggroSubject in _subjects)
            {
                playerAggroSubject.OnAggroActionDone -= OnAggroActionDone;
            }
        }

        private void OnAggroActionDone(float aggroValue) => _currentAggro = aggroValue >= _currentAggro ? aggroValue : _currentAggro;

        public static float ReadAggro()
        {
            float temp = _currentAggro;
            _currentAggro = 0f;

            return temp;
        }
    }
}
