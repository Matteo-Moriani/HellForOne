using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject boss;
    [SerializeField]
    private GameObject midBoss;

    // TODO - andarle a prendere nei prefab
    [SerializeField]
    private GameObject bossPosition;
    [SerializeField]
    private GameObject midBossPosition;
    [SerializeField]
    private GameObject bossFightScriptedPosition;
    [SerializeField]
    private GameObject midBossFightScriptedPosition;
    [SerializeField]
    private GameObject bossArenaCenter;
    [SerializeField]
    private GameObject midBossArenaCenter;

    private bool isMidBossAlive;
    private bool isBossAlive;

    public int bossTotRegenDemons = 2;
    public int midBossTotRegenDemons = 1;

    public bool IsMidBossAlive { get => isMidBossAlive; set => isMidBossAlive = value; }
    public bool IsBossAlive { get => isBossAlive; set => isBossAlive = value; }

    private void OnEnable()
    {
        BattleEventsManager.onBossBattleExit += MidBossKilled;
        BattleEventsManager.onBattlePreparation += InstantiatePosition;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBossBattleExit -= MidBossKilled;
        BattleEventsManager.onBattlePreparation -= InstantiatePosition;
    }

    private void Start(){
        IsMidBossAlive = true;
        IsBossAlive = false;
    }

    private void MidBossKilled() {
        if (IsMidBossAlive) {
            IsMidBossAlive = false;
            IsBossAlive = true;
            DestroyMidBossArenaCenter();
            Destroy(midBossFightScriptedPosition);
            ActivateBossArenaCenter();
            ActivateBoss();
            BattleEventsManager.onBossBattleExit -= MidBossKilled;
        }
    }

    private void ActivateBossArenaCenter() { 
        bossArenaCenter.gameObject.SetActive(true);    
    }

    private void DestroyMidBossArenaCenter() { 
        GameObject.Destroy(midBossArenaCenter);    
    }

    private void InstantiatePosition() {
        GameObject bossPositions = new GameObject();
        if (IsMidBossAlive) {
            bossPositions = Instantiate(midBossPosition, midBoss.transform.position, Quaternion.identity);
            midBossFightScriptedPosition.SetActive(true);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScriptedMovements>().SetTargetPosition(midBossFightScriptedPosition.transform.position);
        }
        if(IsBossAlive){
            bossPositions = Instantiate(bossPosition, boss.transform.position, Quaternion.identity);
            bossFightScriptedPosition.SetActive(true);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScriptedMovements>().SetTargetPosition(bossFightScriptedPosition.transform.position);
        }
        bossPositions.SetActive(true);
    }

    private void BossKilled() { 
        IsBossAlive = false;    
    }

    private void ActivateBoss() { 
        boss.gameObject.SetActive(true);   
    }
}
