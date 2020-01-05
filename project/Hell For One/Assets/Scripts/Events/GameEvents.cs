using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public delegate void OnStartGame();
    public static event OnStartGame onStartGame;

    public delegate void OnPause();
    public static event OnPause onPause;

    public delegate void OnResume();
    public static event OnResume onResume;

    public static void RaiseOnStartGame() { 
        if(onStartGame != null) { 
            onStartGame();    
        }    
    }

    public static void RaiseOnPause() {
        if (onPause != null) { 
            onPause();
        }    
    }

    public static void RaiseOnResume() { 
        if(onResume != null) { 
            onResume();    
        }    
    }
}
