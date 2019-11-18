using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPositions : MonoBehaviour
{
    [SerializeField]
    private GameObject boss;
    private Transform[] meleePositions;
    private Transform[] rangedPositions;
    private Dictionary<Transform, bool> meleeAvailable = new Dictionary<Transform, bool>();
    private Dictionary<Transform, Transform> closestRanged = new Dictionary<Transform, Transform>();
    
    void Start()
    {
        // Cause we don't want to assign it manually
        boss = GameObject.FindGameObjectWithTag("Boss");
    }

    void Awake() {
        Transform[] tempPositions = gameObject.GetComponentsInChildren<Transform>();

        // 5 = the opposite 4 + myself
        meleePositions = new Transform[tempPositions.Length - 5];
        rangedPositions = new Transform[tempPositions.Length - 5];

        int j = 0;
        int k = 0;

        for(int i = 0; i<tempPositions.Length; i++) {
            if(tempPositions[i].CompareTag("MeleePosition")) {
                meleePositions[j] = tempPositions[i];
                j++;
            } else if(tempPositions[i].CompareTag("RangedPosition")) {
                rangedPositions[k] = tempPositions[i];
                k++;
            }
        }

        foreach(Transform position in meleePositions) {
            meleeAvailable.Add(position, true);
            closestRanged.Add(position, FindClosest(position));
        }
    }

    void FixedUpdate() {
        if(!boss)
            Destroy(gameObject);
        else
            transform.position = boss.transform.position;
    }

    public void SetAvailability(Transform t, bool b) {
        meleeAvailable[t] = b;
    }

    public bool GetAvailability(Transform t) {
        bool b;
        meleeAvailable.TryGetValue(t, out b);
        return b;
    }

    public Transform GetClosestRanged(Transform melee) {
        Transform ranged;
        closestRanged.TryGetValue(melee, out ranged);
        return ranged;
    }

    public Transform[] GetMeleePositions() {
        return meleePositions;
    }

    public Transform[] GetRangedPositions() {
        return rangedPositions;
    }

    public Transform FindClosest(Transform melee) {
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (Transform ranged in rangedPositions) {
            if((melee.position - ranged.position).magnitude < minDist) {
                minDist = (melee.position - ranged.position).magnitude;
                closest = ranged;
            }
        }

        return closest;
    }
}
