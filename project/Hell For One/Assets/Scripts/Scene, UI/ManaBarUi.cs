﻿using ManaSystem;
using UnityEngine;
using UnityEngine.UI;

public class ManaBarUi : MonoBehaviour
{
    [SerializeField]
    private Color _fullSegmentColor;
    private Color _defaultColor;

    private Image _leftManaBarIn;
    private Image _rightManaBarIn;

    private Image _currentBar;
    
    #region Unity methods

    void Awake()
    {
        _leftManaBarIn = transform.GetChild( 1 ).GetComponent<Image>();
        _rightManaBarIn = transform.GetChild( 3 ).GetComponent<Image>();

        _currentBar = _leftManaBarIn;
        _defaultColor = _leftManaBarIn.color;
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

    private void OnSegmentCharged(int segmentsCharged)
    {
        if(segmentsCharged == 0)
            return;
        else if(segmentsCharged == 1)
        {
            _currentBar = _rightManaBarIn;
            _leftManaBarIn.color = _fullSegmentColor;
            _rightManaBarIn.color = _defaultColor;
        }
        else
        {
            _currentBar = null;
            _leftManaBarIn.color = _fullSegmentColor;
            _rightManaBarIn.color = _fullSegmentColor;
        }
    }

    private void OnSegmentSpent(int segmentsSpent)
    {
        if(segmentsSpent == 0)
            return;
        else if(segmentsSpent == 1)
        {
            if(_currentBar == null)
            {
                _rightManaBarIn.color = _defaultColor;
                _currentBar = _rightManaBarIn;
            }
            else if(_currentBar == _rightManaBarIn)
            {
                _leftManaBarIn.color = _defaultColor;
                _currentBar = _leftManaBarIn;
            }
        }
        else
        {
            _leftManaBarIn.color = _defaultColor;
            _rightManaBarIn.color = _defaultColor;
            _currentBar = _leftManaBarIn;
        }
    }
    
    private void OnManaPoolChanged(float currentManaPool)
    {
        if(currentManaPool >= ImpMana.SingleSegmentPool)
        {
            _leftManaBarIn.fillAmount = 1f;
            _rightManaBarIn.fillAmount = (currentManaPool - ImpMana.SingleSegmentPool) / ImpMana.SingleSegmentPool;
        }
        else
        {
            _rightManaBarIn.fillAmount = 0f;
            _leftManaBarIn.fillAmount = currentManaPool / ImpMana.SingleSegmentPool;
        }

        //_currentBar.fillAmount = Mathf.Clamp(currentManaPool / (ImpMana.SingleSegmentPool * (ImpMana.CurrentChargedSegments + 1)), 0f, 1f);
    }

    #endregion
}
