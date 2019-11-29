using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private InputManager inputManager;

    private Dash dash;
    private Combat combat;
    private TacticsManager tacticsManager;

    private void Start()
    {
        dash = GetComponent<Dash>();
        combat = GetComponent<Combat>();
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
        tacticsManager = GetComponent<TacticsManager>();
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

            // Cross (PS3) / A (XBOX)
            if ( inputManager.XButtonDown() )
            {
                if ( combat != null && tacticsManager)
                {
                    tacticsManager.AssignOrder();
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

            // L2 (PS3) / LT (XBOX) - Down
            //if ( inputManager.L2Axis() )
            if(inputManager.L2ButtonDown())
            {
                if ( combat != null &&  tacticsManager)
                {
                    tacticsManager.RotateGroups(); ;
                }
            }

            // Need in order to set an internal bool in input manager
            // Im looking for a better solution
            if (inputManager.L2ButtonUp()) { }

            // R2 (PS3) / RT (XBOX) - Down
            //if ( inputManager.R2Axis() )
            if(inputManager.R2ButtonDown())
            {
                if ( combat != null && tacticsManager)
                {
                    tacticsManager.RotateTactics();
                }
            }

            // Need in order to set an internal bool in input manager
            // Im looking for a better solution
            if (inputManager.R2ButtonUp()) { }
        }
        else { 
            Debug.Log( name + " PlayerInput cannot find InputManager");    
        }
    }
}
