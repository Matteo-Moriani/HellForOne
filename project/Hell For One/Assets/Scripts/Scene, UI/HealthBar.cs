using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
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
        BattleEventsManager.onBossBattleEnter += OnBossBattleEnter;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBossBattleEnter -= OnBossBattleEnter;
    }

    // Update is called once per frame
    void Update()
    {
        HealthBarInside.fillAmount = characterStats.health / maxHealth;
    }

    private void OnBossBattleEnter() {
        characterStats = GameObject.FindGameObjectWithTag("Boss").GetComponent<Stats>();
        HealthBarInside = gameObject.GetComponent<Image>();
        maxHealth = characterStats.health;
    }
}
