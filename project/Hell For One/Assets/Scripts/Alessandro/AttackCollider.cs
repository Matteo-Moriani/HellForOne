using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField]
    private Stats stats;

    [SerializeField]
    private CombatManager combatManager;

    private void OnTriggerEnter(Collider other)
    {
        GameObject targetRootGo = other.transform.root.gameObject;

        if (targetRootGo.tag != "Player" && targetRootGo.tag != "Ally") { 
            if (other.tag == "BlockCollider") { 
                combatManager.StopAttack();        
            }
            if(other.tag == "IdleCollider") { 
                Stats targetRootStats = targetRootGo.GetComponent<Stats>();
                
                if(targetRootStats != null) { 
                    targetRootStats.TakeHit(stats.Damage);  
                }
                else { 
                    Debug.Log("Target does not have Stats attached");    
                }
            }
        }
    }
}
