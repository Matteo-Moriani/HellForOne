using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupAggro : MonoBehaviour
{
    private GroupBehaviour groupBehaviour;
    
    [SerializeField]
    private int groupAggro = 0;

    private void Start()
    {
        groupAggro = 0;
        groupBehaviour = GetComponent<GroupBehaviour>();
    }

    public void UpdateGruopAggro() {
        groupAggro = 0;
        
        if (groupBehaviour == null)
            groupBehaviour = GetComponent<GroupBehaviour>();

        if (groupBehaviour != null)
        {
            foreach (GameObject demon in groupBehaviour.demons)
            {
                Stats stats = demon.GetComponent<Stats>();

                if (stats != null)
                {
                    groupAggro += stats.Aggro;    
                }
                else
                {
                    Debug.Log(this.transform.root.gameObject.name + " GruopAggro.ResetGroupAggro cannot find stats in " + demon.name);
                }
            }
        }
        else
        {
            Debug.Log(this.transform.root.gameObject.name + " GruopAggro.ResetGroupAggro cannot find GroupBehaviour");
        }
    }

    public void ResetGroupAggro() { 
        groupAggro = 0;
        
        if(groupBehaviour == null)
            groupBehaviour = GetComponent<GroupBehaviour>();

        if (groupBehaviour != null) { 
            foreach(GameObject demon in groupBehaviour.demons) { 
                Stats stats = demon.GetComponent<Stats>();
                
                if(stats != null) { 
                    stats.Aggro = 0;   
                }
                else { 
                    Debug.Log(this.transform.root.gameObject.name + " GuopAggro.ResetGroupAggro cannot find stats in " + demon.name);
                }
            }    
        }
        else { 
            Debug.Log(this.transform.root.gameObject.name + " GuopAggro.ResetGroupAggro cannot find GroupBehaviour");    
        }
    }

    public void RaiseGroupAggro(int n) { 
        groupAggro += n;    
    }

    public void LowerGroupAggro(int n) { 
        groupAggro -= n;    
    }
}
