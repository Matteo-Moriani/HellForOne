using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private InputManager inputManager;

    private Dash dash;
    private Combat combat;

    private void Start()
    {
        dash = GetComponent<Dash>();
        combat = GetComponent<Combat>();
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
    }

    private void Update()
    {
        if(inputManager != null) {
            
            // Circle (PS3) / B (XBOX) 
            if (inputManager.CircleButtonDown()) { 
                if(dash != null) {
                    dash.TryDash(inputManager.LeftStickVertical(), inputManager.LeftStickHorizontal());
                }
            }

            // Square (PS3) / X (XBOX)
            if (inputManager.SquareButtonDown()) { 
                if(combat != null) {
                    combat.Attack();
                }
            }

            // L1 (PS3) / LB (XBOX) - Down
            if (inputManager.L1ButtonDown())
            {
                if(combat != null) { 
                    combat.StartBlock();    
                }
            }

            // L1 (PS3) / LB (XOBX) - Up
            if (inputManager.L1ButtonUp()) { 
                if(combat != null) { 
                    combat.StopBlock();    
                }    
            }

            // Triangle (PS3) / Y (XBOX)
            if (inputManager.TriangleButtonDown()) { 
                if(combat != null) { 
                    combat.RangedAttack(null);    
                }    
            }
        }
        else { 
            Debug.Log( name + " PlayerInput cannot find InputManager");    
        }
    }
}
