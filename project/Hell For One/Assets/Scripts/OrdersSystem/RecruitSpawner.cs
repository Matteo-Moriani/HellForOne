using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RecruitSpawner : MonoBehaviour
{
    private float arenaRay;
    private float midBossArenaRay = 8f;
    private float meleeBossArenaRay = 13f;

    private Coroutine spawnerCr;

    [SerializeField]
    private GameObject impPrefab;

    [SerializeField]
    private int impRewardAfterBossBattle = 4;

    private GameObject arenaCenter;

    private Vector3 SpawnPosition()
    {
        //circumference with a little adjustment of 2 in both coordinates to the center.
        Vector3 spawnPosition = new Vector3( 0, 0, 0 );
        spawnPosition.x = Random.Range(0f, arenaRay);
        spawnPosition.z = Mathf.Sqrt(Mathf.Pow(arenaRay, 2f) - Mathf.Pow(spawnPosition.x, 2f));     // circumference with the center in the origin: x^2 + y^2 = r^2
        if(Random.Range(0f, 1f) > 0.5f)
            spawnPosition.x = spawnPosition.x * -1;
        if(Random.Range(0f, 1f) > 0.5f)
            spawnPosition.z = spawnPosition.z * -1;

        //Debug.Log("ally spawned in " + spawnPosition.x + " , " + spawnPosition.z + ". Must be between (-" + arenaRay + ", " + arenaRay + ") in both coordinates.");
        spawnPosition += transform.position;
        return spawnPosition;
    }

    private void OnEnable()
    {
        BattleEventsManager.onBattleEnter += OnBattleEnter;
        BattleEventsManager.onBattleExit += OnBattleExit;
    }

    private void OnNewImpSpawned(GameObject newImp)
    {
        RegisterToEvents(newImp);
    }

    private void OnDisable()
    {
        BattleEventsManager.onBattleEnter -= OnBattleEnter;
        BattleEventsManager.onBattleExit -= OnBattleExit;
        AlliesManager.Instance.onNewImpSpawned -= OnNewImpSpawned;
    }

    private void Start()
    {
        // TODO: I don't know why but RecruitSpawner.OnEnable is called before AlliesManager.Awake, so
        //    if we need to register to AlliedManager events we need to do it in Start
        AlliesManager.Instance.onNewImpSpawned += OnNewImpSpawned;
        
        foreach (GameObject imp in AlliesManager.Instance.AlliesList)
        {
            RegisterToEvents(imp);            
        }
    }

    private void RegisterToEvents(GameObject imp)
    {
        imp.GetComponent<Recruit>().onStartRecruit += OnStartRecruit;
        imp.GetComponent<Recruit>().onStopRecruit += OnStopRecruit;
        imp.GetComponent<Stats>().onDeath += OnDeath;
        imp.GetComponent<Reincarnation>().onReincarnation += OnReincarnation;
    }

    private void OnDeath(Stats sender)
    {
        UnregisterToEvents(sender.gameObject);
    }

    private void UnregisterToEvents(GameObject imp)
    {
        imp.GetComponent<Recruit>().onStartRecruit -= OnStartRecruit;
        imp.GetComponent<Recruit>().onStopRecruit -= OnStopRecruit;
        imp.GetComponent<Stats>().onDeath -= OnDeath;
        imp.GetComponent<Reincarnation>().onReincarnation -= OnReincarnation;
    }

    private void OnStopRecruit(Recruit sender)
    {
        // TODO - decouple from Recruit
        //        maybe an event to raise when there are no units recruiting
        if (Recruit.RecruitingUnits == 0)
        {
            if (spawnerCr != null)
            {
                StopCoroutine(spawnerCr);
                spawnerCr = null;
            }
        }
    }

    private void OnStartRecruit(Recruit sender)
    {
        if (spawnerCr == null)
            spawnerCr = StartCoroutine(SpawnerCoroutine());
    }

    private IEnumerator SpawnerCoroutine()
    {
        while (true)
        {
            // TODO - Decouple This
            AlliesManager.Instance.SpawnAlly(impPrefab,SpawnPosition());
            //yield return new WaitForSeconds(2.0f);
            yield return new WaitForSeconds( -15 / 11 * Recruit.RecruitingUnits + 25);
        }
    }

    private void OnReincarnation(GameObject newplayer)
    {
        UnregisterToEvents(newplayer);
    }

    private void OnBattleEnter() { 
        arenaCenter = GameObject.FindGameObjectWithTag("ArenaCenter");

        if(LevelManager.IsMidBossAlive) {
            arenaRay = midBossArenaRay;
        }
        else if(LevelManager.IsBossAlive) {
            arenaRay = meleeBossArenaRay;
        }
        
        if (arenaCenter != null) {
            transform.position = arenaCenter.transform.position;
        }
    }

    private void OnBattleExit() {
        // And we spawn 3-4 allied Imps as reward
        //nt impNumber = AlliesManager.Instance.AlliesList.Count;

        //int impMissing = ImpMaxNumber - impNumber;

        //if(impMissing >= impRewardAfterBossBattle) { 
            //int counter = impRewardAfterBossBattle;
            
            //while(counter > 0) { 
            //    AlliesManager.Instance.SpawnAlly(impPrefab, SpawnPosition());
                
            //    counter--;
            //}
        //}
    }
}
