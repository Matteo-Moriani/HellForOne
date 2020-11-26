using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public List<GameObject> littleEnemiesList;

    private GameObject boss;

    private static EnemiesManager _instance;

    public static EnemiesManager Instance { get { return _instance; } }
    public List<GameObject> LittleEnemiesList { get => littleEnemiesList; private set => littleEnemiesList = value; }
    public GameObject Boss { get => boss; private set => boss = value; }

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

    private void OnEnable()
    {
        BattleEventsManager.onBattleEnter += FindBoss;
    }
    
    private void OnDisable()
    {
        BattleEventsManager.onBattleEnter -= FindBoss;
    }

    private void FindBoss() { 
        Boss = GameObject.FindGameObjectWithTag("Boss");
        
        // TODO - Manage better this.
        BossBehavior bossBehaviour = boss.GetComponent<BossBehavior>();

        if(bossBehaviour != null) { 
            bossBehaviour.enabled = true;    
        }

        MidBossBehavior midBossBehavior = boss.GetComponent<MidBossBehavior>();

        if(midBossBehavior != null) { 
            midBossBehavior.enabled = true;    
        }
    }

    public void LittleEnemyKilled(GameObject littleEnemy) { 
        LittleEnemiesList.Remove(littleEnemy);       
    }

    public void BossKilled() { 
        Boss = null;    
    }

    /*
    public void AddEnemy(GameObject enemy) {
        if (!littleEnemiesList.Contains(enemy))
        {
            littleEnemiesList.Add(enemy);
        }
        else
        {
            // TODO - Im not 100% sure about this
            // Need to test
            //Destroy(enemy);

            // Need to find a way to manage this
        }
    }

    public void AddBoss(GameObject boss) {
        if (this.boss == null)
        {
            this.boss = boss;    
        }
        else
        {
            // TODO - Im not 100% sure about this
            // Need to test
            //Destroy(boss);

            // Need to find a way to manage this
        }
    }
    */
}
