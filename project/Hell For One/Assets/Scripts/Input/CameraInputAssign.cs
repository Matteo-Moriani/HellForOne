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

    private void Start()
    {
        cinemachineFreeLook = GetComponent<CinemachineFreeLook>();

        AxisState defaultXAxisState = cinemachineFreeLook.m_XAxis;
        AxisState defaultYaxisState = cinemachineFreeLook.m_YAxis;

        switch (InputManager.Instance.Type) { 
            case InputManager.Controller.Ps3:
                defaultXAxisState.m_InputAxisName = ps4XAxisName;
                defaultYaxisState.m_InputAxisName = ps4YAxisName;
                break;
            case InputManager.Controller.Xbox:
                defaultXAxisState.m_InputAxisName = xboxXAxisName;
                defaultYaxisState.m_InputAxisName = xboxYAxisName;
                break;
            case InputManager.Controller.MouseAndKeyboard:
                break;
            case InputManager.Controller.None:;
                break;
        }

        cinemachineFreeLook.m_XAxis = defaultXAxisState;
        cinemachineFreeLook.m_YAxis = defaultYaxisState;
    }
}
