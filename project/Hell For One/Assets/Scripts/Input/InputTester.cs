using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTester : MonoBehaviour
{
    private InputManager inputManager;

    private void Start()
    {
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("RT: " + inputManager.R2Axis());
        //Debug.Log("LT: " + inputManager.L2Axis());
    }
}
