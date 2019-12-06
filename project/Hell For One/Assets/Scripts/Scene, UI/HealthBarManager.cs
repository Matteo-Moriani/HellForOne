using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    public GameObject healthBar;

    private void OnEnable()
    {
        BattleEventsManager.onBossBattleEnter += ActivateHealthBar;
        BattleEventsManager.onBossBattleExit += DeactivateHealthBar;
    }

    private void OnDisable() {
        BattleEventsManager.onBossBattleEnter -= ActivateHealthBar;
        BattleEventsManager.onBossBattleExit -= DeactivateHealthBar;
    }

    private void ActivateHealthBar() {
        healthBar.SetActive(true);
    }

    private void DeactivateHealthBar() {
        healthBar.SetActive(false);
    }
}
