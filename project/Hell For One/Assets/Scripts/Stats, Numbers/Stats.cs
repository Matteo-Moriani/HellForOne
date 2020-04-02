using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stats : MonoBehaviour
{
    #region fields

    private IdleCombat idleCombat;
    private Block block;
    private KnockbackReceiver knockbackReceiver;
    
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
    
    private Reincarnation reincarnation;

    // TODO - Refactor PushAway
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
    
    private bool isDying = false;
    private float deathDuration = 1f;
    private Coroutine deathCR;

    private CombatEventsManager combatEventsManager;
    
    #endregion

    #region properties
    
    // TODO - Refactor orders
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

    private void Awake()
    {
        combatEventsManager = this.gameObject.GetComponent<CombatEventsManager>();
        reincarnation = this.gameObject.GetComponent<Reincarnation>();
        
        idleCombat = this.gameObject.GetComponentInChildren<IdleCombat>();
        block = this.gameObject.GetComponentInChildren<Block>();
        knockbackReceiver = this.gameObject.GetComponentInChildren<KnockbackReceiver>();
    }

    private void OnEnable()
    {
        if(knockbackReceiver != null) {
            knockbackReceiver.onStartKnockback += OnStartKnockback;
            knockbackReceiver.onEndKnockback += OnEndKnockback;
        }

        if(reincarnation != null)
            reincarnation.onReincarnation += OnReincarnation;
        
        if (idleCombat != null)
            idleCombat.onNormalAttackBeingHit += OnNormalAttackBeingHit;
        
        if (block != null)
            block.onBlockFailed += OnBlockFailed;
    }

    private void OnDisable()
    {
        if(knockbackReceiver != null) { 
            knockbackReceiver.onStartKnockback += OnStartKnockback;
            knockbackReceiver.onEndKnockback += OnEndKnockback;
        }

        if (reincarnation != null)
            reincarnation.onReincarnation -= OnReincarnation;
        
        if (idleCombat != null)
            idleCombat.onNormalAttackBeingHit -= OnNormalAttackBeingHit;
        
        if (block != null)
            block.onBlockFailed -= OnBlockFailed;
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
        //combatEventsManager.RaiseOnBeenHit();
        // TODO - remove after testing
        Debug.Log(gameObject.name + " took " + damage + " damage ");
        
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
            //PlayerController playerController = this.GetComponent<PlayerController>();
           // Dash dash = this.GetComponent<Dash>();

            //playerController.enabled = true;
            //dash.enabled = true;
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
            //PlayerController playerController = this.GetComponent<PlayerController>();
            //Dash dash = this.GetComponent<Dash>();

            //playerController.enabled = false;
            //dash.enabled = false;
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

    // TODO - Refactor death
    private void ManageDeath()
    {
        if (!IsDying) { 
            IsDying = true;

            // If an ally is dying...
            if (ThisUnitType == Stats.Type.Ally)
            {
                // TODO - Manage this inside DemonMovement with OnDeath event
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
            }

            // TODO - Manage player death with OnDeath event
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

            // TODO - Manage Boss death with OnDeath event
            // if the boss is dying...
            if (ThisUnitType == Type.Boss)
            {
                // Update EnemiesManager boss
                EnemiesManager.Instance.BossKilled();
            }

            // TODO - Remove LittleEnemy logic
            // if a littleEnemy is dying...
            if (ThisUnitType == Type.Enemy)
            {
                // Update EnemiesManager littleEnemiesList
                EnemiesManager.Instance.LittleEnemyKilled(this.gameObject);
            }

            RaiseOnDeath();
            RaiseOnLateDeath();
 
            // TODO - disable components for death duration
            deathCR = StartCoroutine(DeathTimer(deathDuration));
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

    private void OnStartKnockback(KnockbackReceiver sender)
    {
        DisableMovement();
    }

    private void OnEndKnockback(KnockbackReceiver sender)
    {
        EnableMovement();
    }

    private void OnBlockFailed(Block sender, NormalAttack normalAttack,NormalCombat attackerNormalCombat)
    {
        // TODO - AlliesList.Count is not #attackingUnits, take in account other orders too
        if(normalAttack.HasDamageSupportBonus && thisUnitType == Type.Boss)
            TakeHit(normalAttack.Damage + Support.SupportingUnits * (3.5f / (AlliesManager.Instance.AlliesList.Count - Support.SupportingUnits - Recruit.RecruitingUnits)));
        else
            TakeHit(normalAttack.Damage);
    }

    private void OnNormalAttackBeingHit(IdleCombat sender, NormalAttack normalAttack, NormalCombat attackerNormalCombat)
    {
        // TODO - AlliesList.Count is not #attackingUnits, take in account other orders too
        if(normalAttack.HasDamageSupportBonus && thisUnitType == Type.Boss)
            TakeHit(normalAttack.Damage + Support.SupportingUnits * (3.5f / (AlliesManager.Instance.AlliesList.Count - Support.SupportingUnits - Recruit.RecruitingUnits)));
        else
            TakeHit(normalAttack.Damage);
    }

    private void OnReincarnation(GameObject player) 
    { 
        SetAsPlayer();
        SetPlayerBaseHP();
    }

    #endregion
    
    #region Coroutines
    
    private IEnumerator DeathTimer(float s)
    {
        yield return new WaitForSeconds(s);
        Destroy(gameObject);
    }

    #endregion
}
