using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackStripe : MonoBehaviour
{
    private void OnEnable() {
        BattleEventsManager.onBattlePreparation += BeginCutscene;
        BattleEventsManager.onBossBattleEnter += EndCutscene;
    }

    private void OnDisable() {
        BattleEventsManager.onBattlePreparation -= BeginCutscene;
        BattleEventsManager.onBossBattleEnter -= EndCutscene;
    }

    private void BeginCutscene() {
        GetComponent<Image>().enabled = true;
        GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>().enabled = false;
    }

    private void EndCutscene() {
        GetComponent<Image>().enabled = false;
        GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>().enabled = true;
    }
}
