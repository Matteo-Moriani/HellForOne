using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    #region Fields

    private string hammerGameObjectNameAndTag = "Hammer";
    
    private GameObject hammerManagerGameObject;
    private HammerManager hammerManager;

    #endregion
    
    #region Unity methods

    private void Awake()
    {
        AbilitiesManager abilitiesManager = GetComponent<AbilitiesManager>();

        hammerManagerGameObject = abilitiesManager.CreateAbility_GO(transform,hammerGameObjectNameAndTag);
        hammerManager = hammerManagerGameObject.AddComponent<HammerManager>();
    }

    #endregion
    
    #region Methods

    public void StartHammer()
    {
        
    }

    public void StopHammer()
    {
        
    }

    #endregion
}
