using System;
using System.Collections;
using System.Collections.Generic;
using ManaSystem;
using UnityEngine;
using UnityEngine.UI;

public class ManaBarUi : MonoBehaviour
{
    private Image _leftManaBarIn;
    private Image _rightManaBarIn;

    private Image _currentBar;
    
    #region Unity methods

    void Awake()
    {
        _leftManaBarIn = transform.GetChild( 1 ).GetComponent<Image>();
        _rightManaBarIn = transform.GetChild( 3 ).GetComponent<Image>();

        _currentBar = _leftManaBarIn;
    }

    private void OnEnable()
    {
        ImpMana.OnManaPoolChanged += OnManaPoolChanged;
        ImpMana.OnSegmentCharged += OnSegmentCharged;
        ImpMana.OnSegmentSpent += OnSegmentSpent;
    }

    private void OnDisable()
    {
        ImpMana.OnManaPoolChanged -= OnManaPoolChanged;
        ImpMana.OnSegmentCharged -= OnSegmentCharged;
        ImpMana.OnSegmentSpent -= OnSegmentSpent;
    }

    private void Start()
    {
        _leftManaBarIn.fillAmount = 0f;
        _rightManaBarIn.fillAmount = 0f;
    }

    #endregion

    #region External events handlers

    private void OnSegmentCharged(int obj)
    {
        _currentBar = _rightManaBarIn;
    }

    private void OnSegmentSpent(int obj)
    {
        if(obj == 1) return;

        _currentBar = _leftManaBarIn;
    }
    
    private void OnManaPoolChanged(float currentManaPool)
    {
        _currentBar.fillAmount = Mathf.Clamp(currentManaPool / (ImpMana.SingleSegmentPool * (ImpMana.CurrentChargedSegments + 1)),0f,1f);
    }

    #endregion
}
