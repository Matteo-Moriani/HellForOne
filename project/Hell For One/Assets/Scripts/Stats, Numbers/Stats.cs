﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stats : MonoBehaviour
{
    #region fields

    /// <summary>
    /// Used to indicate the type of this unit
    /// </summary>
    public enum Type
    {
        Player,
        Enemy,
        Ally,
        Boss,
        None
    }

    [SerializeField]
    [Tooltip("The type of this demon")]
    private Type thisUnitType = Type.None;

    /// <summary>
    /// Health of this unit
    /// </summary>
    [SerializeField]
    [Tooltip("Starting health of this demon")]
    public float health = 2f;

    [SerializeField]
    [Tooltip("How much meleeDamage this demon can deal")]
    private float meleeDamage = 2f;

    [SerializeField]
    [Tooltip("How mush ranged damage this demon can deal")]
    private float rangedDamage = 1f;

    [SerializeField]
    private float supportDamageBuffMultiplier = 3.5f;

    [SerializeField]
    [Tooltip("How far the attack collider will go")]
    private float attackRange = 1.0f;
    
    [SerializeField]
    [Tooltip("How fast will be the attackCollider movement")]
    private float attackDurationMultiplier = 1.0f;

    [SerializeField]
    [Tooltip("How big sweep area will be")]
    private float groupAttackSize = 2.0f;
    
    /// <summary>
    /// Size of the global attack of this unit
    /// </summary>
    public float globalAttackSize = 666f;
    
    [SerializeField]
    [Tooltip("How long the global attack will be (in seconds)")]
    private float globalAttackDuration = 1.0f;

    public float attackingBlockChance = 60f;
    public float supportingBlockChance = 30f;
    public float blockingBlockChance = 90f;

    [Tooltip("Do not change, here only for balancing and testing")]
    [SerializeField]
    private float aggro = 1f;

    [SerializeField]
    [Tooltip("How many seconds will pass before decreasing aggro")]
    private float aggroTime = 60.0f;

    //Used for aggro decreasing
    private DemonBehaviour demonBehaviour;

    private Reincarnation reincarnation;
    
    Coroutine aggroDecreasingCR = null;

    // -TODO- Manage Crisis
    [SerializeField]
    private int crisis = 0;

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
    /// Tells if this unit is supporting
    /// </summary>
    private bool isSupporting = false;
    /// <summary>
    /// Tells if this unit is recruiting
    /// </summary>
    private bool isRecruiting = false;

    // Used to store all groups reference
    // Using this we can avoid a lot of FindGameObjectsWithTag
    private GameObject[] groups;

    private bool isDying = false;

    private float deathDuration = 1f;
    private Coroutine deathCR;

    private CombatEventsManager combatEventsManager;
    private KnockbackReceiver knockBackReceiver;

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
    /// Probability bonus if this unit is blocking
    /// </summary>
    //public float ShieldBonusProbability { get => shieldBonusProbability; set => shieldBonusProbability = value; }

    public float Aggro { get => aggro; set => aggro = value; }
    public int Crisis { get => crisis; set => crisis = value; }

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
    /// Tells is the unit is recruiting
    /// </summary>
    public bool IsRecruiting { get => isRecruiting; set => isRecruiting = value; }
    /// <summary>
    /// How big will be a sweep attack
    /// </summary>
    public float GroupAttackSize { get => groupAttackSize; private set => groupAttackSize = value; }
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
    
    public bool IsDying { get => isDying; private set => isDying = value; }

    /// <summary>
    /// The type of this unit
    /// </summary>
    public Type ThisUnitType { get => thisUnitType; private set => thisUnitType = value; }

    #endregion

    #region methods

    private void Awake()
    {
        combatEventsManager = this.gameObject.GetComponent<CombatEventsManager>();
        knockBackReceiver = this.gameObject.GetComponent<KnockbackReceiver>();
        reincarnation = this.gameObject.GetComponent<Reincarnation>();
    }

    private void OnEnable()
    {
        if(knockBackReceiver != null) { 
            knockBackReceiver.RegisterOnStartKnockback(DisableMovement);
            knockBackReceiver.RegisterOnEndKnockback(EnableMovement);
        }

        if(reincarnation != null) { 
            reincarnation.RegisterOnReincarnation(OnReincarnation);
        }
    }

    private void OnDisable()
    {
        if(knockBackReceiver != null) { 
            knockBackReceiver.UnRegisterOnStartKnockback(DisableMovement);
            knockBackReceiver.UnRegisterOnEndKnockback(EnableMovement);
        }
        if (reincarnation != null)
        {
            reincarnation.UnregisterOnReincarnation(OnReincarnation);
        }
    }

    private void Start()
    {
        AnimationsManager animationsManager = this.gameObject.GetComponent<AnimationsManager>();

        if(animationsManager != null) { 
            AnimationClip death = animationsManager.GetAnimation("Death");
            
            if(death != null) { 
                deathDuration = death.length;    
            }
        }

        if (aggroDecreasingCR == null)
        {
            aggroDecreasingCR = StartCoroutine(AggroDecreasingCR());
        }

        Groups = GameObject.FindGameObjectsWithTag("Group");

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

    private void EnableMovement() {
        // If is processing a KnockBack the Player cannot move or dash
        if (ThisUnitType == Stats.Type.Player)
        {
            Controller controller = this.GetComponent<Controller>();
            Dash dash = this.GetComponent<Dash>();

            controller.enabled = true;
            dash.enabled = true;
        }

        if (ThisUnitType == Stats.Type.Ally)
        {
            DemonMovement dm = GetComponent<DemonMovement>();

            if (dm != null)
            {
                dm.CanMove = true;
            }
            else
            {
                Debug.Log(this.transform.root.name + " ManageMovement cannot find DemonMovement ");
            }
        }
    }

    private void DisableMovement() {
        // If is processing a KnockBack the Player cannot move or dash
        if (ThisUnitType == Stats.Type.Player)
        {
            Controller controller = this.GetComponent<Controller>();
            Dash dash = this.GetComponent<Dash>();

            controller.enabled = false;
            dash.enabled = false;
        }

        if (ThisUnitType == Stats.Type.Ally)
        {
            DemonMovement dm = GetComponent<DemonMovement>();

            if (dm != null)
            {
                dm.CanMove = false;
            }
            else
            {
                Debug.Log(this.transform.root.name + " ManageMovement cannot find DemonMovement ");
            }
        }
    }

    public bool CalculateBeenHitChance(bool isBlocking)
    {
        switch (ThisUnitType)
        {
            case Stats.Type.Ally:
                // Support buff removed, we don't need this part anymore
                
                // Calculate supporting units number
                //int supportingUnits = 0;

                /*                
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
                */

                if (isBlocking)
                {
                    return Random.Range(0f, 100f) <= (100 - blockingBlockChance);
                }
                else
                {
                    if(gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupBehaviour>().currentState == GroupBehaviour.State.Support)
                        return Random.Range(0f, 100f) <= (100 - supportingBlockChance);
                    else
                        return Random.Range(0f, 100f) <= (100 - attackingBlockChance);
                }
            case Type.Player:
                if (isBlocking)
                {
                    // When Player is blocking will allways avoid damage
                    return false;
                }
                else
                {
                    // When Player is not blocking will allways take damage
                    return true;
                }
            case Type.Enemy:
                return true;
            case Type.Boss:
                return true;
            default:
                Debug.Log(this.transform.root.name + " Stats error, did you set type?");
                return false;
        }
    }

    private void ManageDeath()
    {
        if (!IsDying) { 
            IsDying = true;

            aggro = 1;

            // If an ally is dying...
            if (ThisUnitType == Stats.Type.Ally)
            {
                // ...we need to update his group
                GroupManager groupManager =  gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupManager>();
                if (groupManager != null)
                {
                    groupManager.RemoveImp(this.gameObject);
                }

                // ...we need to update his group aggro
                GroupAggro ga = transform.root.gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupAggro>();
                if (ga != null)
                {
                    ga.UpdateGroupAggro();
                }

                // ...if the unit is supporting we have to Update his group supporting units number
                if (IsSupporting)
                {
                    GroupSupport gs = transform.root.gameObject.GetComponent<DemonBehaviour>().groupBelongingTo.GetComponent<GroupSupport>();

                    if (gs != null)
                    {
                        gs.UpdateSupportingUnits();
                    }
                }

                // ...we need to disable his combat.
                Combat combat = this.gameObject.GetComponent<Combat>();

                if (combat != null)
                {
                    combat.enabled = false;
                }

                // ...we need to disable his demonBehaviour.
                DemonBehaviour demonBehaviour = this.gameObject.GetComponent<DemonBehaviour>();

                if (demonBehaviour != null)
                {
                    demonBehaviour.enabled = false;
                }

                // ...we need to disable his demonMovement.
                DemonMovement demonMovement = this.gameObject.GetComponent<DemonMovement>();

                if (demonMovement != null)
                {
                    demonMovement.enabled = false;
                }

                // ...we nned to disable his NavMeshAgent.
                NavMeshAgent navMeshAgent = this.GetComponent<NavMeshAgent>();

                if (navMeshAgent != null)
                {
                    navMeshAgent.enabled = false;
                }

                AlliesManager.Instance.AllyKilled(this.gameObject);
            }

            // if the player is dying...
            if (ThisUnitType == Type.Player)
            {
                // It only works if Hat is the first child of Imp - don't understand this check
                if (!GameObject.Find("Crown(Clone)"))
                {
                    // TODO - change someway this
                    GameObject crown = Instantiate(Resources.Load("Prefabs/Crown"), transform.position + new Vector3(0, 5, 0), Quaternion.identity) as GameObject;
                    crown.GetComponent<Hat>().PlayerDied();
                }
                
                GetComponent<Reincarnation>().Reincarnate();
            }

            // if the boss is dying...
            if (ThisUnitType == Type.Boss)
            {
                // Update EnemiesManager boss
                EnemiesManager.Instance.BossKilled();
            }

            // if a littleEnemy is dying...
            if (ThisUnitType == Type.Enemy)
            {
                // Update EnemiesManager littleEnemiesList
                EnemiesManager.Instance.LittleEnemyKilled(this.gameObject);
            }

            // Other events related to death
            if (combatEventsManager != null)
            {
                //combatEventsManager.RaiseOnStopAnimation();
                combatEventsManager.RaiseOnDeath();
            }
 
            // TODO - disable components for death duratin
            deathCR = StartCoroutine(DeathTimer(deathDuration));
        }
    }

    private void SetPlayerBaseHP() { 
        health  = 2.0f;    
    }

    private void SetAsPlayer() { 
        thisUnitType = Type.Player;    
    }

    private void OnReincarnation(GameObject player) { 
        SetAsPlayer();
        SetPlayerBaseHP();
    }
    
    #endregion

    #region Coroutines

    private IEnumerator AggroDecreasingCR()
    {

        while (true)
        {
            yield return new WaitForSeconds(aggroTime);
            // only the player will reduce the aggro to everyone
            if (ThisUnitType == Type.Player)
            {
                float maxAggro = 1;
                

                foreach (GameObject group in GroupsManager.Instance.Groups)
                {
                    if (maxAggro < group.GetComponent<GroupAggro>().GetAggro())
                        maxAggro = group.GetComponent<GroupAggro>().GetAggro();
                }
                if (maxAggro < aggro)
                    maxAggro = aggro;

                // the max aggro group will have 10 as new value
                foreach (GameObject group in GroupsManager.Instance.Groups)
                {
                    foreach (GameObject imp in group.GetComponent<GroupManager>().Imps)
                    {
                        if (imp)
                            imp.GetComponent<Stats>().LowerAggro(maxAggro / group.GetComponent<GroupManager>().ImpsInGroupNumber * 10);
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
