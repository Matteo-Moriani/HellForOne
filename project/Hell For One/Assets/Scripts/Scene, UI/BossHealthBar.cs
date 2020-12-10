using System;
using System.Collections;
using System.Collections.Generic;
using ArenaSystem;
using FactoryBasedCombatSystem;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    #region Fields

    [SerializeField] private Image healthBarInside;
    [SerializeField] private Image healthBarOutside;
    [SerializeField] private TMPro.TextMeshProUGUI bossName;
    
    private float _bossStartHp;
    private HitPoints _bossHitPoints;

    #endregion

    #region Unity Methods

    private void Start() => DeactivateHealthBar();

    private void OnEnable()
    {
        ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
        ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
    }

    private void OnDisable()
    {
        ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
        ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;
    }

    #endregion

    #region Methods

    private void DeactivateHealthBar()
    {
        healthBarInside.enabled = false;
        healthBarOutside.enabled = false;
        bossName.enabled = false;    
    }

    private void ActivateHealthBar()
    {
        healthBarInside.enabled = true;
        healthBarOutside.enabled = true;
        bossName.enabled = true;

        healthBarInside.fillAmount = 1f;
    }

    #endregion

    #region Event handlers

    private void OnGlobalStartBattle(ArenaManager obj)
    {
        ActivateHealthBar();

        bossName.text = obj.Boss.gameObject.name;
        
        _bossHitPoints = obj.Boss.GetComponent<HitPoints>();
        _bossHitPoints.OnHpChanged += OnHpChanged;
        _bossStartHp = _bossHitPoints.StartingHp;
    }

    private void OnGlobalEndBattle(ArenaManager obj)
    {
        DeactivateHealthBar();
        
        _bossHitPoints.OnHpChanged -= OnHpChanged;
        _bossHitPoints = null;
        _bossStartHp = 0f;
    }

    private void OnHpChanged(float obj) => healthBarInside.fillAmount = obj / _bossStartHp;

    #endregion
}
