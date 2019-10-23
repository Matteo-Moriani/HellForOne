using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupsFormation : MonoBehaviour
{
    //public GameObject player;

    //private Transform[] positions;
    //private Dictionary<Transform, bool> meleeAvailable = new Dictionary<Transform, bool>();
    //private Dictionary<Transform, Transform> closestRanged = new Dictionary<Transform, Transform>();

    //void Start() {

    //}

    //void Awake() {
    //    Transform[] tempPositions = gameObject.GetComponentsInChildren<Transform>();
        
    //    positions = new Transform[tempPositions.Length - 1];

    //    for(int i = 1; i < tempPositions.Length; i++) {
    //        positions[i] = tempPositions[i];
    //    }

    //    // assegnare posizioni fisse a gruppi invece che cercare il primo disponibile. cambiare questa cosa anche in boss positions
    //    foreach(Transform position in positions) {
    //        meleeAvailable.Add(position, true);
    //        closestRanged.Add(position, FindClosest(position));
    //    }
    //}

    //void Update() {

    //}

    //void FixedUpdate() {
    //    transform.position = boss.transform.position;
    //}

    //public void SetAvailability(Transform t, bool b) {
    //    meleeAvailable[t] = b;
    //}

    //public bool GetAvailability(Transform t) {
    //    bool b;
    //    meleeAvailable.TryGetValue(t, out b);
    //    return b;
    //}

    //public Transform GetClosestRanged(Transform melee) {
    //    Transform ranged;
    //    closestRanged.TryGetValue(melee, out ranged);
    //    return ranged;
    //}

    //public Transform[] GetMeleePositions() {
    //    return positions;
    //}

    //public Transform[] GetRangedPositions() {
    //    return rangedPositions;
    //}

    //public Transform FindClosest(Transform melee) {
    //    Transform closest = null;
    //    float minDist = float.MaxValue;

    //    foreach(Transform ranged in rangedPositions) {
    //        if((melee.position - ranged.position).magnitude < minDist) {
    //            minDist = (melee.position - ranged.position).magnitude;
    //            closest = ranged;
    //        }
    //    }

    //    return closest;
    //}
}
