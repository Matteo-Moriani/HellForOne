using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpMana : MonoBehaviour
{
    #region Fields
    
    private static float maxMana = 100;
    private static float baseManaChargeRate = 1f;
    private static float activeManaChargeRate = 2f;
    private static float rechargeTimer = 0f;
    private static float currentManaRechargeRate;
    private static float manaPool = 45f;
    private static List<ParticleSystem> _manaParticles = new List<ParticleSystem>();

    private static GroupAbilities[] groupAbilitiesArray;
    
    private Coroutine manaRechargeCr = null;
    
    #endregion

    #region Properties

    public static float ManaPool
    {
        get => manaPool;
        private set => manaPool = value;
    }

    public static float MaxMana
    {
        get => maxMana;
        private set => maxMana = value;
    }

    #endregion
    
    #region Delegates and events

    public delegate void OnManaPoolChanged();
    public static event OnManaPoolChanged onManaPoolChanged;

    public static event Action OnOneSegment;
    public static event Action OnTwoSegments;

    // TODO Mancano gli eventi per il consumo delle barre
    
    private void RaiseOnManaPoolChanged()
    {
        onManaPoolChanged?.Invoke();
    }
    
    #endregion
    
    #region Unity methods

    private void Awake()
    {
        ParticleSystem[] allParticles = GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem p in allParticles)
        {
            if(p.gameObject.name == "ManaParticles")
                _manaParticles.Add(p);
        }

        if (manaRechargeCr == null)
        {
            manaRechargeCr = StartCoroutine(ManaRechargeCoroutine());
        }

        currentManaRechargeRate = baseManaChargeRate;

        groupAbilitiesArray = new GroupAbilities[4];
        int i = 0;
        foreach (GameObject group in GroupsManager.Instance.Groups)
        {
            groupAbilitiesArray[i] = group.GetComponentInChildren<GroupAbilities>();
            i++;
        }
    }

    private void OnEnable()
    {
        GetComponent<Stats>().onDeath += OnDeath;

        foreach (GroupAbilities groupAbilities in groupAbilitiesArray)
        {
            groupAbilities.onStartAbility += OnStartAbility;
        }
    }

    private void OnDisable()
    {
        GetComponent<Stats>().onDeath -= OnDeath;
        
        foreach (GroupAbilities groupAbilities in groupAbilitiesArray)
        {
            groupAbilities.onStartAbility -= OnStartAbility;
        }
    }

    private static void OnStartAbility(AbilityAttack startedability)
    {
        manaPool -= startedability.ManaCost;
    }

    #endregion
    
    #region Methods

    public void StartActiveManaRecharge()
    {
        currentManaRechargeRate = activeManaChargeRate;
    }

    public void StopActiveManaRecharge()
    {
        currentManaRechargeRate = baseManaChargeRate;
    }

    #endregion

    #region External events handlers
    
    private void OnDeath(Stats sender)
    {
        StopAllCoroutines();
        this.enabled = false;
    }
    
    #endregion
    
    #region Coroutines

    private IEnumerator ManaRechargeCoroutine()
    {
        while (true)
        {
            rechargeTimer += Time.deltaTime;
            
            if (rechargeTimer >= 1.0f)
            {
                rechargeTimer = 0f;
                
                if (manaPool < maxMana)
                {
                    manaPool += currentManaRechargeRate;
                    RaiseOnManaPoolChanged();
                }

                if(manaPool == maxMana / 2f)
                {
                    OnOneSegment?.Invoke();
                    PlayManaParticles();

                }
                else if(manaPool == maxMana)
                {
                    OnTwoSegments?.Invoke();
                    PlayManaParticles();
                }
            }

            yield return null;
        }
    }

    private void PlayManaParticles()
    {
        foreach(ParticleSystem p in _manaParticles)
        {
            p.Play();
        }
    }

    #endregion

}
