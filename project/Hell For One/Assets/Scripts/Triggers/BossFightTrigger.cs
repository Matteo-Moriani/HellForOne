using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightTrigger : MonoBehaviour
{
    private bool isAlreadyInBattle = false;

    private void OnEnable()
    {
        BattleEventsManager.onBattleExit += OnBattleExit;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBattleExit -= OnBattleExit;
    }

    private void OnTriggerEnter( Collider other )
    {
        if ( other.tag == "Player" )
        {
            if ( !isAlreadyInBattle )
            {
                isAlreadyInBattle = true;
                
                BattleEventsManager.RaiseOnBattlePreparation();
                //Debug.Log( "Player entered boss area" );

                Destroy(gameObject);
            }
        }
    }

    private void OnBattleExit()
    {
        isAlreadyInBattle = false;
    }
}
