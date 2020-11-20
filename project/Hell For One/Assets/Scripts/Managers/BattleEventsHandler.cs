using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventsHandler : MonoBehaviour
{
    private GameObject spawner;

    private static bool isInBattle = false;

    private static BattleEventsHandler _instance;
    public static BattleEventsHandler Instance { get { return _instance; } }

    public static bool IsInBattle { get => isInBattle; private set => isInBattle = value; }

    private void Awake()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner");

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        // TODO - Testing this to remove mob battle
        // find better solution
        //BattleEventsManager.RaiseOnBattleExit();
    }

    private void OnEnable()
    {
        BattleEventsManager.onBattleEnter += OnBattleEnter;
        
        BattleEventsManager.onBattleExit += OnBattleExit;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBattleEnter -= OnBattleEnter;
        
        BattleEventsManager.onBattleExit -= OnBattleExit;
    }

    private void Update()
    {
        if (IsInBattle)
        {
            if (EnemiesManager.Instance.Boss == null)
            {
                BattleEventsManager.RaiseOnBattleExit();
            }
        }
    }

    private void OnBattleEnter()
    {   
        if (!isInBattle)
        {
            IsInBattle = true;
            /*
            // TODO - Manage this in alliesManager
            if (spawner != null)
            {
                AllyDemonSpawnerTest adst = spawner.GetComponent<AllyDemonSpawnerTest>();

                if (adst != null)
                {
                    adst.enabled = true;
                }
            }
            /*
            GameObject boss = GameObject.FindGameObjectWithTag("Boss");

            if(boss != null) { 
                BossBehavior bossBehavior = boss.GetComponent<BossBehavior>();
                if(bossBehavior != null) { 
                    bossBehavior.enabled = true;    
                }
            }
            */
        }
    }

    private void OnBattleExit()
    {   /*
        if (isInBossBattle)
        {
            IsInBossBattle = false;
            if (spawner != null)
            {
                StopCoroutine(spawner.GetComponent<AllyDemonSpawnerTest>().SpawnAllyCR);
                spawner.GetComponent<AllyDemonSpawnerTest>().enabled = false;
            }
        }
        */
        if (isInBattle) {
            isInBattle = false;
        }
    }

}
