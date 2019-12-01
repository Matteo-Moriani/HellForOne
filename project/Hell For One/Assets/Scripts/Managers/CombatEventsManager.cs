using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEventsManager : MonoBehaviour
{
    #region CombatEvents fields

    public delegate void OnMeleeAttack();
    public event OnMeleeAttack onMeleeAttack;

    public delegate void OnStopMeleeAttack();
    public event OnStopMeleeAttack onStopMeleeAttack;

    public delegate void OnRangedAttack();
    public event OnRangedAttack onRangedAttack;

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

    public delegate void OnStartSweep();
    public event OnStartSweep onStartSweep;

    public delegate void OnStopSweep();
    public event OnStopSweep onStopSweep;

    public delegate void OnStartGlobalAttack();
    public event OnStartGlobalAttack onStartGlobalAttack;

    public delegate void OnStopGlobalattack();
    public event OnStopGlobalattack onStopGlobalAttack;

    public delegate void OnSuccessfulHit();
    public event OnSuccessfulHit onSuccessfulHit;

    public delegate void OnBlockedHit();
    public event OnBlockedHit onBlockedHit;

    public delegate void OnDeath();
    public event OnDeath onDeath;

    #endregion

    #region CombatEventFields

    public void RaiseOnMeleeAttack()
    {
        if (onMeleeAttack != null)
        {
            onMeleeAttack();
        }
    }

    public void RaiseOnStopMeleeAttack()
    {
        if (onStopMeleeAttack != null)
        {
            onStopMeleeAttack();
        }
    }

    public void RaiseOnRangedAttack()
    {
        if (onRangedAttack != null)
        {
            onRangedAttack();
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

    public void RaiseOnStartSweep()
    {
        if (onStartSweep != null)
        {
            onStartSweep();
        }
    }

    public void RaiseOnStopSweep()
    {
        if (onStopSweep != null)
        {
            onStopSweep();
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

    public void RaiseOnDeath()
    {
        if (onDeath != null)
        {
            onDeath();
        }
    }

    #endregion
}
