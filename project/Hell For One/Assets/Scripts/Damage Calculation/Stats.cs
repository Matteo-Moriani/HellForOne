using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    #region fields
    
    /// <summary>
    /// Useful to indicate the type of this unit
    /// </summary>
    public enum Type
    {
        Player,
        Enemy,
        Ally,
        Boss,
        None
    }
     
    /// <summary>
    /// The type of this unit
    /// </summary>
    [SerializeField]
    [Tooltip("The type of this demon")]
    public Type type = Type.None;

    /// <summary>
    /// Health of this unit
    /// </summary>
    [SerializeField]
    [Tooltip("Starting health of this demon")]
    public int health = 2;

    /// <summary>
    /// How much damage this unit will deal
    /// </summary>
    [SerializeField]
    [Tooltip("How much damage this demon can deal")]
    private int damage = 1;

    /// <summary>
    /// How far will go an attack
    /// </summary>
    [SerializeField]
    [Tooltip("How far the attack collider will go")]
    private float attackRange = 1.0f;
    /// <summary>
    /// How fast will be an attack
    /// </summary>
    [SerializeField]
    [Tooltip("How fast will be the attackCollider movement")]
    private float attackDurationMultiplier = 1.0f;

    /// <summary>
    /// How big will be a sweep attack
    /// </summary>
    [SerializeField]
    [Tooltip("How big sweep area will be")]
    private float sweepSize = 2.0f;

    /// <summary>
    /// How big will be a global attack
    /// </summary>
    [SerializeField]
    [Tooltip("How big the global attack will be")]
    private float globalAttackSize = 10f;
    /// <summary>
    /// How long will last a global attack
    /// </summary>
    [SerializeField]
    [Tooltip("How long the global attack will be (in seconds)")]
    private float globalAttackDuration = 1.0f;

    /// <summary>
    /// Probability of this unit to dodge an attack
    /// </summary>
    [SerializeField]
    [Range(0f,100f)]
    [Tooltip("The probability for this demon to dodge an attack. to stick to GDD it should be 75.0")]
    private float blockChance = 0f;
    /// <summary>
    /// Probability bonus if this unit is blocking
    /// </summary>
    [SerializeField]
    [Range(0f,100f)]
    [Tooltip("This add to blockChance to increase the probability to block an attack. to stick to GDD it should be 15.0")]
    private float shieldBonusProbability = 0f;

    /// <summary>
    /// Probability of this unit to deal a knockBack
    /// </summary>
    [SerializeField]
    [Range(0f,100f)]
    [Tooltip("For Boss only")]
    private float knockBackChance = 0f;
    /// <summary>
    /// How far this unit will push a target when dealing a knockBack
    /// </summary>
    [SerializeField]
    [Tooltip("For Boss only")]
    private float knockBackUnits = 0f;
    /// <summary>
    /// How far this unit will push a target when dealing a knockBack
    /// </summary>
    [SerializeField]
    [Tooltip("For boss only")]
    private float knockBackSpeed = 5.0f;

    [SerializeField]
    private int aggro = 0;

    // -TODO- Manage Crisis
    [SerializeField]
    private int crisis = 0;

    
    /// <summary>
    /// Tells if we are processing a knockBack
    /// </summary>
    private bool isProcessingKnockBack = false;
    /// <summary>
    /// Tells if this unit is Idle (not blocking)
    /// </summary>
    private bool isIdle = true;
    /// <summary>
    /// Tells if this unit is blocking
    /// </summary>
    private bool isBlocking = false;

    #endregion

    #region properties
    /// <summary>
    /// How fast will be an attack
    /// </summary>
    public float AttackDurationMultiplier { get => attackDurationMultiplier; private set => attackDurationMultiplier = value; }
    /// <summary>
    /// How far will go an attack
    /// </summary>
    public float AttackRange { get => attackRange; private set => attackRange = value; }
    /// <summary>
    /// How much damage will deal this unit
    /// </summary>
    public int Damage { get => damage; private set => damage = value; }
    /// <summary>
    /// Probability of this unit to dodge an attack
    /// </summary>
    public float BlockChance { get => blockChance; set => blockChance = value; }
    /// <summary>
    /// Probability bonus if this unit is blocking
    /// </summary>
    public float ShieldBonusProbability { get => shieldBonusProbability; set => shieldBonusProbability = value; }
    
    public int Aggro { get => aggro; set => aggro = value; }
    public int Crisis { get => crisis; set => crisis = value; }
    
    /// <summary>
    /// Probability of this unit to deal a knockBack
    /// </summary>
    public float KnockBackChance { get => knockBackChance; set => knockBackChance = value; }
    /// <summary>
    /// How far this unit will push a target when dealing a knockBack
    /// </summary>
    public float KnockBackUnits { get => knockBackUnits; set => knockBackUnits = value; }
    /// <summary>
    /// how fast this unit will push a target when dealing a knockBack
    /// </summary>
    public float KnockBackSpeed { get => knockBackSpeed; set => knockBackSpeed = value; }
    /// <summary>
    /// Tells if the unit is Idle (not blocking)
    /// </summary>
    public bool IsIdle { get => isIdle; set => isIdle = value; }
    /// <summary>
    /// Tells is the unit is blocking
    /// </summary>
    public bool IsBlocking { get => isBlocking; set => isBlocking = value; }
    /// <summary>
    /// How big will be a sweep attack
    /// </summary>
    public float SweepSize { get => sweepSize; set => sweepSize = value; }
    /// <summary>
    /// How big will be a global attack
    /// </summary>
    public float GlobalAttackSize { get => globalAttackSize; set => globalAttackSize = value; }
    /// <summary>
    /// How long will last a global attack
    /// </summary>
    public float GlobalAttackDuration { get => globalAttackDuration; set => globalAttackDuration = value; }
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
    public void ResetAggro(int n) { 
        aggro = 0;

        if (type == Stats.Type.Ally)
            this.transform.root.gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupAggro>().UpdateGruopAggro();
    }

    /// <summary>
    /// Lower this unit health by amount n
    /// </summary>
    /// <param name="damage">The damage that this unit will take</param>
    public void TakeHit(int damage) { 
        health -= damage;
        
        if(health <= 0) { 
            ManageDeath();    
        }
    }

    /// <summary>
    /// Knockback this unit
    /// </summary>
    /// <param name="units">Knockback meters size</param>
    /// <param name="attackerTransform">Transform of the unit that is causing the knockback</param>
    public void TakeKnockBack(float units, Transform attackerTransform, float knockBackSpeed) { 
        if(!isProcessingKnockBack)
            StartCoroutine(TakeKnockBackCR(units,attackerTransform,knockBackSpeed));
    }

    private IEnumerator TakeKnockBackCR(float units, Transform attackerTransform, float knockBackSpeed) {
        isProcessingKnockBack = true;

        float lerpTimer = 0f;

        Vector3 startPosition = this.transform.position;
        Vector3 targetPosition = startPosition + (attackerTransform.forward + (-this.transform.forward)).normalized * units; 
           
        while(Vector3.Distance(this.transform.position,targetPosition) > 0.2f) { 
            
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

    private void ManageDeath() {
        aggro = 0;

        if (type == Stats.Type.Ally)
            this.transform.root.gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupAggro>().UpdateGruopAggro();
        
        Destroy(this.gameObject);
    }

    #endregion
}
