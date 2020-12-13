using System.Collections;
using System.Collections.Generic;
using Managers;
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
    
    // TODO - Refactor PushAway
    private bool isPushedAway = false;

    // TODO - Delete these after deleting CombatManager
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
    
    private bool isDying = false;
    private float deathDuration = 1f;
    private Coroutine deathCR;

    #endregion

    #region properties
    
    // TODO - Delete these after deleting CombatManager
    public bool CombatIdle { get => combatIdle; set => combatIdle = value; }
    public bool IsBlocking { get => isBlocking; set => isBlocking = value; }
    public bool IsSupporting { get => isSupporting; set => isSupporting = value; }
    public bool IsRecruiting { get => isRecruiting; set => isRecruiting = value; }
    public bool IsPushedAway { get => isPushedAway; set => isPushedAway = value; }
    public bool IsDying { get => isDying; private set => isDying = value; }

    /// <summary>
    /// The type of this unit
    /// </summary>
    public Type ThisUnitType { get => thisUnitType; private set => thisUnitType = value; }

    #endregion

    #region Delegates and events

    public delegate void OnDeath(Stats sender);
    public event OnDeath onDeath;

    public delegate void OnLateDeath(Stats sender);
    public event OnLateDeath onLateDeath;

    #region Methods

    private void RaiseOnLateDeath()
    {
        onLateDeath?.Invoke(this);
    }

    private void RaiseOnDeath()
    {
        onDeath?.Invoke(this);
    }

    #endregion
    
    #endregion
    
    #region methods

    private void OnEnable()
    {
        Reincarnation reincarnation = GetComponent<Reincarnation>();
        if(reincarnation != null)
            reincarnation.onReincarnation += OnReincarnation;
        
        IdleCombat idleCombat = GetComponentInChildren<IdleCombat>();
        if (idleCombat != null)
            idleCombat.onAttackBeingHit += OnAttackBeingHit;
        
        // Block block = GetComponentInChildren<Block>();
        // if (block != null)
        //     block.onBlockFailed += OnBlockFailed;
    }

    private void OnDisable()
    {
        Reincarnation reincarnation = GetComponent<Reincarnation>();
        if (reincarnation != null)
            reincarnation.onReincarnation -= OnReincarnation;
        
        IdleCombat idleCombat = GetComponentInChildren<IdleCombat>();
        if (idleCombat != null)
            idleCombat.onAttackBeingHit -= OnAttackBeingHit;
        
        // Block block = GetComponentInChildren<Block>();
        // if (block != null)
        //     block.onBlockFailed -= OnBlockFailed;
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
    }
    
    public void TakeHit(float damage)
    {
        // TODO - remove after testing
        //Debug.Log(gameObject.name + " took " + damage + " damage ");
        
        health -= damage;

        if (health <= 0)
        {
            ManageDeath();
        }
    }
    
    // TODO - Refactor death
    private void ManageDeath()
    {
        if (!IsDying) { 
            IsDying = true;

            // If an ally is dying...
            // if (ThisUnitType == Stats.Type.Ally)
            // {
            //     // TODO - Manage this inside DemonMovement with OnDeath event
            //     // ...we need to disable his demonMovement.
            //     //AllyImpMovement demonMovement = gameObject.GetComponent<AllyImpMovement>();
            //
            //     // if (demonMovement != null)
            //     // {
            //     //     demonMovement.enabled = false;
            //     // }
            //
            //     // ...we nned to disable his NavMeshAgent.
            //     NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            //
            //     if (navMeshAgent != null)
            //     {
            //         navMeshAgent.enabled = false;
            //     }
            // }
            //
            // // TODO - Manage player death with OnDeath event
            // // if the player is dying...
            // if (ThisUnitType == Type.Player)
            // {
            //     // It only works if Hat is the first child of Imp - don't understand this check
            //     if (!GameObject.Find("Crown(Clone)"))
            //     {
            //         // TODO - change someway this
            //         GameObject crown = Instantiate(LevelManager.Instance.GetCrown(), transform.position + new Vector3(0, 5, 0), Quaternion.identity);
            //         crown.GetComponent<Hat>().PlayerDied();
            //     }
            //     
            //     GetComponent<Reincarnation>().Reincarnate();
            // }
            //
            // // TODO - Manage Boss death with OnDeath event
            // // if the boss is dying...
            // if (ThisUnitType == Type.Boss)
            // {
            //     // Update EnemiesManager boss
            //     EnemiesManager.Instance.BossKilled();
            // }
            //
            // // TODO - Remove LittleEnemy logic
            // // if a littleEnemy is dying...
            // if (ThisUnitType == Type.Enemy)
            // {
            //     // Update EnemiesManager littleEnemiesList
            //     EnemiesManager.Instance.LittleEnemyKilled(this.gameObject);
            // }
            //
            // RaiseOnDeath();
            // RaiseOnLateDeath();
 
            // TODO - disable components for death duration
            
            // gameObject destory is now up to the animation event
        }
    }

    private void SetPlayerBaseHP() { 
        health  = 2.0f;    
    }

    private void SetAsPlayer() { 
        thisUnitType = Type.Player;    
    }
    
    #endregion

    #region EventHandlers

    // private void OnBlockFailed(Block sender, GenericAttack genericAttack,NormalCombat attackerNormalCombat)
    // {
    //     // TODO - AlliesList.Count is not #attackingUnits, take in account other orders too
    //     if(genericAttack.HasDamageSupportBonus && thisUnitType == Type.Boss)
    //         TakeHit(genericAttack.Damage + Support.SupportingUnits * (3.5f / (AlliesManager.Instance.AlliesList.Count - Support.SupportingUnits - Recruit.RecruitingUnits)));
    //     else
    //         TakeHit(genericAttack.Damage);
    // }

    private void OnAttackBeingHit(IdleCombat sender, GenericAttack genericAttack, NormalCombat attackerNormalCombat)
    {
        // // TODO - AlliesList.Count is not #attackingUnits, take in account other orders too
        // if(genericAttack.HasDamageSupportBonus && thisUnitType == Type.Boss)
        //     TakeHit(genericAttack.Damage + Support.SupportingUnits * (3.5f / (AlliesManager.Instance.AlliesList.Count - Support.SupportingUnits - Recruit.RecruitingUnits)));
        // else
        //     TakeHit(genericAttack.Damage);
    }

    private void OnReincarnation(GameObject player) 
    { 
        SetAsPlayer();
        SetPlayerBaseHP();
    }

    #endregion

}
