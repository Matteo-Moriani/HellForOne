using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public enum Controller { 
        Xbox,
        Ps3,
        MouseAndKeyboard,
        None
    }

    [SerializeField]
    [Tooltip("Type of the controller that player will use")]
    private Controller type = Controller.None;

    private bool ltWasPressedLastFrame = false;
    private bool rtWasPressedLastFrame = false;

    public Controller Type { get => type; set => type = value; }

    #region XorA

    /// <summary>
    /// Returns true when x (Ps3), a (Xbox) is pressed
    /// </summary>
    public bool XButtonDown() {
        switch (Type) { 
            case Controller.Xbox:
                return Input.GetKeyDown(KeyCode.JoystickButton0);
            case Controller.Ps3:
                return Input.GetButtonDown( "cross" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true when x (Ps3), a (Xbox) is relased
    /// </summary>
    public bool XButtonUp() {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyUp(KeyCode.JoystickButton0);
            case Controller.Ps3:
                return Input.GetButtonUp( "cross" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;    
    }

    /// <summary>
    /// Returns true while x (Ps3), a (Xbox) is pressed
    /// </summary>
    public bool XButtonHeldDown() {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKey(KeyCode.JoystickButton0);
            case Controller.Ps3:
                return Input.GetButton( "cross" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    #endregion

    #region CircleOrB

    /// <summary>
    /// Returns true when cirlce (Ps3), b (Xbox) is pressed
    /// </summary>
    public bool CircleButtonDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyDown(KeyCode.JoystickButton1);
            case Controller.Ps3:
                return Input.GetButtonDown( "circle" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true when circle (Ps3), b (Xbox) is relased
    /// </summary>
    public bool CircleButtonUp()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyUp(KeyCode.JoystickButton1);
            case Controller.Ps3:
                return Input.GetButtonUp( "circle" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true while circle (Ps3), b (Xbox) is pressed
    /// </summary>
    public bool CircleButtonHeldDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKey(KeyCode.JoystickButton1);
            case Controller.Ps3:
                return Input.GetButton( "circle" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    #endregion

    #region TriangleOrY

    /// <summary>
    /// Returns true when triangle (Ps3), y (Xbox) is pressed
    /// </summary>
    public bool TriangleButtonDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyDown(KeyCode.JoystickButton3);
            case Controller.Ps3:
                return Input.GetButtonDown( "triangle" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true when triangle (Ps3), y (Xbox) is relased
    /// </summary>
    public bool TriangleButtonUp()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyUp(KeyCode.JoystickButton3);
            case Controller.Ps3:
                return Input.GetButtonUp( "triangle" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true while triangle (Ps3), y (Xbox) is pressed
    /// </summary>
    public bool TriangleButtonHeldDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKey(KeyCode.JoystickButton3);
            case Controller.Ps3:
                return Input.GetButton( "triangle" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    #endregion

    #region SquareOrX

    /// <summary>
    /// Returns true when square (Ps3), x (Xbox) is pressed
    /// </summary>
    public bool SquareButtonDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyDown(KeyCode.JoystickButton2);
            case Controller.Ps3:
                return Input.GetButtonDown( "square" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true when square (Ps3), x (Xbox) is relased
    /// </summary>
    public bool SquareButtonUp()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyUp(KeyCode.JoystickButton2);
            case Controller.Ps3:
                return Input.GetButtonUp( "square" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true while square (Ps3), x (Xbox) is pressed
    /// </summary>
    public bool SquareButtonHeldDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKey(KeyCode.JoystickButton2);
            case Controller.Ps3:
                return Input.GetButton( "square" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    #endregion

    // TODO - To implement PS3 Input
    #region LeftStick

    /// <summary>
    /// Returns the value of the horizontal axis of the left stick
    /// </summary>
    public float LeftStickHorizontal() {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetAxis("Horizontal");
            case Controller.Ps3:
                // TODO - To Implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return 0f;
    }
    
    /// <summary>
    /// Returns the value of the vertical axis of the left stick
    /// </summary>
    public float LeftStickVertical() {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetAxis("Vertical");
            case Controller.Ps3:
                // TODO - To implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return 0f;
    }

    /// <summary>
    /// Returns true when left stick is pressed
    /// </summary>
    public bool LeftStickButtonDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyDown(KeyCode.JoystickButton8);
            case Controller.Ps3:
                // TODO - To implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true when left stick is relased
    /// </summary>
    public bool LeftStickButtonUp()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyUp(KeyCode.JoystickButton8);
            case Controller.Ps3:
                // TODO - To implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true while left stick is pressed
    /// </summary>
    public bool LeftStickButtonHeldDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKey(KeyCode.JoystickButton8);
            case Controller.Ps3:
                // TODO - To implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    #endregion

    // TODO - To implement PS3 Input
    #region RightStick

    /// <summary>
    /// Returns the value of the horizontal axis of the right stick
    /// </summary>
    public float RightStickHorizontal()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetAxis("XBoxRightStickHorizontal");
            case Controller.Ps3:
                // TODO - To Implement
                break;
            case Controller.MouseAndKeyboard:
                // TODO - To Implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return 0f;
    }

    /// <summary>
    /// Returns the value of the vertical axis of the right stick
    /// </summary>
    public float RightStickVertical()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetAxis("XBoxRightStickVertical");
            case Controller.Ps3:
                // TODO - To implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return 0f;
    }

    /// <summary>
    /// Returns true when right stick is pressed
    /// </summary>
    public bool RightStickButtonDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyDown(KeyCode.JoystickButton9);
            case Controller.Ps3:
                // TODO - To implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true when right stick is relased
    /// </summary>
    public bool RightStickButtonUp()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyUp(KeyCode.JoystickButton9);
            case Controller.Ps3:
                // TODO - To implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true while right stick is pressed
    /// </summary>
    public bool RightStickButtonHeldDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKey(KeyCode.JoystickButton9);
            case Controller.Ps3:
                // TODO - To implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    #endregion

    // TODO - To implement PS3 Input
    #region Dpad

    /// <summary>
    /// Returns the value of the horizontal axis of the Dpad
    /// </summary>
    public float DpadHorizontal()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetAxis("XBoxDpadHorizontal");
            case Controller.Ps3:
                // TODO - To Implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return 0f;
    }

    /// <summary>
    /// Returns the value of the vertical axis of the Dpad
    /// </summary>
    public float DpadVertical()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetAxis("XBoxDpadVertical");
            case Controller.Ps3:
                // TODO - To implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return 0f;
    }

    #endregion

    // TODO - To implement PS3 Input
    #region L1orLB

    /// <summary>
    /// Returns true when L1 (Ps3), LB (Xbox) is pressed
    /// </summary>
    public bool L1ButtonDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyDown(KeyCode.JoystickButton4);
            case Controller.Ps3:
                return Input.GetButtonDown( "L1" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true when L1 (Ps3), LB (Xbox) is relased
    /// </summary>
    public bool L1ButtonUp()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyUp(KeyCode.JoystickButton4);
            case Controller.Ps3:
                return Input.GetButtonUp( "L1" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true while L1 (Ps3), LB (Xbox) is pressed
    /// </summary>
    public bool L1ButtonHeldDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKey(KeyCode.JoystickButton4);
            case Controller.Ps3:
                return Input.GetButton( "L1" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    #endregion

    // TODO - To implement PS3 Input
    #region R1orRB

    /// <summary>
    /// Returns true when R1 (Ps3), RB (Xbox) is pressed
    /// </summary>
    public bool R1ButtonDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyDown(KeyCode.JoystickButton5);
            case Controller.Ps3:
                // TODO - To implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true when R1 (Ps3), RB (Xbox) is relased
    /// </summary>
    public bool R1ButtonUp()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKeyUp(KeyCode.JoystickButton5);
            case Controller.Ps3:
                // TODO - To implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true while R1 (Ps3), RB (Xbox) is pressed
    /// </summary>
    public bool R1ButtonHeldDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                return Input.GetKey(KeyCode.JoystickButton5);
            case Controller.Ps3:
                // TODO - To implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    #endregion

    // TODO - To implement PS3 Input
    #region L2

    /// <summary>
    /// Returns the value of the L2 axis
    /// </summary>
    /*
    public bool L2Axis()
    {
        switch (type)
        {
            case Controller.Xbox:
                if ( Input.GetAxis( "XBoxLT" ) == 1f )
                    return true;
                else
                    return false;
            case Controller.Ps3:
                return Input.GetButtonDown( "L2" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }
    */

    /// <summary>
    /// Returns true when L2 (Ps3), LT (Xbox) is pressed
    /// </summary>
    public bool L2ButtonDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                if( Input.GetAxis("XBoxLT") == 1f && !ltWasPressedLastFrame) { 
                    ltWasPressedLastFrame = true;
                    return true;
                }
                break;
            case Controller.Ps3:
                return Input.GetButtonDown("L2");
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true when L2 (Ps3), LT (Xbox) is relased
    /// </summary>
    public bool L2ButtonUp()
    {
        switch (Type)
        {
            case Controller.Xbox:
                if (Input.GetAxis("XBoxLT") == 0f && ltWasPressedLastFrame)
                {
                    ltWasPressedLastFrame = false;
                    return true;
                }
                break;
            case Controller.Ps3:
                return Input.GetButtonUp("L2");
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true while L2 (Ps3), LT (Xbox) is pressed
    /// </summary>
    public bool L2ButtonHeldDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                if (Input.GetAxis("XBoxLT") == 1f && ltWasPressedLastFrame)
                {
                    return true;
                }
                break;
            case Controller.Ps3:
                return Input.GetButton("L2");
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    #endregion

    // TODO - To implement PS3 Input
    #region R2

    /// <summary>
    /// Returns the value of the R2 axis
    /// </summary>
    /*
    public bool R2Axis()
    {
        switch (type)
        {
            case Controller.Xbox:
                if ( Input.GetAxis( "XBoxRT" ) == 1f)
                    return true;
                else
                    return false;
            case Controller.Ps3:
                return Input.GetButtonDown( "R2" );
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }
    */

    /// <summary>
    /// Returns true when R2 (Ps3), RT (Xbox) is pressed
    /// </summary>
    public bool R2ButtonDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                if (Input.GetAxis("XBoxRT") == 1f && !rtWasPressedLastFrame)
                {
                    rtWasPressedLastFrame = true;
                    return true;
                }
                break;
            case Controller.Ps3:
                return Input.GetButtonDown("R2");
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true when R2 (Ps3), RT (Xbox) is relased
    /// </summary>
    public bool R2ButtonUp()
    {
        switch (Type)
        {
            case Controller.Xbox:
                if (Input.GetAxis("XBoxRT") == 0f && rtWasPressedLastFrame)
                {
                    rtWasPressedLastFrame = false;
                    return true;
                }
                break;
            case Controller.Ps3:
                return Input.GetButtonUp("R2");
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true while R2 (Ps3), RT (Xbox) is pressed
    /// </summary>
    public bool R2ButtonHeldDown()
    {
        switch (Type)
        {
            case Controller.Xbox:
                if (Input.GetAxis("XBoxRT") == 1f && rtWasPressedLastFrame)
                {
                    return true;
                }
                break;
            case Controller.Ps3:
                return Input.GetButton("R2");
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return false;
    }

    #endregion
}
