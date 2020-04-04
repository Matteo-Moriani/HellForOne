using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    private Image manaBarIn;

    #region Unity methods

    void Awake()
    {
        manaBarIn = transform.GetChild( 0 ).GetComponent<Image>();
    }
    
    private void OnEnable()
    {
        ImpMana.onManaPoolChanged += OnManaPoolChanged;
    }

    private void OnDisable()
    {
        ImpMana.onManaPoolChanged -= OnManaPoolChanged;
    }

    void Start()
    {
        manaBarIn.fillAmount = ImpMana.ManaPool / ImpMana.MaxMana;
    }
    
    #endregion
    
    #region External events handlers

    private void OnManaPoolChanged()
    {
        manaBarIn.fillAmount = ImpMana.ManaPool / ImpMana.MaxMana;
    }

    #endregion
}
