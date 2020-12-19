using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryBasedCombatSystem;
using FactoryBasedCombatSystem.ScriptableObjects.Attacks;
using System;

public class CameraShakeRequest : MonoBehaviour
{
    CombatSystem combatSystem;
    public static event Action<float , float, bool> SingleCameraShakeRequest;

    private void Awake()
    {
        combatSystem = GetComponentInChildren<CombatSystem>();
    }

    private void OnEnable()
    {
        combatSystem.OnDamageHitReceived += OnDamageHitReceived;
        combatSystem.OnStartAttack += OnStartAttack;
        combatSystem.OnStopAttack += OnStopAttack;
    }

    private void OnStopAttack(Attack attack)
    {
        attack.OnAttackActivated -= OnAttackActivated;
    }

    private void OnDisable()
    {
        combatSystem.OnDamageHitReceived -= OnDamageHitReceived;
        combatSystem.OnStartAttack -= OnStartAttack;
        combatSystem.OnStopAttack -= OnStopAttack;
    }

    private void OnStartAttack( Attack attack )
    {
        SingleCameraShakeRequest?.Invoke( 0 ,
            1 , attack.GetData().DoCameraShakeOnDamageHit );
        attack.OnAttackActivated += OnAttackActivated;
    }

    private void OnAttackActivated( Attack attack )
    {
        SingleCameraShakeRequest?.Invoke( attack.GetData().ONDamageHitShakeDuration , 
            attack.GetData().ONDamageHitShakeIntensity , attack.GetData().DoCameraShakeOnDamageHit );
    }

    private void OnDamageHitReceived( Attack arg1 , CombatSystem arg2 , Vector3 arg3 )
    {
        SingleCameraShakeRequest?.Invoke(arg1.GetData().ONDamageHitShakeDuration, arg1.GetData().ONDamageHitShakeIntensity, arg1.GetData().DoCameraShakeOnDamageHit);
    }
}
