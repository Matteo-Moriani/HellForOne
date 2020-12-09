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
    [SerializeField] private Image healthBarInside;
    [SerializeField] private Image healthBarOutside;
    [SerializeField] private TMPro.TextMeshProUGUI bossName;
    
    private float _bossStartHp;
    private HitPoints _bossHitPoints;

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
    }

    private void OnGlobalEndBattle(ArenaManager obj) => DeactivateHealthBar();

    private void OnGlobalStartBattle(ArenaManager obj)
    {
        ActivateHealthBar();

        _bossHitPoints = obj.Boss.GetComponent<HitPoints>();
        //maxHealth = _bossHitPoints;

        bossName.text = obj.Boss.gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        //HealthBarInside.fillAmount = characterStats.health / maxHealth;
    }
}
