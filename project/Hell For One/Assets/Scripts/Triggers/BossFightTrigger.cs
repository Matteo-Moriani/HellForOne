using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightTrigger : MonoBehaviour
{
    private bool isAlreadyInBossFight = false;

    private void OnEnable()
    {
        BattleEventsManager.onBossBattleExit += onBossBattleExit;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBossBattleExit -= onBossBattleExit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!isAlreadyInBossFight)
            {
                isAlreadyInBossFight = true;

                BattleEventsManager.RaiseOnBossBattleEnter();
                Debug.Log("Player entered boss area");

                Destroy(this.gameObject);
            }
        }
    }

    private void onBossBattleExit() { 
        isAlreadyInBossFight = false;    
    }
}
