using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackCaster : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("Probability of this unit to deal a knockBack")]
    private float knockBackChance = 0f;
    
    [SerializeField]
    [Tooltip("How far this unit will push a target when dealing a knockBack")]
    private float knockBackSize = 0f;
    
    [SerializeField]
    [Tooltip("How many seconds will take to go through knockBackSize")]
    private float knockBackTime = 5.0f;

    /// <summary>
    /// Probability of this unit to deal a knockBack
    /// </summary>
    public float KnockBackChance { get => knockBackChance; private set => knockBackChance = value; }
    /// <summary>
    /// How far this unit will push a target when dealing a knockBack
    /// </summary>
    public float KnockBackSize { get => knockBackSize; private set => knockBackSize = value; }
    /// <summary>
    /// How many seconds will take to go through knockBackSize
    /// </summary>
    public float KnockBackTime { get => knockBackTime; private set => knockBackTime = value; }
}
