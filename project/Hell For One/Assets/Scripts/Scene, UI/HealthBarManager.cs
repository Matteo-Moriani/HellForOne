using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public GameObject healthBarOutside;
    public GameObject healthBarInside;

    private void OnEnable()
    {
        BattleEventsManager.onBattleEnter += ActivateHealthBar;
        BattleEventsManager.onBattleExit += DeactivateHealthBar;
    }

    private void OnDisable() {
        BattleEventsManager.onBattleEnter -= ActivateHealthBar;
        BattleEventsManager.onBattleExit -= DeactivateHealthBar;
    }

    private void ActivateHealthBar() {
        //healthBar.SetActive(true);
        //healthBar.transform.GetChild( 0 ).gameObject.GetComponent<Image>().fillAmount = 1f;
        healthBarOutside.GetComponent<Image>().enabled = true;
        healthBarInside.GetComponent<Image>().enabled = true;
    }

    private void DeactivateHealthBar() {
        //healthBar.SetActive(false);
        healthBarOutside.GetComponent<Image>().enabled = false;
        healthBarInside.GetComponent<Image>().enabled = false;
    }
}
