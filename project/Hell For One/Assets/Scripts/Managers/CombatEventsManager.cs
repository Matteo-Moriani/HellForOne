using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEventsManager : MonoBehaviour
{
    #region CombatEvents fields

    public delegate void OnStartSingleAttack();
    public event OnStartSingleAttack onStartSingleAttack;

    public delegate void OnStopSingleAttack();
    public event OnStopSingleAttack onStopSingleAttack;

    public delegate void OnStartRangedAttack();
    public event OnStartRangedAttack onStartRangedAttack;

    public delegate void OnStopRangedAttack();
    public event OnStopRangedAttack onStopRangedAttack;

    public delegate void OnStartBlock();
    public event OnStartBlock onStartBlock;

    public delegate void OnStopBlock();
    public event OnStopBlock onStopBlock;

    public delegate void OnStartSupport();
    public event OnStartSupport onStartSupport;

    public delegate void OnStopSupport();
    public event OnStopSupport onStopSupport;

    public delegate void OnStartRecruit();
    public event OnStartSupport onStartRecruit;

    public delegate void OnStopRecruit();
    public event OnStopSupport onStopRecruit;

    public delegate void OnStartGroupAttack();
    public event OnStartGroupAttack onStartGroupAttack;

    public delegate void OnStopGroupAttack();
    public event OnStopGroupAttack onStopGroupAttack;

    public delegate void OnStartGlobalAttack();
    public event OnStartGlobalAttack onStartGlobalAttack;

    public delegate void OnStopGlobalAttack();
    public event OnStopGlobalAttack onStopGlobalAttack;

    public delegate void OnSuccessfulHit();
    public event OnSuccessfulHit onSuccessfulHit;

    public delegate void OnBlockedHit();
    public event OnBlockedHit onBlockedHit;

    public delegate void OnBeenHit(Stats attackerStats);
    public event OnBeenHit onBeenHit;

    public delegate void OnDeath();
    public event OnDeath onDeath;

    public delegate void OnReincarnation();
    public event OnReincarnation onReincarnation;

    public delegate void OnStartMoving();
    public event OnStartMoving onStartMoving;

    public delegate void OnStartDash();
    public event OnStartDash onStartDash;

    public delegate void OnStartIdle();
    public event OnStartIdle onStartIdle;

    //public delegate void OnStopAnimation();
    //public event OnStopAnimation onStopAnimation;

    #endregion

    #region CombatEvents methods

    public void RaiseOnStartSingleAttack()
    {
        if (onStartSingleAttack != null)
        {
            onStartSingleAttack();
        }
    }

    public void RaiseOnStopSingleAttack()
    {
        if (onStopSingleAttack != null)
        {
            onStopSingleAttack();
        }
    }

    public void RaiseOnStartRangedAttack()
    {
        if (onStartRangedAttack != null)
        {
            onStartRangedAttack();
        }
    }

    public void RaiseOnStopRangedAttack()
    {
        if (onStopRangedAttack != null)
        {
            onStopRangedAttack();
        }
    }

    public void RaiseOnStartBlock()
    {
        if (onStartBlock != null)
        {
            onStartBlock();
        }
    }

    public void RaiseOnStopBlock()
    {
        if (onStopBlock != null)
        {
            onStopBlock();
        }
    }

    public void RaiseOnStartSupport()
    {
        if (onStartSupport != null)
        {
            onStartSupport();
        }
    }

    public void RaiseOnStopSupport()
    {
        if (onStopSupport != null)
        {
            onStopSupport();
        }
    }

    public void RaiseOnStartRecruit()
    {
        if (onStartRecruit != null )
        {
            onStartRecruit();
        }
    }

    public void RaiseOnStopRecruit()
    {
        if (onStopRecruit != null )
        {
            onStopRecruit();
        }
    }

    public void RaiseOnStartGroupAttack()
    {
        if (onStartGroupAttack != null)
        {
            onStartGroupAttack();
        }
    }

    public void RaiseOnStopGroupAttack()
    {
        if (onStopGroupAttack != null)
        {
            onStopGroupAttack();
        }
    }

    public void RaiseOnStartGlobalAttack()
    {
        if (onStartGlobalAttack != null)
        {
            onStartGlobalAttack();
        }
    }

    public void RaiseOnStopGlobalAttack()
    {
        if (onStopGlobalAttack != null)
        {
            onStopGlobalAttack();
        }
    }

    public void RaiseOnSuccessfulHit()
    {
        if (onSuccessfulHit != null)
        {
            onSuccessfulHit();
        }
    }

    public void RaiseOnBlockedHit()
    {
        if (onBlockedHit != null)
        {
            onBlockedHit();
        }
    }

    public void RaiseOnBeenHit(Stats attackerStats) { 
        if(onBeenHit != null) { 
            onBeenHit(attackerStats);    
        }    
    }

    public void RaiseOnDeath()
    {
        // We need to stop all animations here!

        if (onDeath != null)
        {
            onDeath();
        }
    }

    public void RaiseOnReincarnation() { 
        if(onReincarnation != null) { 
            onReincarnation();    
        }    
    }

    public void RaiseOnStartMoving()
    {
        if (onStartMoving != null)
        {
            onStartMoving();
        }
    }

    public void RaiseOnStartIdle()
    {
        if (onStartIdle != null)
        {
            onStartIdle();
        }
    }

    public void RaiseOnStartDash() { 
        if(onStartDash != null) { 
            onStartDash();    
        }
    }

    /*
    public void RaiseOnStopAnimation()
    {
        if (onStopAnimation != null)
        {
            onStopAnimation();
        }
    }
    */

    #endregion
}
