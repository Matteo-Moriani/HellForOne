﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventsManager : MonoBehaviour
{
    public delegate void OnBattlePreparation();
    public static event OnBattlePreparation onBattlePreparation;

    public delegate void OnGameOver();
    public static event OnGameOver onGameOver;

    public delegate void OnBattleEnter();
    public static event OnBattleEnter onBattleEnter;

    public delegate void OnBattleExit();
    public static event OnBattleExit onBattleExit;

    private static BattleEventsManager _instance;

    public static BattleEventsManager Instance { get { return _instance;} }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public static void RaiseOnBattlePreparation() {
        if(onBattlePreparation != null) {
            onBattlePreparation();
        }
    }

    public static void RaiseOnBattleEnter() { 
        if(onBattleEnter != null) { 
            onBattleEnter();    
        }    
    }

    public static void RaiseOnBattleExit() { 
        if(onBattleExit != null) { 
            onBattleExit();
        }    
    }

    public static void RaiseOnGameOver() {
        if(onGameOver != null) {
            onGameOver();
        }
    }
}
