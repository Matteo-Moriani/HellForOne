using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCollisions : MonoBehaviour
{
    public enum Mode { Attack, Block, Idle}
    
    private Mode mode = Mode.Idle;
    
    public void SetMode(Mode mode) { 
        this.mode = mode;    
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (mode == Mode.Attack) { 
            Debug.Log("You Hit " + other.name);    
        }
        if(mode == Mode.Block) { 
            Debug.Log("You block" + other.name); 
        }
    }
}
