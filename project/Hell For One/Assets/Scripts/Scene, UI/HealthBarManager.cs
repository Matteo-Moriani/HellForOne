using System.Collections;
using System.Collections.Generic;
using ArenaSystem;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public GameObject healthBarOutside;
    public GameObject healthBarInside;

    private void OnEnable()
    {
        ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
        ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
    }

    private void OnDisable() 
    {
        ArenaManager.OnGlobalStartBattle -= OnGlobalStartBattle;
        ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;
    }

    private void OnGlobalStartBattle(ArenaManager instance) 
    {
        //healthBar.SetActive(true);
        //healthBar.transform.GetChild( 0 ).gameObject.GetComponent<Image>().fillAmount = 1f;
        healthBarOutside.GetComponent<Image>().enabled = true;
        healthBarInside.GetComponent<Image>().enabled = true;
    }

    private void OnGlobalEndBattle(ArenaManager instance) 
    {
        //healthBar.SetActive(false);
        healthBarOutside.GetComponent<Image>().enabled = false;
        healthBarInside.GetComponent<Image>().enabled = false;
    }
}
