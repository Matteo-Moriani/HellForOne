using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyDemonSpawnerTest : MonoBehaviour
{
    public float timer = 30f;
    public float countdown;
    private float impMaxNumber = 16;
    public float ImpMaxNumber { get => impMaxNumber; set => impMaxNumber = value; }
    private bool needForRegen = true;
    private Coroutine spawnAllyCR;
    public Coroutine SpawnAllyCR { get => spawnAllyCR; set => spawnAllyCR = value; }

    public IEnumerator SpawnAlly()
    {
        while ( true )
        {
            int impNumber = GameObject.FindGameObjectsWithTag("Demon").Length;

            // 0 demons = game over, max number of demons = no need for spawn ally
            if ( impNumber >= ImpMaxNumber * 0.75 && impNumber < ImpMaxNumber ) {
                needForRegen = true;
                timer = 45;
            }
            else if (impNumber >= ImpMaxNumber * 0.25 && impNumber < ImpMaxNumber * 0.75) {
                needForRegen = true;
                timer = 30;
            }
            else if (impNumber >= 1 && impNumber < ImpMaxNumber * 0.25) {
                needForRegen = true;
                timer = 15;
            }
            else {
                needForRegen = false;
            }

            if(needForRegen) {
                yield return new WaitForSeconds(timer);
                GameObject demonToSpawn = Resources.Load("Prefabs/FakeImp") as GameObject;

                // TODO - We need to spawn the ally via AlliesManager
                AlliesManager.Instance.SpawnAlly(demonToSpawn, SpawnPosition());
            }
        }
    }

    // TODO - imps must spawn at the borders of the arena: x is random between -ray and +ray of the arena, z must be that value - the ray *1 or *-1
    public Vector3 SpawnPosition()
    {
        Vector3 spawnPosition = new Vector3( 0, 1, 0 );
        spawnPosition.x = Random.Range( -10f, 10f );
        spawnPosition.z = Random.Range( -10f, 10f );
        return spawnPosition;
    }

    void Start()
    {
        SpawnAllyCR = StartCoroutine( SpawnAlly() );
        countdown = timer;
    }

    private void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            countdown = timer;
        }
    }
}
