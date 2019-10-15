using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    private float aggro;

    void Awake() {
        NewRandomAggro();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateAggro());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetAggro() {
        return aggro;

    }

    private void NewRandomAggro() {
        aggro = Random.Range(1, 11);
    }

    private IEnumerator UpdateAggro() {
        while(true) {
            NewRandomAggro();
            yield return new WaitForSeconds(5);
        }
    }
}
