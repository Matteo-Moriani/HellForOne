using System;
using ReincarnationSystem;
using UnityEngine;
using ArenaSystem;

namespace CrownSystem
{
    public class GroundCrownBehaviour : MonoBehaviour
    {
        private void OnEnable()
        {
            ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        }

        private void OnDisable()
        {
            ArenaManager.OnGlobalEndBattle -= OnGlobalEndBattle;
        }

        private void OnTriggerEnter(Collider other)
        {
            ReincarnableBehaviour reincarnableBehaviour = other.GetComponent<ReincarnableBehaviour>();
            
            if(reincarnableBehaviour == null) return;
            
            if(ReincarnationManager.Instance.CurrentLeader != null && ReincarnationManager.Instance.CurrentLeader != reincarnableBehaviour) return;
            
            reincarnableBehaviour.GetComponentInChildren<CrownBehaviour>(true).gameObject.SetActive(true);
            
            gameObject.SetActive(false);
        }

        /// <summary>
        /// If the crown is on the ground then put it back on player's head
        /// </summary>
        public void OnGlobalEndBattle( ArenaManager arenaManager )
        {
            GameObject[] imps = GameObject.FindGameObjectsWithTag( "Demon" );
            foreach ( GameObject imp in imps )
            {
                if ( LayerMask.NameToLayer( "Player" ) == imp.layer )
                {
                    ReincarnableBehaviour reincarnableBehaviour = imp.GetComponent<ReincarnableBehaviour>();

                    reincarnableBehaviour.GetComponentInChildren<CrownBehaviour>( true ).gameObject.SetActive( true );

                    gameObject.SetActive( false );
                }
            }
        }
    }
}