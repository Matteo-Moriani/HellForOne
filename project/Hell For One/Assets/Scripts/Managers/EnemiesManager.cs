﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    private List<GameObject> littleEnemiesList;

    private GameObject boss;

    private static EnemiesManager _instance;

    public static EnemiesManager Instance { get { return _instance; } }
    public List<GameObject> LittleEnemiesList { get => littleEnemiesList; private set => littleEnemiesList = value; }
    public GameObject Boss { get => boss; private set => boss = value; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnEnable()
    {
        BattleEventsManager.onBattleEnter += FindLittleEnemies;
        
        BattleEventsManager.onBossBattleEnter += FindLittleEnemies;
        BattleEventsManager.onBossBattleEnter += FindBoss;
    }
    
    private void OnDisable()
    {
        BattleEventsManager.onBattleEnter -= FindLittleEnemies;

        BattleEventsManager.onBossBattleEnter -= FindLittleEnemies;
        BattleEventsManager.onBossBattleEnter -= FindBoss;
    }
    
    private void FindLittleEnemies() { 
        LittleEnemiesList = new List<GameObject>(GameObject.FindGameObjectsWithTag("LittleEnemy"));   
    }

    private void FindBoss() { 
        Boss = GameObject.FindGameObjectWithTag("Boss");   
    }

    public void LittleEnemyKilled(GameObject littleEnemy) { 
        LittleEnemiesList.Remove(littleEnemy);       
    }

    public void BossKilled() { 
        Boss = null;    
    }
}