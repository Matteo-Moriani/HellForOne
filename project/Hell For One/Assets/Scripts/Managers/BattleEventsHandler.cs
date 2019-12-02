using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventsHandler : MonoBehaviour
{
    private GameObject spawner;

    private bool isInBossBattle = false;
    private bool isInRegularBattle = false;

    private static BattleEventsHandler _instance;
    public static BattleEventsHandler Instance { get { return _instance;} }

    public bool IsInBossBattle { get => isInBossBattle; private set => isInBossBattle = value; }
    public bool IsInRegularBattle { get => isInRegularBattle; private set => isInRegularBattle = value; }

    private void Awake()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner");

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
        BattleEventsManager.onBattleEnter += EnterRegularBattle;
        BattleEventsManager.onBossBattleEnter += EnterBossBattle;

        BattleEventsManager.onBattleExit += ExitRegularBattle;
        BattleEventsManager.onBossBattleExit += ExitBossBattle;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBattleEnter -= EnterRegularBattle;
        BattleEventsManager.onBossBattleEnter -= EnterBossBattle;

        BattleEventsManager.onBattleExit -= ExitRegularBattle;
        BattleEventsManager.onBossBattleExit -= ExitBossBattle;
    }

    void Start()
    {
        // This is here now for testing, need to implment some logic, 
        // maybe triggers to enter in battle or boss battle state.
        BattleEventsManager.RaiseOnBossBattleEnter();
    }

    private void Update()
    {   
        // TODO - Need to test this update logic
        // Should use triggers to activate battles!
        if (IsInRegularBattle)
        {
            if (EnemiesManager.Instance.LittleEnemiesList.Count == 0)
            {
                BattleEventsManager.RaiseOnBattleExit();
            }
        }


        if (IsInBossBattle)
        {
            if (EnemiesManager.Instance.Boss == null)
            {
                BattleEventsManager.RaiseOnBossBattleExit();
            }
        }

        /*
        if (!isInRegularBattle) { 
            if(EnemiesManager.Instance.LittleEnemiesList.Count > 0) { 
                BattleEventsManager.RaiseOnBattleEnter();    
            }    
        }

        if (!IsInBossBattle) { 
            if(EnemiesManager.Instance.Boss != null) { 
                BattleEventsManager.RaiseOnBossBattleEnter();    
            }    
        }
        */
    }

    private void EnterRegularBattle() { 
        IsInRegularBattle = true;
        spawner.GetComponent<AllyDemonSpawnerTest>().enabled = true;
    }

    private void EnterBossBattle(){
        IsInBossBattle = true;
        spawner.GetComponent<AllyDemonSpawnerTest>().enabled = true;
    }

    private void ExitRegularBattle() { 
        IsInRegularBattle = false;
        spawner.GetComponent<AllyDemonSpawnerTest>().enabled = false;
    }

    private void ExitBossBattle() { 
        IsInBossBattle = false;
        spawner.GetComponent<AllyDemonSpawnerTest>().enabled = false;
    }
    
}
