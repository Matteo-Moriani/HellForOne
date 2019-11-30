using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventsManager : MonoBehaviour
{
    public delegate void OnBattleEnter();
    public static event OnBattleEnter onBattleEnter;

    public delegate void OnBattleExit();
    public static event OnBattleExit onBattleExit;

    public delegate void OnBossBattleEnter();
    public static event OnBossBattleEnter onBossBattleEnter;

    public delegate void OnBossBattleExit();
    public static event OnBossBattleExit onBossBattleExit;

    // TODO - Insert Some Boolean? maybe in BattleEventsHandler

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

    public static void RaiseOnBossBattleEnter() { 
        if(onBossBattleEnter != null) { 
            onBossBattleEnter();    
        }    
    }

    public static void RaiseOnBossBattleExit() { 
        if(onBossBattleExit != null) { 
            onBossBattleExit();
            Debug.Log("ExitBattle");
        }    
    }
}
