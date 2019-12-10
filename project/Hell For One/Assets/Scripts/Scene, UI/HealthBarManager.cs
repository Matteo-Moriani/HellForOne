using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        //healthBar.SetActive(true);
        //healthBar.transform.GetChild( 0 ).gameObject.GetComponent<Image>().fillAmount = 1f;
        healthBar.GetComponent<Image>().enabled = true;
    }

    private void DeactivateHealthBar() {
        //healthBar.SetActive(false);
        healthBar.GetComponent<Image>().enabled = false;
    }
}
