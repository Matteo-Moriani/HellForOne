using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlliesManager : MonoBehaviour
{
    /// <summary>
    /// Instance of this class
    /// </summary>
    public static AlliesManager Instance { get { return _instance; } }
    
    /// <summary>
    /// A list of all the ally imps
    /// </summary>
    public List<GameObject> AlliesList { get => alliesList; private set => alliesList = value; }

    private Action OnNewImpSpawned;
    private Action OnImpDied;

    private List<GameObject> alliesList;

    private static AlliesManager _instance;
    
    private float maxHealth = 4f;

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

        alliesList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Demon"));
    }

    private void OnEnable()
    {
        BattleEventsManager.onBattleEnter += EnterBattle;
        BattleEventsManager.onBossBattleEnter += EnterBattle;
        BattleEventsManager.onBattleExit += ExitBattle;
        BattleEventsManager.onBossBattleExit += ExitBattle;
        BattleEventsManager.onGameOver += GameOver;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBattleEnter -= EnterBattle;
        BattleEventsManager.onBossBattleEnter -= EnterBattle;
        BattleEventsManager.onBattleExit -= ExitBattle;
        BattleEventsManager.onBossBattleExit -= ExitBattle;
        BattleEventsManager.onGameOver -= GameOver;
    }

    private void EnterBattle()
    {
        foreach(GameObject ally in alliesList) { 
            ally.GetComponent<Combat>().enabled = true;    
        }    
    }

    private void ExitBattle() { 
        foreach(GameObject ally in alliesList) { 
            ally.GetComponent<Combat>().enabled = false;
            ally.GetComponent<Stats>().health = maxHealth;
        }  
    }
    
    /// <summary>
    /// Register a method to OnNewImpSpawned event
    /// </summary>
    /// <param name="method">The method to register</param>
    public void RegisterOnNewImpSpawned(Action method) { 
        OnNewImpSpawned += method;    
    }

    /// <summary>
    /// Unregister a method to OnNewImpSpawned event
    /// </summary>
    /// <param name="method">The method to unregister</param>
    public void UnregisterOnNewImpSpawned(Action method) { 
        OnNewImpSpawned -= method;    
    }

    private void RaiseOnNewImpSpawned() { 
        if(OnNewImpSpawned != null) { 
            OnNewImpSpawned();    
        }    
    }

    /// <summary>
    /// Register a method to OnImpDied event
    /// </summary>
    /// <param name="method">The method to register</param>
    public void RegisterOnImpDied(Action method)
    {
        OnImpDied += method;
    }

    /// <summary>
    /// Unregister a method to OnImpDied event
    /// </summary>
    /// <param name="method">The method to unregister</param>
    public void UnregisterOnImpDied(Action method)
    {
        OnImpDied -= method;
    }

    private void RaiseOnImpDied()
    {
        if (OnImpDied != null)
        {
            OnImpDied();
        }
    }

    /// <summary>
    /// Spawn a new ally imp
    /// </summary>
    /// <param name="demonToSpawn">The imp GameObject to spawn</param>
    /// <param name="spawnPosition">Spawn position for imp to spawn</param>
    public void SpawnAlly(GameObject demonToSpawn, Vector3 spawnPosition) {
        GameObject spawnedDemon = Instantiate(demonToSpawn, spawnPosition, Quaternion.identity);
        alliesList.Add(spawnedDemon);

        // TODO - MANAGE ALLY COMPONENTS WHEN IN COMBAT AND IN OUT OF COMBAT
        if(BattleEventsHandler.IsInBossBattle || BattleEventsHandler.IsInRegularBattle) {
            spawnedDemon.GetComponent<Combat>().enabled = true;
        }
        else {
            spawnedDemon.GetComponent<Combat>().enabled = false;
        }

        RaiseOnNewImpSpawned();
    }

    public void ManagePlayerReincarnation(GameObject newPlayer) { 
        if(newPlayer != null) { 
            alliesList.Remove(newPlayer);
            if(alliesList.Count == 0)
                BattleEventsManager.RaiseOnGameOver();
        }           
    }

    public void AllyKilled(GameObject deadAlly) { 
        alliesList.Remove(deadAlly);

        RaiseOnImpDied();

        // TODO - Create a Game over manager
        if(alliesList.Count == 0)
            BattleEventsManager.RaiseOnGameOver();
    }

    private void GameOver() {
        SceneManager.LoadScene("Game Over Screen");
    }
}
