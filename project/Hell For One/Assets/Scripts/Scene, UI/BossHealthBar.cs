using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    private Image healthBarInside;
    private float maxHealth;
    private Stats characterStats;

    public Image HealthBarInside { get => healthBarInside; set => healthBarInside = value; }

    // Start is called before the first frame update
    void Start()
    {
        characterStats = GameObject.FindGameObjectWithTag("Boss").GetComponent<Stats>();
        HealthBarInside = gameObject.GetComponent<Image>();
        maxHealth = characterStats.health;
    }

    private void OnEnable()
    {
        BattleEventsManager.onBattleEnter += OnBattleEnter;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBattleEnter -= OnBattleEnter;
    }

    // Update is called once per frame
    void Update()
    {
        HealthBarInside.fillAmount = characterStats.health / maxHealth;
    }

    private void OnBattleEnter() {
        characterStats = GameObject.FindGameObjectWithTag("Boss").GetComponent<Stats>();
        healthBarInside = gameObject.GetComponent<Image>();
        maxHealth = characterStats.health;
    }
}
