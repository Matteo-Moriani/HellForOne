using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // [SerializeField]
    // private GameObject boss;
    // [SerializeField]
    // private GameObject midBoss;
    //
    // // TODO - andarle a prendere nei prefab
    // [SerializeField]
    // private GameObject bossPosition;
    // [SerializeField]
    // private GameObject midBossPosition;
    // [SerializeField]
    // private GameObject bossFightScriptedPosition;
    // [SerializeField]
    // private GameObject midBossFightScriptedPosition;
    // [SerializeField]
    // private GameObject bossArenaCenter;
    // [SerializeField]
    // private GameObject midBossArenaCenter;
    // [SerializeField]
    // private GameObject crownPrefab;
    //
    // private static bool isMidBossAlive;
    // private static bool isBossAlive;
    //
    // public static bool IsMidBossAlive { get => isMidBossAlive; private set => isMidBossAlive = value; }
    // public static bool IsBossAlive { get => isBossAlive; private set => isBossAlive = value; }
    //
    // private static LevelManager _instance;
    //
    // public static LevelManager Instance { get { return _instance; } }
    //
    // private void Awake()
    // {
    //     if(_instance != null && _instance != this)
    //     {
    //         Destroy(gameObject);
    //     }
    //     else
    //     {
    //         _instance = this;
    //     }
    // }

    // private void OnEnable()
    // {
    //     GameEventsManager.OnBattleExit += MidBossKilled;
    //     GameEventsManager.OnBattlePreparation += InstantiatePosition;
    // }
    //
    // private void OnDisable()
    // {
    //     GameEventsManager.OnBattleExit -= MidBossKilled;
    //     GameEventsManager.OnBattlePreparation -= InstantiatePosition;
    // }

    // private void Start(){
    //     IsMidBossAlive = true;
    //     IsBossAlive = false;
    // }
    //
    // private void MidBossKilled() {
    //     if (IsMidBossAlive) {
    //         IsMidBossAlive = false;
    //         IsBossAlive = true;
    //         Destroy(midBossArenaCenter);
    //         Destroy(midBossFightScriptedPosition);
    //         ActivateBossArenaCenter();
    //         ActivateBoss();
    //         GameEventsManager.OnBattleExit -= MidBossKilled;
    //     }
    // }
    //
    // private void ActivateBossArenaCenter() { 
    //     bossArenaCenter.gameObject.SetActive(true);    
    // }
    //
    // private void InstantiatePosition() {
    //     GameObject bossPositions = new GameObject();
    //     if (IsMidBossAlive) {
    //         bossPositions = Instantiate(midBossPosition, midBoss.transform.position, Quaternion.identity);
    //         midBossFightScriptedPosition.SetActive(true);
    //         GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScriptedMovements>().SetTargetPosition(midBossFightScriptedPosition.transform.position);
    //     }
    //     if(IsBossAlive){
    //         bossPositions = Instantiate(bossPosition, boss.transform.position, Quaternion.identity);
    //         bossFightScriptedPosition.SetActive(true);
    //         GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScriptedMovements>().SetTargetPosition(bossFightScriptedPosition.transform.position);
    //     }
    //     bossPositions.SetActive(true);
    // }
    //
    // private void BossKilled() { 
    //     IsBossAlive = false;    
    // }
    //
    // private void ActivateBoss() { 
    //     boss.gameObject.SetActive(true);   
    // }
    //
    // public GameObject GetCrown()
    // {
    //     return crownPrefab;
    // }
}
