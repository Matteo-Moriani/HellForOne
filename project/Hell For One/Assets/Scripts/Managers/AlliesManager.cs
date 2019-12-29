using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlliesManager : MonoBehaviour
{
    // Remove ally from list when reincarnate player
    private List<GameObject> alliesList;

    private static AlliesManager _instance;
    private float maxHealth = 4f;

    public static AlliesManager Instance { get { return _instance; } }
    public List<GameObject> AlliesList { get => alliesList; private set => alliesList = value; }

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
        if(alliesList.Count == 0)
            BattleEventsManager.RaiseOnGameOver();
    }

    private void GameOver() {
        SceneManager.LoadScene("Game Over Screen");
    }

    /*
    public void AddAlly(GameObject ally) {
        if (!alliesList.Contains(ally)) { 
            alliesList.Add(ally);   
        }
        else {
            // TODO - Im not 100% sure about this
            // Need to test
            // Destroy(ally);

            // Need to find a way to manage this
        }
    }
    */
}
