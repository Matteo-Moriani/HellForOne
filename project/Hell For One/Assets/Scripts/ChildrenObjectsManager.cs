﻿using System.Collections;
using System.Collections.Generic;
using AI.Imp;
using Groups;
using UnityEngine;

public class ChildrenObjectsManager : MonoBehaviour
{
    public GameObject circle;
    public GameObject crown;
    public GameObject spear;
    public GameObject shield;
    public GameObject scepter;

    public void ActivateCircle() {
        circle.SetActive(true);
        circle.GetComponent<MeshRenderer>().material = gameObject.GetComponent<GroupFinder>().GroupBelongingTo.GetComponent<GroupManager>().groupColorMat;
    }

    public void DeactivateCircle() {
        circle.SetActive(false);
    }
}
