using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenObjectsManager : MonoBehaviour
{
    public GameObject circle;
    public GameObject crown;
    public GameObject spear;
    public GameObject shield;

    public void ActivateCircle() {
        circle.SetActive(true);
        circle.GetComponent<MeshRenderer>().material = gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupManager>().groupColorMat;
    }

    public void DeactivateCircle() {
        circle.SetActive(false);
    }
}
