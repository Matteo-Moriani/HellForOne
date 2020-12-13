using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.ObjectPooling;

public class HitEffectBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _hitEffect;
    private CombatSystem _combatSystem;

    private void Awake()
    {
        _combatSystem = GetComponentInChildren<CombatSystem>();
    }

    private void OnEnable()
    {
        _combatSystem.OnDamageHitReceived += OnDamageHitReceived;
    }

    private void OnDisable()
    {
        _combatSystem.OnDamageHitReceived -= OnDamageHitReceived;
    }

    private void OnDamageHitReceived(Attack a, CombatSystem c, Vector3 hitLocation)
    {
        GameObject effect = PoolersManager.Instance.TryGetPooler(_hitEffect).GetPooledObject(1f);
        effect.transform.position = hitLocation;
        effect.transform.SetParent(gameObject.transform);
        foreach(ParticleSystem p in effect.GetComponentsInChildren<ParticleSystem>())
        {
            p.Play();
        }
    }
}
