using System.Collections;
using System.Collections.Generic;
using ArenaSystem;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class BlackStripe : MonoBehaviour
{
    private void OnEnable() 
    {
        ArenaManager.OnGlobalSetupBattle += OnGlobalSetupBattle;
        ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
    }

    private void OnDisable() 
    {
        ArenaManager.OnGlobalSetupBattle += OnGlobalSetupBattle;
        ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
    }

    private void OnGlobalSetupBattle(ArenaManager arenaManager) {
        GetComponent<Image>().enabled = true;
        //GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>().enabled = false;
        //GameObject.FindGameObjectWithTag( "UIOrders" ).SetActive( false );
    }

    private void OnGlobalStartBattle(ArenaManager arenaManager) {
        GetComponent<Image>().enabled = false;
        //GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>().enabled = true;
        //GameObject.FindGameObjectWithTag( "UIOrders" ).SetActive( true );
    }
}
