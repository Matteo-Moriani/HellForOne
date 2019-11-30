using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlliesManager : MonoBehaviour
{
    // Remove ally from list when reincarnate player
    private List<GameObject> alliesList;

    private static AlliesManager _instance;

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
    }

    private void Start()
    {
        alliesList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Demon"));    
    }

    private void OnEnable()
    {
        BattleEventsManager.onBattleEnter += EnterBattle;

        BattleEventsManager.onBossBattleEnter += EnterBattle;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBattleEnter -= EnterBattle;

        BattleEventsManager.onBossBattleEnter -= EnterBattle;
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
        }  
    }

    public void SpawnAlly(GameObject demonToSpawn, Vector3 spawnPosition) {
        GameObject spawnedDemon = Instantiate(demonToSpawn, spawnPosition, Quaternion.identity);
        alliesList.Add(spawnedDemon);

        // TODO - MANAGE ALLY COMPONENTS WHEN IN COMBAT AND IN OUT OF COMBAT
        if(BattleEventsHandler.Instance.IsInBossBattle || BattleEventsHandler.Instance.IsInRegularBattle) {
            spawnedDemon.GetComponent<Combat>().enabled = true;
        }
        else {
            spawnedDemon.GetComponent<Combat>().enabled = false;
        }
    }

    public void ManagePlayerReincarnation(GameObject newPlayer) { 
        if(newPlayer != null) { 
            alliesList.Remove(newPlayer);    
        }           
    }

    public void AllyKilled(GameObject deadAlly) { 
        alliesList.Remove(deadAlly);
    }
}
