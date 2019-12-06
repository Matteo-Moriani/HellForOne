using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleEnemiesFightTrigger : MonoBehaviour
{
    private bool isAlreadyInLittleEnemiesFight = false;

    private void OnEnable()
    {
        BattleEventsManager.onBattleExit += OnBattleExit;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBattleExit -= OnBattleExit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!isAlreadyInLittleEnemiesFight)
            {
                isAlreadyInLittleEnemiesFight = true;

                BattleEventsManager.RaiseOnBattleEnter();
                Debug.Log("Player entered littleEnemies area");

                Destroy(this.gameObject);
            }
        }
    }

    private void OnBattleExit()
    {
        isAlreadyInLittleEnemiesFight = false;
    }
}