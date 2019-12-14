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
    private GameObject minBossPosition;
    [SerializeField]
    private GameObject bossArenaCenter;
    [SerializeField]
    private GameObject midBossArenaCenter;

    private bool isMidBossAlive;
    private bool isBossAlive;

    private void OnEnable()
    {
        BattleEventsManager.onBossBattleExit += MidBossKilled;
        BattleEventsManager.onBossBattleEnter += InstantiateBossPosition;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBossBattleExit -= MidBossKilled;
        BattleEventsManager.onBossBattleEnter -= InstantiateBossPosition;
    }

    private void Start(){
        isMidBossAlive = true;
        isBossAlive = false;
    }

    private void MidBossKilled() {
        if (isMidBossAlive) {
            isMidBossAlive = false;
            isBossAlive = true;
            DestroyMidBossArenaCenter();
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

    private void InstantiateBossPosition() {
        GameObject positions = new GameObject();
        if (isMidBossAlive) {
            positions = Instantiate(minBossPosition, midBoss.transform.position, Quaternion.identity);
        }
        if(isBossAlive){
            positions = Instantiate(bossPosition, boss.transform.position, Quaternion.identity);
        }
        positions.SetActive(true);
    }

    private void BossKilled() { 
        isBossAlive = false;    
    }

    private void ActivateBoss() { 
        boss.gameObject.SetActive(true);   
    }
}
