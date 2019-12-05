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
    public float health = 2f;

    /// <summary>
    /// How much meleeDamage this unit will deal
    /// </summary>
    [SerializeField]
    [Tooltip("How much meleeDamage this demon can deal")]
    private float meleeDamage = 2f;

    [SerializeField]
    [Tooltip("How mush ranged damage this demon can deal")]
    private float rangedDamage = 1f;

    [SerializeField]
    private float supportDamageBuffMultiplier = 3.5f;

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
    [Range(0f, 100f)]
    [Tooltip("The probability for this demon to dodge an attack. to stick to GDD it should be 75.0")]
    private float blockChance = 0f;
    /// <summary>
    /// Probability bonus if this unit is blocking
    /// </summary>
    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("This add to blockChance to increase the probability to block an attack. to stick to GDD it should be 15.0")]
    private float shieldBonusProbability = 0f;

    [SerializeField]
    private float supportingUnitsMultiplier = 0.95f;

    /// <summary>
    /// Probability of this unit to deal a knockBack
    /// </summary>
    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("For Boss only")]
    private float knockBackChance = 0f;
    /// <summary>
    /// How far this unit will push a target when dealing a knockBack
    /// </summary>
    [SerializeField]
    [Tooltip("For Boss only")]
    private float knockBackSize = 0f;
    /// <summary>
    /// How many seconds will take to go through knockBackSize
    /// </summary>
    [SerializeField]
    [Tooltip("For boss only")]
    private float knockBackTime = 5.0f;

    [Tooltip("Do not change, here only for balancing and testing")]
    [SerializeField]
    private float aggro = 1f;

    //[SerializeField]
    //[Tooltip("How much aggro will be subtracted every aggroTime")]
    //private int aggroDescreasingRateo = 1;

    [SerializeField]
    [Tooltip("How many seconds will pass before decreasing aggro")]
    private float aggroTime = 60.0f;

    //Used for aggro decreasing
    private DemonBehaviour demonBehaviour;
    Coroutine aggroDecreasingCR = null;

    // -TODO- Manage Crisis
    [SerializeField]
    private int crisis = 0;

    /// <summary>
    /// Tells if we are processing a KnockBack
    /// </summary>
    private bool isProcessingKnockBack = false;
    /// <summary>
    /// Tells if this unit can process a KnockBack
    /// </summary>
    private bool canProcessKnockBack = true;
    
    private Coroutine knockBackCR = null;

    private bool isPushedAway = false;

    /// <summary>
    /// Tells if this unit is Idle (not blocking)
    /// </summary>
    private bool combatIdle = true;
    /// <summary>
    /// Tells if this unit is blocking
    /// </summary>
    private bool isBlocking = false;
    /// <summary>
    /// Tells if this unit is blocking
    /// </summary>
    private bool isSupporting = false;

    // Used to store all groups reference
    // Using this we can avoid a lot of FindGameObjectsWithTag
    private GameObject[] groups;

    private bool isDying = false;
    
    private float deathDuration;
    private Coroutine deathCR;

    private CombatEventsManager combatEventsManager;

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
    /// How much meleeDamage will deal this unit
    /// </summary>
    public float MeleeDamage { get => meleeDamage; private set => meleeDamage = value; }
    /// <summary>
    /// How much ranged damage will deal this unit
    /// </summary>
    public float RangedDamage { get => rangedDamage; set => rangedDamage = value; }
    /// <summary>
    /// Probability of this unit to dodge an attack
    /// </summary>
    public float BlockChance { get => blockChance; set => blockChance = value; }
    /// <summary>
    /// Probability bonus if this unit is blocking
    /// </summary>
    public float ShieldBonusProbability { get => shieldBonusProbability; set => shieldBonusProbability = value; }

    public float Aggro { get => aggro; set => aggro = value; }
    public int Crisis { get => crisis; set => crisis = value; }

    /// <summary>
    /// Probability of this unit to deal a knockBack
    /// </summary>
    public float KnockBackChance { get => knockBackChance; set => knockBackChance = value; }
    /// <summary>
    /// How far this unit will push a target when dealing a knockBack
    /// </summary>
    public float KnockBackSize { get => knockBackSize; set => knockBackSize = value; }
    /// <summary>
    /// How many seconds will take to go through knockBackSize
    /// </summary>
    public float KnockBackTime { get => knockBackTime; set => knockBackTime = value; }
    /// <summary>
    /// Tells if the unit is Idle (not blocking)
    /// </summary>
    public bool CombatIdle { get => combatIdle; set => combatIdle = value; }
    /// <summary>
    /// Tells is the unit is blocking
    /// </summary>
    public bool IsBlocking { get => isBlocking; set => isBlocking = value; }
    /// <summary>
    /// Tells is the unit is supporting
    /// </summary>
    public bool IsSupporting { get => isSupporting; set => isSupporting = value; }
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

    public bool IsPushedAway { get => isPushedAway; set => isPushedAway = value; }

    public GameObject[] Groups { get => groups; private set => groups = value; }
    public float SupportDamageBuffMultiplier { get => supportDamageBuffMultiplier; set => supportDamageBuffMultiplier = value; }

    #endregion

    #region methods

    private void Start()
    {
        // TODO - everyone will have a death animation
        if(type == Stats.Type.Boss)
            deathDuration = GetComponent<AnimationsManager>().GetAnimation("Death").length;

        if (aggroDecreasingCR == null)
        {
            aggroDecreasingCR = StartCoroutine(AggroDecreasingCR());
        }

        Groups = GameObject.FindGameObjectsWithTag("Group");

        combatEventsManager = GetComponent<CombatEventsManager>();

        //ManageSpawn();
    }

    /// <summary>
    /// Raise this unit aggro points by amount n
    /// </summary>
    /// <param name="n">The amount the aggro will be raised</param>
    public void RaiseAggro(float n)
    {
        aggro *= n;
    }

    /// <summary>
    /// Lower this unit aggro points by amount n
    /// </summary>
    /// <param name="n"></param>
    public void LowerAggro(float n)
    {
        aggro = aggro / n;

        if (aggro < 1f)
            aggro = 1f;
    }

    /// <summary>
    /// Lower this unit health by amount n
    /// </summary>
    /// <param name="damage">The meleeDamage that this unit will take</param>
    public void TakeHit(float damage)
    {
        //combatEventsManager.RaiseOnBeenHit();

        health -= damage;

        if (health <= 0)
        {
            ManageDeath();
        }
    }

    /// <summary>
    /// Knockback this unit
    /// </summary>
    /// <param name="units">Knockback meters size</param>
    /// <param name="attackerTransform">Transform of the unit that is causing the knockback</param>
    public void TakeKnockBack(float units, Transform attackerTransform, float knockBackSpeed)
    {
        if (!isProcessingKnockBack && knockBackCR == null)
            knockBackCR = StartCoroutine(TakeKnockBackCR(units, attackerTransform, knockBackSpeed));
    }

    private void ManageMovement(bool enable)
    {
        // If is processing a KnockBack the Player cannot move or dash
        if (type == Stats.Type.Player)
        {
            Controller controller = this.GetComponent<Controller>();
            Dash dash = this.GetComponent<Dash>();

            controller.enabled = enable;
            dash.enabled = enable;
        }

        if (type == Stats.Type.Ally)
        {
            DemonMovement dm = GetComponent<DemonMovement>();

            if (dm != null)
            {
                dm.CanMove = enable;
            }
            else
            {
                Debug.Log(this.transform.root.name + " ManageMovement cannot find DemonMovement ");
            }
        }
    }

    public bool CalculateBeenHitChance(bool isBlocking)
    {
        switch (type)
        {
            case Stats.Type.Ally:
                // Calculate supporting units number
                int supportingUnits = 0;

                if (Groups != null)
                {
                    foreach (GameObject group in Groups)
                    {
                        if (group != null)
                            supportingUnits += group.GetComponent<GroupSupport>().SupportingUnits;
                        else
                            Debug.Log(this.gameObject.name + " a group in Stats.groups is null");
                    }
                }

                if (isBlocking)
                {

                    // 0.9: hardcoded value for support units bonus
                    // 4:   hardcoded value for number of support units
                    return Random.Range(1f, 101f) <= (100 - (blockChance + shieldBonusProbability) * Mathf.Pow(supportingUnitsMultiplier, supportingUnits));
                }
                else
                {
                    // 0.9: hardcoded value for support units bonus
                    // 4:   hardcoded value for number of support units
                    return Random.Range(1f, 101f) <= (100 - blockChance) * Mathf.Pow(supportingUnitsMultiplier, supportingUnits);
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
                    return Random.Range(1f, 101f) <= (100 - blockChance);
                }
            case Stats.Type.Boss:
                if (isBlocking)
                {
                    // TODO - Boss will have support units?
                    return Random.Range(1f, 101f) <= (100 - blockChance + shieldBonusProbability);
                }
                else
                {
                    // TODO - Boss will have support units?
                    return Random.Range(1, 101f) <= (100 - blockChance);
                }
            default:
                Debug.Log(this.transform.root.name + " Stats error, did you set type?");
                return false;
        }
    }

    private void ManageDeath()
    {
        aggro = 1;

        // If an ally is dying...
        if (type == Stats.Type.Ally)
        {
            // ...we need to update his group
            GroupBehaviour gb = gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupBehaviour>();
            gb.SetDemonsNumber(gb.GetDemonsNumber() - 1);
            
            // ...and remove him from his group
            int demonIndex = System.Array.IndexOf(gb.demons, this.gameObject);
            gb.demons[demonIndex] = null;

            // ...we need to update his group aggro
            GroupAggro ga = transform.root.gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupAggro>();
            if (ga != null)
            {
                ga.UpdateGroupAggro();
            }

            // ...if the unit is supporting we have to Update his gruop supporting units number
            if (IsSupporting)
            {
                GroupSupport gs = transform.root.gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupSupport>();

                if (gs != null)
                {
                    gs.UpdateSupportingUnits();
                }
            }

            AlliesManager.Instance.AllyKilled(this.gameObject);
        }

        // if the player is dying...
        if (type == Type.Player)
        {
            // It only works if Hat is the first child of Imp
            if (!GameObject.Find("Hat(Clone)"))
            {
                GameObject hat = Instantiate(Resources.Load("Prefabs/Hat"), transform.position + new Vector3(0, 5, 0), Quaternion.identity) as GameObject;
                hat.GetComponent<Hat>().PlayerDied();
            }
            GetComponent<Reincarnation>().Reincarnate();
        }

        // if the boss is dying...
        if (type == Type.Boss)
        {
            // TODO - Implement death animation using events
            if (!isDying)
            {
                //gameObject.GetComponent<BossAnimator>().StopAnimations();
                //gameObject.GetComponent<BossAnimator>().PlayAnimation(BossAnimator.Animations.Death);
                combatEventsManager.RaiseOnDeath();
            }
            isDying = true;

            // Update EnemiesManager boss
            EnemiesManager.Instance.BossKilled();
        }

        // if a littleEnemy is dying...
        if (type == Type.Enemy)
        {
            // Update EnemiesManager littleEnemiesList
            EnemiesManager.Instance.LittleEnemyKilled(this.gameObject);
        }

        // Other events related to death
        if(combatEventsManager != null) {
            //combatEventsManager.RaiseOnStopAnimation();
            combatEventsManager.RaiseOnDeath();
        }

        deathCR = StartCoroutine(DeathTimer(deathDuration));
    }

    // TODO - Do not use this, I'm testing this
    /*
    private void ManageSpawn() {
        switch (type) { 
            case Type.Ally:
                AlliesManager.Instance.AddAlly(this.gameObject);
                break;
            case Type.Enemy:
                EnemiesManager.Instance.AddEnemy(this.gameObject);
                break;
            case Type.Boss:
                EnemiesManager.Instance.AddBoss(this.gameObject);
                break;
        }
    }
    */

    #endregion

    #region Coroutines

    private IEnumerator TakeKnockBackCR(float AttackerKnockBackSize, Transform attackerTransform, float attackerKnockBackTime)
    {
        // We are processing the KnockBack
        isProcessingKnockBack = true;

        float knockBackTimeCounter = 0f;

        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 startingVelocity = rb.velocity;

        if (type == Stats.Type.Player)
        {
            rb.interpolation = RigidbodyInterpolation.Extrapolate;
        }

        // The Player or Ally imps cannot move or dash if is processig KnockBack
        ManageMovement(false);

        // Calculate KnocKback direction 
        Vector3 knockBackDirection = this.transform.position - attackerTransform.position;
        knockBackDirection.y = 0.0f;
        knockBackDirection = knockBackDirection.normalized;

        // KnockBack lerp
        do
        {
            // Use FixedDeltaTimeInstead?
            knockBackTimeCounter += Time.fixedDeltaTime;

            rb.velocity = knockBackDirection * (AttackerKnockBackSize / attackerKnockBackTime);

            // We are using physics so we need to wait for FixedUpdate
            yield return new WaitForFixedUpdate();
        } while (knockBackTimeCounter <= attackerKnockBackTime);

        // Player or Ally imps now can move or dash
        ManageMovement(true);

        rb.velocity = startingVelocity;

        if (type == Stats.Type.Player)
        {
            rb.interpolation = RigidbodyInterpolation.None;
        }

        // We are done processing the KnockBack
        isProcessingKnockBack = false;

        yield return null;
        knockBackCR = null;

    }

    private IEnumerator AggroDecreasingCR()
    {

        while (true)
        {
            yield return new WaitForSeconds(aggroTime);
            // only the player will reduce the aggro to everyone
            if (type == Type.Player)
            {
                float maxAggro = 1;
                GameObject[] groups = GameObject.FindGameObjectsWithTag("Group");

                foreach (GameObject group in groups)
                {
                    if (maxAggro < group.GetComponent<GroupAggro>().GetAggro())
                        maxAggro = group.GetComponent<GroupAggro>().GetAggro();
                }
                if (maxAggro < aggro)
                    maxAggro = aggro;

                // the max aggro group will have 10 as new value
                foreach (GameObject group in groups)
                {
                    foreach (GameObject demon in group.GetComponent<GroupBehaviour>().demons)
                    {
                        if (demon)
                            demon.GetComponent<Stats>().LowerAggro(maxAggro / group.GetComponent<GroupBehaviour>().GetDemonsNumber() * 10);
                    }

                    group.GetComponent<GroupAggro>().UpdateGroupAggro();
                }
                // for the player
                LowerAggro(maxAggro / 10);
            }

        }
    }

    private IEnumerator DeathTimer(float s)
    {
        yield return new WaitForSeconds(s);
        Destroy(gameObject);
    }

    #endregion
}
