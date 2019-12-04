using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    private int target = 52;
    private int noTarget = 1000;
    private int oldVsyncCount;
    private bool isPaused = false;

    public bool IsPaused { get => isPaused; set => isPaused = value; }

    void Awake()
    {
        oldVsyncCount = QualitySettings.vSyncCount;
    }

    void Update()
    {
        if ( IsPaused )
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = target;
        }
        else
        {
            if ( QualitySettings.vSyncCount != oldVsyncCount )
                QualitySettings.vSyncCount = oldVsyncCount;
            Application.targetFrameRate = noTarget;
        }
    }
}
