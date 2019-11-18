using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupSupport : MonoBehaviour
{
    private GroupBehaviour groupBehaviour;
    
    // Field serialized only for testing
    // TODO - Remove SerializeField after testing
    [SerializeField]
    private int supportingUnits = 0;

    public int SupportingUnits { get => supportingUnits; private set => supportingUnits = value; }

    private void Start()
    {
        groupBehaviour = GetComponent<GroupBehaviour>();    
    }

    public void AddSupportingUnit() { 
        supportingUnits++;
        
        // If supporting units excedes maxNumDemons or DemonsInGroup...
        if(supportingUnits > groupBehaviour.maxNumDemons || supportingUnits > groupBehaviour.GetDemonsNumber()) { 
            // ...We undo the add
            supportingUnits--;
            Debug.Log("You were trying to set supporting units > maxNumDemons or > DemonsInGroup, check for error");
        }
    }

    public void RemoveSupportingUnit() { 
        supportingUnits--;
        
        if(supportingUnits < 0) { 
            supportingUnits = 0;
            Debug.Log("You were trying to set supporting units < 0, check for errors");
        }
    }

    public void UpdateSupportingUnits() { 
        supportingUnits = 0;

        foreach(GameObject demon in groupBehaviour.demons) { 
            if(demon != null) { 
                Stats stats = demon.GetComponent<Stats>();
                
                if(stats != null){
                    if (stats.IsSupporting) { 
                        AddSupportingUnit();    
                    }
                }
            }    
        }    
    }
}
