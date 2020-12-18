using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using System;

public class ShakeOnHit : MonoBehaviour
{
    CombatSystem combatSystem;
    public static event Action<float , float, bool> OnHitReceivedCameraShakeRequest;

    private void Awake()
    {
        combatSystem = GetComponentInChildren<CombatSystem>();
    }

    private void OnEnable()
    {
        combatSystem.OnDamageHitReceived += OnDamageHitReceived;
    }

    private void OnDisable()
    {
        combatSystem.OnDamageHitReceived -= OnDamageHitReceived;
    }

    private void OnDamageHitReceived( Attack arg1 , CombatSystem arg2 , Vector3 arg3 )
    {
        OnHitReceivedCameraShakeRequest?.Invoke(arg1.GetData().ONDamageHitShakeDuration, arg1.GetData().ONDamageHitShakeIntensity, arg1.GetData().DoCameraShakeOnDamageHit);
    }
}
