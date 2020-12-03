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
    private static float manaPool = 45f;            // public and 45 to debug faster
    private static List<ParticleSystem> _manaParticles = new List<ParticleSystem>();
    private static bool inBattle = false;

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

    public static event Action OnManaPoolChanged;
    public static event Action OnOneSegment;
    public static event Action OnTwoSegments;

    // TODO Mancano gli eventi per il consumo delle barre
        
    #endregion
    
    #region Unity methods

    private void Awake()
    {
        _manaParticles.Clear();
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
        BattleEventsManager.onBattleEnter += OnBattleEnter;
        BattleEventsManager.onBattleExit += OnBattleExit;

        foreach (GroupAbilities groupAbilities in groupAbilitiesArray)
        {
            groupAbilities.onStartAbility += OnStartAbility;
        }
    }

    private void OnDisable()
    {
        GetComponent<Stats>().onDeath -= OnDeath;
        BattleEventsManager.onBattleEnter -= OnBattleEnter;
        BattleEventsManager.onBattleExit -= OnBattleExit;

        foreach (GroupAbilities groupAbilities in groupAbilitiesArray)
        {
            groupAbilities.onStartAbility -= OnStartAbility;
        }
    }

    private static void OnStartAbility(AbilityAttack startedAbility)
    {
        manaPool -= startedAbility.ManaCost;
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
        enabled = false;
    }
    
    #endregion
    
    #region Coroutines

    private static IEnumerator ManaRechargeCoroutine()
    {
        bool firstSegmentActivated = false;
        bool secondSegmentActivated = false;

        while (true)
        {
            if(inBattle)
            {
                rechargeTimer += Time.deltaTime;

                if(rechargeTimer >= 1.0f)
                {
                    rechargeTimer = 0f;

                    if(manaPool < maxMana / 2f)
                    {
                        manaPool += currentManaRechargeRate;
                        OnManaPoolChanged?.Invoke();
                        firstSegmentActivated = false;
                        secondSegmentActivated = false;
                        StopManaParticles();
                    }
                    else if(manaPool < maxMana)
                    {
                        manaPool += currentManaRechargeRate;
                        OnManaPoolChanged?.Invoke();
                        secondSegmentActivated = false;
                    }

                    if(manaPool >= maxMana / 2f && !firstSegmentActivated)
                    {
                        OnOneSegment?.Invoke();
                        PlayManaParticles();
                        firstSegmentActivated = true;
                    }
                    else if(manaPool == maxMana && !secondSegmentActivated)
                    {
                        OnTwoSegments?.Invoke();
                        PlayManaParticles();
                        secondSegmentActivated = true;
                    }
                }
            }

            yield return null;
        }
    }

    private static void PlayManaParticles()
    {
        foreach(ParticleSystem p in _manaParticles)
        {
            p.Play();
        }
    }

    private static void StopManaParticles()
    {
        foreach(ParticleSystem p in _manaParticles)
        {
            if(p.isPlaying)
                p.Stop();
        }
    }

    private void OnBattleEnter()
    {
        inBattle = true;
    }

    private void OnBattleExit()
    {
        inBattle = false;
        manaPool = 0f;
    }

    #endregion

}
