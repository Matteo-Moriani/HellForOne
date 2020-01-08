using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraInputAssign : MonoBehaviour
{
    // TODO - support MouseAndKeyboard and None cases.

    CinemachineFreeLook cinemachineFreeLook;

    [SerializeField]
    private string ps4XAxisName;
    [SerializeField]
    private string ps4YAxisName;
    [SerializeField]
    private string xboxXAxisName;
    [SerializeField]
    private string xboxYAxisName;

    private AxisState defaultAxisState_X;
    private AxisState defaultAxisState_Y;
    
    private void Start()
    {
        cinemachineFreeLook = GetComponent<CinemachineFreeLook>();

        defaultAxisState_X = cinemachineFreeLook.m_XAxis;
        defaultAxisState_Y = cinemachineFreeLook.m_YAxis;

        switch (InputManager.Instance.Type) { 
            case InputManager.Controller.Ps3:
                defaultAxisState_X.m_InputAxisName = ps4XAxisName;
                defaultAxisState_Y.m_InputAxisName = ps4YAxisName;
                break;
            case InputManager.Controller.Xbox:
                defaultAxisState_X.m_InputAxisName = xboxXAxisName;
                defaultAxisState_Y.m_InputAxisName = xboxYAxisName;

                defaultAxisState_X.m_InvertInput = false;
                break;
            case InputManager.Controller.MouseAndKeyboard:
                break;
            case InputManager.Controller.None:;
                break;
        }

        cinemachineFreeLook.m_XAxis = defaultAxisState_X;
        cinemachineFreeLook.m_YAxis = defaultAxisState_Y;
    }

    public void InvertAxis_X() { 
        defaultAxisState_X.m_InvertInput = !defaultAxisState_X.m_InvertInput;
    }

    public void InvertAxis_Y() { 
        defaultAxisState_Y.m_InvertInput = !defaultAxisState_Y.m_InvertInput;
    }
}
