using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupAbilities : MonoBehaviour
{
    #region fields

    [SerializeField]
    private AbilityAttack meleeAbility;
    [SerializeField]
    private AbilityAttack rangedAbility;
    [SerializeField]
    private AbilityAttack tankAbility;
    [SerializeField]
    private AbilityAttack recruitAbility;
    
    private bool isPerformingAbility = false;
    
    private Dictionary<GroupBehaviour.State, AbilityAttack> abilitiesDictionary = new Dictionary<GroupBehaviour.State, AbilityAttack>();

    private NormalCombat normalCombat;
    private GroupBehaviour groupBehaviour;
    private GroupManager groupManager;

    private Transform parent;

    private Coroutine abilityCr = null;
    
    #endregion

    #region Delegates and events

    public delegate void OnStartAbility(AbilityAttack startedAbility);
    public event OnStartAbility onStartAbility;
    
    public delegate void OnStopAbility(AbilityAttack stoppedAbility);
    public event OnStopAbility onStopAbility;

    #region Methods

    private void RaiseOnStartAbility(AbilityAttack startedAbility)
    {
        onStartAbility?.Invoke(startedAbility);
    }

    private void RaiseOnStopAbility(AbilityAttack stoppedAbility)
    {
        onStopAbility?.Invoke(stoppedAbility);
    }
    
    #endregion
    
    #endregion
    
    #region Unity methods

    private void Awake()
    {
        abilitiesDictionary[GroupBehaviour.State.MeleeAttack] = meleeAbility;
        abilitiesDictionary[GroupBehaviour.State.RangeAttack] = rangedAbility;
        abilitiesDictionary[GroupBehaviour.State.Recruit] = recruitAbility;
        abilitiesDictionary[GroupBehaviour.State.Tank] = tankAbility;

        normalCombat = GetComponent<NormalCombat>();

        groupBehaviour = transform.root.GetComponent<GroupBehaviour>();

        groupManager = transform.root.GetComponent<GroupManager>();

        parent = transform.parent;
    }

    private void OnEnable()
    {
        normalCombat.onStopAttack += OnStopAttack;
        PlayerInput.onXButtonDown += OnXButtonDown;
    }

    private void OnDisable()
    {
        normalCombat.onStopAttack -= OnStopAttack;
        PlayerInput.onXButtonDown -= OnXButtonDown;
    }

    private void Update()
    {
        SetPosition();
    }

    #endregion
    
    #region Methods

    private void SetPosition()
    {
        if (!isPerformingAbility)
        {
            Vector3 accumulationVector = Vector3.zero;

            foreach (GameObject imp in groupManager.Imps)
            {
                if (imp != null)
                {
                    accumulationVector += imp.transform.position;
                }
            }

            accumulationVector /= groupManager.ImpsInGroupNumber;

            transform.position = accumulationVector;

            // We want this object closer to the target
            // TODO - parametrize or find better solution
            //if (groupBehaviour.Target)
            //{
            //    transform.position += (groupBehaviour.Target.transform.position - transform.position).normalized * 0.8f;    
            //}
        }
        
        if(groupBehaviour.Target)
            transform.LookAt(groupBehaviour.Target.transform);
    }

    private void StartAbility()
    {
        if (!isPerformingAbility && GroupsInRangeDetector.MostRappresentedGroupInRange == groupManager.ThisGroupName && abilityCr == null)
        {
            isPerformingAbility = true;

            AbilityAttack abilityToStart = abilitiesDictionary[groupBehaviour.currentState];
            
            if (ImpMana.ManaPool >= abilityToStart.ManaCost)
            {
                if (abilityToStart.IsOffensive)
                {
                    if(abilityToStart.IsRanged)
                        normalCombat.StartAttackRanged(abilityToStart,groupBehaviour.Target);
                    else
                    {
                        normalCombat.StartAttack(abilityToStart);    
                    }    
                }
                
                transform.SetParent(null);

                abilityCr = StartCoroutine(AbilityCoroutine(abilityToStart));
                
                RaiseOnStartAbility(abilityToStart);   
            }
            else
            {
                StopAbility(abilityToStart);
            }
        }
    }

    private void StopAbility(AbilityAttack abilityToStop)
    {
        if (isPerformingAbility)
        {
            isPerformingAbility = false;
            
            transform.SetParent(parent);

            abilityCr = null;
            
            RaiseOnStopAbility(abilityToStop);
        }
    }

    #endregion

    #region External events handlers

    // TODO - Check if this can be removed
    private void OnStopAttack(NormalCombat sender, GenericAttack genericAttack)
    {
        //if(attack.GetType() == typeof(AbilityAttack))
        //    StopAbility((AbilityAttack)attack);
    }
    
    private void OnXButtonDown()
    {
        StartAbility();
    }
    
    #endregion

    #region Coroutines

    private IEnumerator AbilityCoroutine(AbilityAttack ability)
    {
        yield return new WaitForSeconds(ability.DelayInSeconds + ability.DurationInSeconds);
        
        StopAbility(ability);
    }

    #endregion
}
