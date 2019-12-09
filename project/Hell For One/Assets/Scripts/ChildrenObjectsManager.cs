using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenObjectsManager : MonoBehaviour
{
    public GameObject circle;
    public GameObject crown;
    public GameObject spear;

    private void Start()
    {
        
    }

    public void ActivateCircle() {
        circle.SetActive(true);
        circle.GetComponent<MeshRenderer>().material = gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupBehaviour>().groupColor;
    }

    public void DeactivateCircle() {
        circle.SetActive(false);
    }
}
