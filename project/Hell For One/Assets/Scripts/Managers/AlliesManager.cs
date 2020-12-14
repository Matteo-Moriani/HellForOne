using System.Collections;
using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO - Refactor part 2
public class AlliesManager : MonoBehaviour
{
    /// <summary>
    /// Instance of this class
    /// </summary>
    // public static AlliesManager Instance { get { return _instance; } }
    //
    // /// <summary>
    // /// A list of all the ally imps
    // /// </summary>
    // public List<GameObject> AlliesList { get => alliesList; private set => alliesList = value; }
    //
    // private int impMaxNumber = 16;
    // public int ImpMaxNumber { get => impMaxNumber; private set => impMaxNumber = value; }
    //
    // public delegate void OnNewImpSpawned(GameObject newImp);
    // public event OnNewImpSpawned onNewImpSpawned;
    //
    // private List<GameObject> alliesList;
    //
    // private static AlliesManager _instance;
    //
    // private float maxHealth = 4f;
    //
    // private void Awake()
    // {
    //     if (_instance != null && _instance != this)
    //     {
    //         Destroy(this.gameObject);
    //     }
    //     else
    //     {
    //         _instance = this;
    //     }
    //
    //     alliesList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Demon"));
    // }
    //
    // private void OnEnable()
    // {
    //     foreach (GameObject demon in alliesList)
    //     {
    //         if (demon != null)
    //         {
    //             Stats stats = demon.GetComponent<Stats>();
    //             Reincarnation reincarnation = demon.GetComponent<Reincarnation>();
    //
    //             stats.onDeath += OnDeath;
    //             reincarnation.onReincarnation += OnReincarnation;
    //         }
    //     }
    //     
    //     // GameEventsManager.OnBattleEnter += OnBattleEnter;
    //     // GameEventsManager.OnBattleExit += OnBattleExit;
    // }
    //
    // private void OnDisable()
    // {
    //     // GameEventsManager.OnBattleEnter -= OnBattleEnter;
    //     // GameEventsManager.OnBattleExit -= OnBattleExit;
    // }
    //
    // // private void OnBattleEnter()
    // // {
    // //     foreach(GameObject ally in alliesList) { 
    // //         ally.GetComponent<Combat>().enabled = true;    
    // //     }    
    // // }
    // //
    // // private void OnBattleExit() { 
    // //     foreach(GameObject ally in alliesList) { 
    // //         ally.GetComponent<Combat>().enabled = false;
    // //         ally.GetComponent<Stats>().health = maxHealth;
    // //     }  
    // // }
    //
    // private void RaiseOnNewImpSpawned(GameObject newImp) { 
    //     onNewImpSpawned?.Invoke(newImp);     
    // }
    //
    // /// <summary>
    // /// Spawn a new ally imp
    // /// </summary>
    // /// <param name="demonToSpawn">The imp GameObject to spawn</param>
    // /// <param name="spawnPosition">Spawn position for imp to spawn</param>
    // public void SpawnAlly(GameObject demonToSpawn, Vector3 spawnPosition) {
    //
    //     if(alliesList.Count >= impMaxNumber) return;
    //
    //     GameObject spawnedDemon = Instantiate(demonToSpawn, spawnPosition, Quaternion.identity);
    //     alliesList.Add(spawnedDemon);
    //
    //     /*
    //     // TODO - MANAGE ALLY COMPONENTS WHEN IN COMBAT AND IN OUT OF COMBAT
    //     if(BattleEventsHandler.IsInBossBattle || BattleEventsHandler.IsInRegularBattle) {
    //         spawnedDemon.GetComponent<Combat>().enabled = true;
    //     }
    //     else {
    //         spawnedDemon.GetComponent<Combat>().enabled = false;
    //     }
    //     */
    //
    //     spawnedDemon.GetComponent<Stats>().onDeath += OnDeath;
    //     spawnedDemon.GetComponent<Reincarnation>().onReincarnation += OnReincarnation;
    //     
    //     RaiseOnNewImpSpawned(spawnedDemon);
    // }
    //
    // public void ManagePlayerReincarnation(GameObject newPlayer) { 
    //               
    // }
    //
    // private void OnGameOver() {
    //     SceneManager.LoadScene("Game Over Screen");
    // }
    //
    // #region Event handlers
    //
    // private void OnDeath(Stats sender)
    // {
    //     alliesList.Remove(sender.gameObject);
    //
    //     sender.onDeath -= OnDeath;
    //     sender.gameObject.GetComponent<Reincarnation>().onReincarnation -= OnReincarnation;
    // }
    //
    // private void OnReincarnation(GameObject newPlayer)
    // {
    //     if(newPlayer != null) { 
    //         alliesList.Remove(newPlayer);
    //     }
    //
    //     newPlayer.GetComponent<Stats>().onDeath -= OnDeath;
    //     newPlayer.GetComponent<Reincarnation>().onReincarnation -= OnReincarnation;
    // }
    //
    // #endregion
}
