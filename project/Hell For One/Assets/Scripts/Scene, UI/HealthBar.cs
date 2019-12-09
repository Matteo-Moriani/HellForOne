using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Image healthBar;
    private float maxHealth;
    private Stats characterStats;

    // Start is called before the first frame update
    void Start()
    {
        characterStats = GameObject.FindGameObjectWithTag("Boss").GetComponent<Stats>();
        healthBar = gameObject.GetComponent<Image>();
        maxHealth = characterStats.health;
    }

    private void OnEnable()
    {
        BattleEventsManager.onBossBattleEnter += OnBossBattleEnter;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBossBattleEnter -= OnBossBattleEnter;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = characterStats.health / maxHealth;
    }

    private void OnBossBattleEnter() {
        characterStats = GameObject.FindGameObjectWithTag("Boss").GetComponent<Stats>();
        healthBar = gameObject.GetComponent<Image>();
        maxHealth = characterStats.health;
    }
}
