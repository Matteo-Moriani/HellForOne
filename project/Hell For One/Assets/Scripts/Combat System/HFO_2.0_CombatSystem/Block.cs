using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Random = UnityEngine.Random;

// If we need to check player's angle before blocking
// Maybe we can check only x and z components
/*
private bool CheckAngle(Transform other)
{
    return Vector3.Angle(this.transform.root.transform.forward, other.forward) < 91;
}
*/

// TODO - We need to manage player's death? or OnDisable will do the job?
public class Block : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private BlockValues blockValues;
    
    private Stats.Type unitType;
    private IdleCombat idleCombat;
    private GroupFinder groupFinder;
    private GroupBehaviour groupBehaviour;
    private Reincarnation reincarnation;

    // TODO - delete serializefield after testing
    [SerializeField]
    private int blockChance = 0;
    
    #endregion
    
    // TODO - Stuff missing
    #region Delegates and events

    public delegate void OnStartBlock(Block sender);
    public event OnStartBlock onStartBlock;

    public delegate void OnStopBlock(Block sender);
    public event OnStopBlock onStopBlock;
    
    public delegate void OnBlockSuccess(Block sender, NormalAttack normalAttack, NormalCombat attackerNormalCombat);
    public event OnBlockSuccess onBlockSuccess;

    public delegate void OnBlockFailed(Block sender, NormalAttack normalAttack, NormalCombat attackerNormalCombat);
    public event OnBlockFailed onBlockFailed;

    // TODO - Stuff missing
    #region Methods

    // TODO - implement for player
    private void RiseOnStartBlock()
    {
        onStartBlock?.Invoke(this);
    }

    // TODO - implement for player
    private void RiseOnStopBlock()
    {
        onStopBlock?.Invoke(this);
    }
    
    // TODO - register in knockback, stun, sound, animation
    private void RiseOnBlockFailed(NormalAttack normalAttack, NormalCombat attackerNormalCombat)
    {
        onBlockFailed?.Invoke(this, normalAttack, attackerNormalCombat);
    }
    
    // TODO - register in stats(?), knockback, stun, sound, animation
    private void RiseOnBlockSuccess(NormalAttack normalAttack, NormalCombat attackerNormalCombat)
    {
        onBlockSuccess?.Invoke(this, normalAttack, attackerNormalCombat);
    }

    #endregion
    
    #endregion

    #region Unity methods

    private void Awake()
    {
        idleCombat = gameObject.GetComponent<IdleCombat>();
        groupFinder = transform.root.gameObject.GetComponent<GroupFinder>();
        unitType = transform.root.GetComponent<Stats>().ThisUnitType;
        reincarnation = transform.root.GetComponent<Reincarnation>();
    }
    
    private void OnEnable()
    {
        idleCombat.onNormalAttackTry += OnNormalAttackTry;

        if (groupFinder != null)
            groupFinder.onGroupFound += OnGroupFound;
        
        transform.root.GetComponent<Stats>().onDeath += OnDeath;
        
        if(reincarnation != null)
            reincarnation.onLateReincarnation += OnLateReincarnation;
    }

    private void OnDisable()
    {
        idleCombat.onNormalAttackTry -= OnNormalAttackTry;
        
        if (groupFinder != null)
            groupFinder.onGroupFound -= OnGroupFound;

        transform.root.GetComponent<Stats>().onDeath -= OnDeath;
        
        if(reincarnation != null)
            reincarnation.onLateReincarnation -= OnLateReincarnation;
    }

    #endregion

    #region Methods

    public void StartBlock()
    {
        if (unitType == Stats.Type.Player)
            blockChance = 100;
        
        RiseOnStartBlock();
    }

    public void StopBlock()
    {
        if (unitType == Stats.Type.Player)
            blockChance = 0;
        
        RiseOnStopBlock();
    }

    #endregion
    
    #region External events handlers

    private void OnLateReincarnation(GameObject newPlayer)
    {
        Stats newPlayerStats = newPlayer.GetComponent<Stats>();
        unitType = newPlayerStats.ThisUnitType;
        StopBlock();
    }

    private void OnStartTankOrderGiven(GroupBehaviour sender)
    {
        StartBlock();
    }

    private void OnStopTankOrderGiven(GroupBehaviour sender)
    {
        StopBlock();
    }

    private void OnGroupFound(GroupFinder sender)
    {
        groupBehaviour = sender.GroupBelongingTo.GetComponent<GroupBehaviour>();

        if (groupBehaviour != null)
        {
            groupBehaviour.onOrderChanged += OnOrderChanged;
            groupBehaviour.onStartTankOrderGiven += OnStartTankOrderGiven;
            groupBehaviour.onStopTankOrderGiven += OnStopTankOrderGiven;
            
            // Initialize this imp block chance
            OnOrderChanged(groupBehaviour, groupBehaviour.currentState);
        }
    }

    private void OnOrderChanged(GroupBehaviour sender, GroupBehaviour.State newState)
    {
        switch (newState)
        {
            case GroupBehaviour.State.MeleeAttack:
                blockChance =blockValues.MeleeBlockChance;
                break;
            case GroupBehaviour.State.Tank:
                blockChance = blockValues.TankBlockChance;
                break;
            case GroupBehaviour.State.RangeAttack:
                blockChance = blockValues.RangedBlockChance;
                break;
            case GroupBehaviour.State.Support:
                blockChance = blockValues.SupportBlockChance;
                break;
            case GroupBehaviour.State.Recruit:
                blockChance = blockValues.RecruitBlockChance;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void OnNormalAttackTry(IdleCombat sender, NormalAttack normalAttack, NormalCombat attackerNormalCombat)
    {
        if (Random.Range(0, 100) < blockChance)
        {
            RiseOnBlockSuccess(normalAttack,attackerNormalCombat);
        }
        else
        {    
            RiseOnBlockFailed(normalAttack, attackerNormalCombat);
        }
    }

    private void OnDeath(Stats sender)
    {
        if (groupBehaviour != null)
        {    
            groupBehaviour.onStartTankOrderGiven -= OnStartTankOrderGiven;
            groupBehaviour.onStopTankOrderGiven -= OnStopTankOrderGiven;
        
            groupBehaviour.onOrderChanged -= OnOrderChanged;
        }
    }

    #endregion
}

[CreateAssetMenu(fileName = "BlockValues", menuName = "CombatSystem/BlockValues", order = 1)]
public class BlockValues : ScriptableObject
{
    [SerializeField]
    private int meleeBlockChance = 0;

    [SerializeField] 
    private int rangedBlockChance = 0;
    
    [SerializeField]
    private int tankBlockChance = 0;
    
    [SerializeField]
    private int supportBlockChance = 0;
    
    [SerializeField]
    private int recruitBlockChance = 0;

    public int MeleeBlockChance
    {
        get => meleeBlockChance;
        private set => meleeBlockChance = value;
    }

    public int RangedBlockChance
    {
        get => rangedBlockChance;
        private set => rangedBlockChance = value;
    }

    public int TankBlockChance
    {
        get => tankBlockChance;
        private set => tankBlockChance = value;
    }

    public int SupportBlockChance
    {
        get => supportBlockChance;
        private set => supportBlockChance = value;
    }

    public int RecruitBlockChance
    {
        get => recruitBlockChance;
        private set => recruitBlockChance = value;
    }
}
