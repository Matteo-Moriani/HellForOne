using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private enum Controller { 
        Xbox,
        Ps3,
        None
    }

    [SerializeField]
    [Tooltip("Type of the controller that player will use")]
    private Controller type = Controller.None;

    // TODO - To implement PS3 Input
    #region XorA

    /// <summary>
    /// Returns true when x (Ps3), a (Xbox) is pressed
    /// </summary>
    public bool XButtonDown() {
        switch (type) { 
            case Controller.Xbox:
                return Input.GetKeyDown(KeyCode.JoystickButton0);
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
    /// Returns true when x (Ps3), a (Xbox) is relased
    /// </summary>
    public bool XButtonUp() {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKeyUp(KeyCode.JoystickButton0);
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
    /// Returns true while x (Ps3), a (Xbox) is pressed
    /// </summary>
    public bool XButtonHeldDown() {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKey(KeyCode.JoystickButton0);
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
    #region CircleOrB

    /// <summary>
    /// Returns true when cirlce (Ps3), b (Xbox) is pressed
    /// </summary>
    public bool CircleButtonDown()
    {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKeyDown(KeyCode.JoystickButton1);
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
    /// Returns true when circle (Ps3), b (Xbox) is relased
    /// </summary>
    public bool CircleButtonUp()
    {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKeyUp(KeyCode.JoystickButton1);
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
    /// Returns true while circle (Ps3), b (Xbox) is pressed
    /// </summary>
    public bool CircleButtonHeldDown()
    {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKey(KeyCode.JoystickButton1);
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
    #region TriangleOrY

    /// <summary>
    /// Returns true when triangle (Ps3), y (Xbox) is pressed
    /// </summary>
    public bool TriangleButtonDown()
    {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKeyDown(KeyCode.JoystickButton3);
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
    /// Returns true when triangle (Ps3), y (Xbox) is relased
    /// </summary>
    public bool TriangleButtonUp()
    {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKeyUp(KeyCode.JoystickButton3);
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
    /// Returns true while triangle (Ps3), y (Xbox) is pressed
    /// </summary>
    public bool TriangleButtonHeldDown()
    {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKey(KeyCode.JoystickButton3);
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
    #region SquareOrX

    /// <summary>
    /// Returns true when square (Ps3), x (Xbox) is pressed
    /// </summary>
    public bool SquareButtonDown()
    {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKeyDown(KeyCode.JoystickButton2);
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
    /// Returns true when square (Ps3), x (Xbox) is relased
    /// </summary>
    public bool SquareButtonUp()
    {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKeyUp(KeyCode.JoystickButton2);
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
    /// Returns true while square (Ps3), x (Xbox) is pressed
    /// </summary>
    public bool SquareButtonHeldDown()
    {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKey(KeyCode.JoystickButton2);
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
    #region LeftStick

    /// <summary>
    /// Returns the value of the horizontal axis of the left stick
    /// </summary>
    public float LeftStickHorizontal() {
        switch (type)
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
        switch (type)
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
        switch (type)
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
        switch (type)
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
        switch (type)
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
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetAxis("XBoxRightStickHorizontal");
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
    /// Returns the value of the vertical axis of the right stick
    /// </summary>
    public float RightStickVertical()
    {
        switch (type)
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
        switch (type)
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
        switch (type)
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
        switch (type)
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
        switch (type)
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
        switch (type)
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
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKeyDown(KeyCode.JoystickButton4);
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
    /// Returns true when L1 (Ps3), LB (Xbox) is relased
    /// </summary>
    public bool L1ButtonUp()
    {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKeyUp(KeyCode.JoystickButton4);
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
    /// Returns true while L1 (Ps3), LB (Xbox) is pressed
    /// </summary>
    public bool L1ButtonHeldDown()
    {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetKey(KeyCode.JoystickButton4);
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
    #region R1orRB

    /// <summary>
    /// Returns true when R1 (Ps3), RB (Xbox) is pressed
    /// </summary>
    public bool R1ButtonDown()
    {
        switch (type)
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
        switch (type)
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
        switch (type)
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
    public float L2Axis()
    {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetAxis("XBoxLT");
            case Controller.Ps3:
                // TODO - To Implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return 0f;
    }

    #endregion

    // TODO - To implement PS3 Input
    #region R2

    /// <summary>
    /// Returns the value of the R2 axis
    /// </summary>
    public float R2Axis()
    {
        switch (type)
        {
            case Controller.Xbox:
                return Input.GetAxis("XBoxRT");
            case Controller.Ps3:
                // TODO - To Implement
                break;
            case Controller.None:
                Debug.Log("Controller.type not set");
                break;
        }
        return 0f;
    }

    #endregion
}
