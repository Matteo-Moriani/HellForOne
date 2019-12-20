using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralInput : MonoBehaviour
{
    private Menu currentScreen;
    public Menu CurrentScreen { get => currentScreen; set => currentScreen = value; }
    public int fpsCounterInMenu = 0;
    public bool dpadPressedInMenu = false;
    public float dpadWaitTime = 0.2f;
    private bool dpadInUse = false;
    public bool DpadInUse { get => dpadInUse; set => dpadInUse = value; }
    public Controller controller;
    public bool canGiveInput;

}
