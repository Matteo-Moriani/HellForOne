using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    private Image leftManaBarIn;
    private Image rightManaBarIn;

    #region Unity methods

    void Awake()
    {
        leftManaBarIn = transform.GetChild( 1 ).GetComponent<Image>();
        rightManaBarIn = transform.GetChild( 3 ).GetComponent<Image>();
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
        leftManaBarIn.fillAmount = ImpMana.ManaPool / ImpMana.MaxMana / 2;
        rightManaBarIn.fillAmount = 0 / ImpMana.MaxMana;
    }

    #endregion

    #region External events handlers

    private void OnManaPoolChanged()
    {
        leftManaBarIn.fillAmount = ImpMana.ManaPool / (ImpMana.MaxMana / 2);

        if ( ImpMana.ManaPool > ImpMana.MaxMana / 2 )
        {
            rightManaBarIn.fillAmount = (ImpMana.ManaPool - (ImpMana.MaxMana / 2)) / (ImpMana.MaxMana / 2);
        }
        else
        {
            rightManaBarIn.fillAmount = 0;
        }
    }

    #endregion
}
