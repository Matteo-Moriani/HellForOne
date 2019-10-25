using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveBattle : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> invisibleWalls;

    [SerializeField]
    private List<GameObject> enemies;

    private bool alreadyTriggered = false;
    // Start is called before the first frame update
    void Start()
    {
        alreadyTriggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!alreadyTriggered && other.gameObject.CompareTag("Player"))
        {
            foreach(GameObject wall in invisibleWalls)
            {
                wall.GetComponent<Collider>().isTrigger = false;
            }

            foreach(GameObject group in GameObject.FindGameObjectsWithTag("Group"))
            {
                group.GetComponent<GroupMovement>().ChangeTarget();
            }

            foreach(GameObject enemy in enemies)
            {
                if (enemy.CompareTag("Boss"))
                {
                    enemy.GetComponent<BossBehavior>().enabled = true;
                }
            }
            alreadyTriggered = true;
        }
    }

}
