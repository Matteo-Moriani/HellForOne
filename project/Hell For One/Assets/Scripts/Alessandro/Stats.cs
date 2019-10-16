using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public enum Type
    {
        Player,
        Enemy,
        Ally,
        Boss
    }

    [SerializeField]
    private Type type;

    [SerializeField]
    private int health;
}
