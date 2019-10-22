using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    private Stats stats;

    [SerializeField]
    private CombatManager combatManager;

    private void Start()
    {
        stats = this.transform.root.gameObject.GetComponent<Stats>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject targetRootGo = other.transform.root.gameObject;
        GameObject attackerRootGo = this.transform.root.gameObject;
        Stats targetRootStats = targetRootGo.GetComponent<Stats>();

        if (attackerRootGo.tag == "Player") { 
            if (targetRootGo.tag != "Player" && targetRootGo.tag != "Ally") { 
                if (other.tag == "BlockCollider") { 
                    combatManager.StopAttack();        
                }
                if(other.tag == "IdleCollider") { 
                    if(targetRootStats != null) { 
                        targetRootStats.TakeHit(stats.Damage);  
                    }
                    else { 
                        Debug.Log("Target does not have Stats attached");    
                    }
                    combatManager.StopAttack();
                }
            }
        }
        if(attackerRootGo.tag == "Ally") { 
            if(targetRootGo.tag != "Player" && targetRootGo.tag != "Ally") { 
                if(other.tag == "BlockCollider") { 
                    if(targetRootStats != null) {
                        if (Random.Range(1, 101) <= stats.AttackChance - targetRootStats.BlockChanceBonus) { 
                            targetRootStats.TakeHit(stats.Damage);   
                        }

                        combatManager.StopAttack();
                    }
                }
                if(other.tag == "IdleCollider") { 
                    if(targetRootStats != null) { 
                        if(Random.Range(1,101) <= stats.AttackChance) { 
                            targetRootStats.TakeHit(stats.Damage);    
                        }    
                    }
                    combatManager.StopAttack();
                }
            }   
        }
        if(attackerRootGo.tag == "Enemy") { 
            if(targetRootGo.tag != "Enemy") { 
                if(other.tag == "BlockCollider") { 
                    if(targetRootStats != null) { 
                        if(Random.Range(1,101) <= stats.AttackChance - targetRootStats.BlockChanceBonus) { 
                            targetRootStats.TakeHit(stats.Damage);    
                        }
                        combatManager.StopAttack();
                    }    
                }
                if(other.tag == "IdleCollider") { 
                    if(targetRootStats != null) { 
                        if(Random.Range(1,101) <= stats.AttackChance) { 
                            targetRootStats.TakeHit(stats.Damage);    
                        }
                        combatManager.StopAttack();
                    }    
                }
            }    
        }
    }
}
