using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    #region fields
    
    public enum Type
    {
        Player,
        Enemy,
        Ally,
        Boss,
        None
    }

    // -TODO- set property?
    [SerializeField]
    [Tooltip("The type of this demon")]
    public Type type = Type.None;

    // -TODO- set property?
    [SerializeField]
    [Tooltip("Starting health of this demon")]
    public int health = 2;

    [SerializeField]
    [Tooltip("How much damage this demon can deal")]
    private int damage = 1;

    [SerializeField]
    [Tooltip("How far the attack collider will go")]
    private float attackRange = 1.0f;

    //[SerializeField]
    //private int attackChance = 0;

    //[SerializeField]
    //private int blockChanceBonus = 0;

    [SerializeField]
    [Range(0f,100f)]
    [Tooltip("The probability for this demon to dodge an attack. to stick to GDD it should be 75.0")]
    private float blockChance = 0f;

    [SerializeField]
    [Range(0f,100f)]
    [Tooltip("This add to blockChance to increase the probability to block an attack. to stick to GDD it should be 15.0")]
    private float shieldBonusProbability = 0f;

    [SerializeField]
    [Range(0f,100f)]
    [Tooltip("For Boss only")]
    private float knockBackChance = 0f;

    [SerializeField]
    [Tooltip("For Boss only")]
    private float knockBackUnits = 0f;

    [SerializeField]
    [Tooltip("For boss only")]
    private float knockBackSpeed = 5.0f;

    // -TODO- Manage aggro 
    [SerializeField]
    private int aggro = 0;

    // -TODO- Manage Crisis
    [SerializeField]
    private int crisis = 0;

    [SerializeField]
    [Tooltip("How fast will be the attackCollider movement")]
    private float attackDurationMultiplier = 1.0f;

    private bool isProcessingKnockBack = false;
    private bool isIdle = true;
    private bool isBlocking = false;

    #endregion

    #region properties

    public float AttackDurationMultiplier { get => attackDurationMultiplier; private set => attackDurationMultiplier = value; }
    public float AttackRange { get => attackRange; private set => attackRange = value; }
    public int Damage { get => damage; private set => damage = value; }
    //public int AttackChance { get => attackChance; private set => attackChance = value; }
    //public int BlockChanceBonus { get => blockChanceBonus; private set => blockChanceBonus = value; }
    public float BlockChance { get => blockChance; set => blockChance = value; }
    public float ShieldBonusProbability { get => shieldBonusProbability; set => shieldBonusProbability = value; }
    public int Aggro { get => aggro; set => aggro = value; }
    public int Crisis { get => crisis; set => crisis = value; }
    public float KnockBackChance { get => knockBackChance; set => knockBackChance = value; }
    public float KnockBackUnits { get => knockBackUnits; set => knockBackUnits = value; }
    public bool IsIdle { get => isIdle; set => isIdle = value; }
    public bool IsBlocking { get => isBlocking; set => isBlocking = value; }

    #endregion

    #region methods

    /// <summary>
    /// Raise this unit aggro points by amount n
    /// </summary>
    /// <param name="n">The amount the aggro will be raised</param>
    public void RaiseAggro(int n) { 
        this.aggro += n;   
    }

    /// <summary>
    /// Lower this unit aggro point by amount n
    /// </summary>
    /// <param name="n">The amount the aggro will be lowered</param>
    public void LowerAggro(int n) { 
        if(aggro - n >= 0)
            aggro -= n;
        else
            aggro = 0;
    }

    /// <summary>
    /// Lower this unit health by amount n
    /// </summary>
    /// <param name="damage">The damage that this unit will take</param>
    public void TakeHit(int damage) { 
        health -= damage;    
    }

    /// <summary>
    /// Knockback this unit
    /// </summary>
    /// <param name="units">Knockback meters size</param>
    /// <param name="attackerTransform">Transform of the unit that is causing the knockback</param>
    public void TakeKnockBack(float units, Transform attackerTransform) { 
        if(!isProcessingKnockBack)
            StartCoroutine(TakeKnockBackCR(units,attackerTransform));
    }

    private IEnumerator TakeKnockBackCR(float units, Transform attackerTransform) {
        isProcessingKnockBack = true;

        float lerpTimer = 0f;

        Vector3 startPosition = this.transform.position;
        Vector3 targetPosition = startPosition + attackerTransform.forward * units; 
           
        while(Vector3.Distance(this.transform.position,targetPosition) > 0.1f) { 
            this.transform.position = Vector3.Lerp(startPosition,targetPosition,lerpTimer * knockBackSpeed);
            
            lerpTimer += Time.deltaTime;

            yield return null;
        }
      
        isProcessingKnockBack = false;
    }

    public bool CalculateBeenHitChance(bool isBlocking) {
        switch (type)
        {
            case Stats.Type.Ally:
                if (isBlocking) 
                {
                    // 0.9: hardcoded value for support units bonus
                    // 4:   hardcoded value for number of support units
                    return Random.Range(1f,101f) <= (100 - (blockChance + shieldBonusProbability)) * Mathf.Pow(0.9f, 4);
                }
                else 
                {
                    // 0.9: hardcoded value for support units bonus
                    // 4:   hardcoded value for number of support units
                    return Random.Range(1f, 101f) <=  (100 - blockChance) * Mathf.Pow(0.9f,4);    
                }
            case Stats.Type.Player:
                if (isBlocking) 
                {   
                    // When Player is blocking will allways avoid damage
                    return false;
                }
                else 
                { 
                    // When Playes is not blocking will allways take damage
                    return true; 
                }
            case Stats.Type.Enemy:
                if (isBlocking) 
                {
                    // TODO - Enemies will have support units?
                    return Random.Range(1f, 101f) <= (100 - blockChance + shieldBonusProbability);
                }
                else 
                {
                    // TODO - Enemies will have support units?
                    return Random.Range(1f,101f) <= (100 - blockChance);
                }
            case Stats.Type.Boss:
                if (isBlocking) 
                { 
                    // TODO - Boss will have support units?
                    return Random.Range(1f,101f) <= (100 - blockChance + shieldBonusProbability);    
                }
                else 
                { 
                    // TODO - Boss will have support units?
                    return Random.Range(1,101f) <= (100 - blockChance);    
                }
            default:
                Debug.Log(this.transform.root.name + " Stats error, did you set type?");
                return false;
        }
    }

    #endregion
}
