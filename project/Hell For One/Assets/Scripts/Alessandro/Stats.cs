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
        Boss,
        None
    }

    [SerializeField]
    private Type type = Type.None;

    [SerializeField]
    private int health = 2;

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private float attackRange = 1.0f;

    [SerializeField]
    private float attackDurationMultiplier = 1.0f;

    public float AttackDurationMultiplier { get => attackDurationMultiplier; private set => attackDurationMultiplier = value; }
    public float AttackRange { get => attackRange; private set => attackRange = value; }
    public int Damage { get => damage; private set => damage = value; }

    public void TakeHit(int damage) { 
        health -= damage;    
    }
}
