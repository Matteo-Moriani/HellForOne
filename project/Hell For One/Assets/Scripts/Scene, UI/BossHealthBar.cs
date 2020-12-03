using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    private Image healthBarInside;
    private Image healthBarOutside;
    private float maxHealth;
    private Stats characterStats;
    private TMPro.TextMeshProUGUI bossName;

    public Image HealthBarInside { get => healthBarInside; set => healthBarInside = value; }
    public Image HealthBarOutside { get => healthBarOutside; set => healthBarOutside = value; }

    // Start is called before the first frame update
    void Start()
    {
        characterStats = GameObject.FindGameObjectWithTag("Boss").GetComponent<Stats>();
        HealthBarInside = GameObject.Find( "BossBarIn" ).GetComponent<Image>();
        HealthBarOutside = GameObject.Find( "BossHealthBarOut" ).GetComponent<Image>();
        maxHealth = characterStats.health;
        bossName = GetComponentInChildren<TMPro.TextMeshProUGUI>();
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
        bossName.enabled = true;
        HealthBarInside.enabled = true;
        HealthBarOutside.enabled = true;
        characterStats = GameObject.FindGameObjectWithTag("Boss").GetComponent<Stats>();
        //healthBarInside = gameObject.GetComponent<Image>();
        maxHealth = characterStats.health;
    }
}
