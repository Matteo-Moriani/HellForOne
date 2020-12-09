using Managers;
using UnityEngine;

namespace Triggers
{
    public class BossFightTrigger : MonoBehaviour
    {
        // private bool isAlreadyInBattle = false;
        //
        // private void OnEnable()
        // {
        //     GameEventsManager.OnBattleExit += OnBattleExit;
        // }
        //
        // private void OnDisable()
        // {
        //     GameEventsManager.OnBattleExit -= OnBattleExit;
        // }
        //
        // private void OnTriggerEnter( Collider other )
        // {
        //     if ( other.tag == "Player" )
        //     {
        //         if ( !isAlreadyInBattle )
        //         {
        //             isAlreadyInBattle = true;
        //         
        //             GameEventsManager.RaiseOnBattlePreparation();
        //             //Debug.Log( "Player entered boss area" );
        //
        //             Destroy(gameObject);
        //         }
        //     }
        // }
        //
        // private void OnBattleExit()
        // {
        //     isAlreadyInBattle = false;
        // }
    }
}
