﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyDemonSpawnerTest : MonoBehaviour
{
    public float timer = 30f;
    private float countdown;
    private float impMaxNumber = 16;
    public float ImpMaxNumber { get => impMaxNumber; set => impMaxNumber = value; }
    private bool needForRegen = true;
    private Coroutine spawnAllyCR;
    public Coroutine SpawnAllyCR { get => spawnAllyCR; set => spawnAllyCR = value; }
    public float Countdown { get => countdown; set => countdown = value; }
    private int regenDemonsLeft;
    private GameObject levelManager;
    private float arenaRay;

    [SerializeField]
    private GameObject impPrefab;

    private GameObject arenaCenter;

    public IEnumerator SpawnAlly()
    {
        while ( regenDemonsLeft > 0 ) {

            int impNumber = AlliesManager.Instance.AlliesList.Count;

            // 0 demons = game over, max number of demons = no need for spawn ally
            if ( impNumber >= ImpMaxNumber * 0.8 && impNumber < ImpMaxNumber ) {            // 13-16
                needForRegen = true;
                timer = 45f;
            }
            else if (impNumber >= ImpMaxNumber * 0.4 && impNumber < ImpMaxNumber * 0.8) {   // 7-12
                needForRegen = true;
                timer = 30f;
            }
            else if ( impNumber > 0 && impNumber < ImpMaxNumber * 0.4 )                    // 1-6
            {
                needForRegen = true;
                timer = 15f;
            }
            else {
                needForRegen = false;
            }

            yield return new WaitForSeconds(timer);

            if(needForRegen) {
                //GameObject demonToSpawn = Resources.Load("Prefabs/FakeImp") as GameObject;

                // TODO - We need to spawn the ally via AlliesManager
                AlliesManager.Instance.SpawnAlly(impPrefab, SpawnPosition());
                regenDemonsLeft--;
            }
        }

        Debug.Log("no more demons will help you!");
    }
    
    public Vector3 SpawnPosition()
    {
        //circumference with a little adjustment of 1 in both coordinates to the center.
        Vector3 spawnPosition = new Vector3( 0, 0, 0 );
        spawnPosition.x = Random.Range(0f, arenaRay-1f);
        spawnPosition.z = Mathf.Sqrt(Mathf.Pow(arenaRay, 2f) - Mathf.Pow(spawnPosition.x+1f, 2f));     // circumference with the center in the origin: x^2 + y^2 = r^2
        spawnPosition.z -= 1f;
        if(Random.Range(0f, 1f) > 0.5f)
            spawnPosition.x = spawnPosition.x * -1;
        if(Random.Range(0f, 1f) > 0.5f)
            spawnPosition.z = spawnPosition.z * -1;

        Debug.Log(spawnPosition.x + " " + spawnPosition.z);
        spawnPosition =  this.transform.position + this.transform.forward * 10;
        spawnPosition.y = 1;
        return spawnPosition;
    }

    private void OnEnable()
    {
        BattleEventsManager.onBossBattleEnter += OnBossBattleEnter;
        BattleEventsManager.onBossBattleExit += OnBossBattleExit;
    }

    private void OnDisable()
    {
        BattleEventsManager.onBossBattleEnter -= OnBossBattleEnter;
        BattleEventsManager.onBossBattleExit -= OnBossBattleExit;
    }

    void Start()
    {
        levelManager = GameObject.Find("LevelManager");
    }

    private void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            countdown = timer;
        }
    }

    private void OnBossBattleEnter() { 
        arenaCenter = GameObject.FindGameObjectWithTag("ArenaCenter");

        if(levelManager.GetComponent<LevelManager>().IsMidBossAlive) {
            regenDemonsLeft = levelManager.GetComponent<LevelManager>().midBossTotRegenDemons;
            arenaRay = 12f;
        }
        else if(levelManager.GetComponent<LevelManager>().IsBossAlive) {
            regenDemonsLeft = levelManager.GetComponent<LevelManager>().bossTotRegenDemons;
            arenaRay = 19f;
        }
        
        if (arenaCenter != null) {
            this.transform.position = arenaCenter.transform.position;
        }
        else { 
            Debug.Log(this.gameObject.name + " cannot find arena center");    
        }
        

        if(spawnAllyCR == null) {
            countdown = timer;
            spawnAllyCR = StartCoroutine(SpawnAlly());
        }
    }

    private void OnBossBattleExit() { 
        if(spawnAllyCR != null) { 
            StopCoroutine(spawnAllyCR);
            spawnAllyCR = null;
        }    
    }
}
