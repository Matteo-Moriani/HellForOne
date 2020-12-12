using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackCaster : MonoBehaviour
{
    private KnockbackValues values;
    
    public KnockbackValues Values
    {
        get => values;
        private set => values = value;
    }
}

[CreateAssetMenu(fileName = "KnockbackValues", menuName = "CombatSystem/KnockbackValues", order = 1)]
public class KnockbackValues : ScriptableObject
{
    [SerializeField]
    [Tooltip("How far this unit will push a target when dealing a knockBack")]
    private float knockBackSize = 0f;
    
    [SerializeField]
    [Tooltip("How many seconds will take to go through knockBackSize")]
    private float knockBackTime = 5.0f;
    
    /// <summary>
    /// How far this unit will push a target when dealing a knockBack
    /// </summary>
    public float KnockBackSize { get => knockBackSize; private set => knockBackSize = value; }
    /// <summary>
    /// How many seconds will take to go through knockBackSize
    /// </summary>
    public float KnockBackTime { get => knockBackTime; private set => knockBackTime = value; }
}
