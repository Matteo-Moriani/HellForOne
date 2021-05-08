using System;
using System.Collections;
using FactoryBasedCombatSystem.Interfaces;
using UnityEngine;

namespace FactoryBasedCombatSystem
{
    public class UnitDestroyer : MonoBehaviour, IHitPointsObserver
    {
        [SerializeField] private float destroyAfterSeconds = 2f;

        public static event Action<Transform> OnPreImpDestroyed;
        
        public void OnZeroHp() => StartCoroutine(DestroyAfter());

        private IEnumerator DestroyAfter()
        {
            yield return new WaitForSeconds(destroyAfterSeconds);
            
            OnPreImpDestroyed?.Invoke(transform);
            
            Destroy(gameObject);
        }
    }
}