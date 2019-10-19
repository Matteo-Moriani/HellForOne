using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPositions : MonoBehaviour
{

    private Transform[] positions;
    private Dictionary<Transform, bool> available = new Dictionary<Transform, bool>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake() {
        Transform[] tempPositions = gameObject.GetComponentsInChildren<Transform>();
        positions = new Transform[tempPositions.Length - 1];

        for(int i = 1; i<tempPositions.Length; i++) {
            positions[i - 1] = tempPositions[i];
        }

        foreach(Transform position in positions) {
            available.Add(position, true);
            Debug.Log(position.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAvailability(Transform t, bool b) {
        available[t] = b;
    }

    public bool GetAvailability(Transform t) {
        bool b;
        available.TryGetValue(t, out b);
        return b;
    }

    public Transform[] GetPositions() {
        return positions;
    }
}
