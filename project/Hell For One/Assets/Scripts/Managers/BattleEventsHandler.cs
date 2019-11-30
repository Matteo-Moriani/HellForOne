using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventsHandler : MonoBehaviour
{
    private bool isInBossBattle = false;
    private bool isInRegularBattle = false;

    private static BattleEventsHandler _instance;
    public static BattleEventsHandler Instance { get { return _instance;} }

    public bool IsInBossBattle { get => isInBossBattle; private set => isInBossBattle = value; }
    public bool IsInRegularBattle { get => isInRegularBattle; private set => isInRegularBattle = value; }

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

    // Start is called before the first frame update
    void Start()
    {
        // This is here now for testing, need to implment some logic, 
        // maybe triggers to enter in battle or boss battle state.
        BattleEventsManager.RaiseOnBossBattleEnter();
    }

    private void Update()
    {
        if (IsInRegularBattle) { 
            if(EnemiesManager.Instance.LittleEnemiesList.Count == 0) { 
                BattleEventsManager.RaiseOnBattleExit();    
            }    
        }

        if (IsInBossBattle) { 
            if(EnemiesManager.Instance.LittleEnemiesList.Count == 0 && EnemiesManager.Instance.Boss == null) { 
                BattleEventsManager.RaiseOnBossBattleExit();    
            }   
        }
    }

    private void EnterRegularBattle() { 
        IsInRegularBattle = true;   
    }

    private void EnterBossBattle(){
        IsInBossBattle = true;
    }

    private void ExitRegularBattle() { 
        IsInRegularBattle = false;    
    }

    private void ExitBossBattle() { 
        IsInBossBattle = false;    
    }
    
}
