using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupBehaviour : MonoBehaviour {
    /// <summary>
    /// Enum that lists the aviable groups
    /// </summary>
    public enum Group {
        GroupAzure,
        GroupPink,
        GroupGreen,
        GroupYellow,
        None
    }

    [SerializeField]
    [Tooltip("Field that indicate wich group this is")]
    private Group thisGroupName = Group.None;

    /// <summary>
    /// Property that idicates wich group this is
    /// </summary>
    public Group ThisGroupName { get => thisGroupName; private set => thisGroupName = value; }

    /// This script is attached to invisible gameobjects that manages the single group

    public int maxNumDemons = 4;
    private int demonsInGroup = 0;
    public float rateo = 2f;
    public Material groupColor;
    private Coroutine continuousAttack;
    public GameObject healthBar;
    private GroupHealthBar groupHealthBar;

    #region 

    // Useful to distinguish between States in game as Tactics for group and FSMState (e.g. Idle not present here since it's not in game)
    public enum State {
        MeleeAttack,
        Tank,
        RangeAttack,
        Support,
        Recruit
    }

    // The time after the next update of the FSM
    public float fsmReactionTime = 1f;
    public State currentState;
    public State newState;
    public bool orderConfirmed = false;
    public GameObject[] demons;

    public FSM groupFSM;
    // Used to know if the group is in combat or not (don't want to add a state in State enum cause it's simpler this way)
    private bool inCombat = false;
    FSMState meleeState, tankState, rangeAttackState, supportState, recruitState, idleState;
    private GameObject target;

    public GameObject Target { get => target; set => target = value; }
    public int DemonsInGroup { get => demonsInGroup; set => demonsInGroup = value; }

    #region Conditions

    public bool MeleeOrderGiven() {
        if((newState != currentState) && (orderConfirmed) && (newState == State.MeleeAttack)) {
            return true;
        }
        return false;
    }

    public bool TankOrderGiven() {
        if((newState.ToString() != groupFSM.current.stateName) && (orderConfirmed) && (newState == State.Tank)) {
            return true;
        }
        return false;
    }

    public bool RangeAttackOrderGiven() {
        if((newState.ToString() != groupFSM.current.stateName) && (orderConfirmed) && (newState == State.RangeAttack)) {
            return true;
        }
        return false;
    }

    public bool SupportOrderGiven() {
        if((newState != currentState) && (orderConfirmed) && (newState == State.Support)) {
            return true;
        }
        return false;
    }

    public bool RecruitOrderGiven() {
        if((newState != currentState) && (orderConfirmed) && (newState == State.Recruit)) {
            return true;
        }
        return false;
    }

    public bool Idle() {
        if(!inCombat)
            return true;
        return false;
    }

    public bool EnterCombat() {
        if(!Idle())
            return true;
        return false;
    }

    private void ConfirmEffects() {
        foreach(GameObject demon in demons) {
            if(demon)
                demon.GetComponent<SelectedUnitEffects>().ConfirmOrder();
        }
    }

    #endregion

    #region Actions

    public void GeneralEnterAction() {
        currentState = newState;
        ConfirmEffects();
        orderConfirmed = false;

        GroupAggro groupAggro = gameObject.GetComponent<GroupAggro>();
        groupAggro.UpdateGroupAggro();

        if(currentState == State.Tank)
            groupAggro.GroupAggroValue = Mathf.Max(Mathf.CeilToInt((groupAggro.CalculateAverageAggro() / groupAggro.groups.Length) * groupAggro.TankMultiplier), groupAggro.GroupAggroValue);
        else if(currentState == State.MeleeAttack || currentState == State.RangeAttack)
            continuousAttack = StartCoroutine(AttackOneTime(rateo));

        GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>().RaiseAggro(groupAggro.OrderGivenMultiplier);
    }

    public void MeleeAttack() {
        if(!CheckDemons())
            return;

        //GameObject[] enemies = GameObject.FindGameObjectsWithTag( "LittleEnemy" );
        //GameObject boss = GameObject.FindGameObjectWithTag( "Boss" );

        //if ( boss )
        if(EnemiesManager.Instance.Boss != null) {
            //target = boss;
            target = EnemiesManager.Instance.Boss;
        }
        //else if ( enemies != null )
        else if(EnemiesManager.Instance.LittleEnemiesList.Count != 0) {
            //target = CameraManager.FindNearestEnemy(gameObject, enemies);
            target = CameraManager.FindNearestEnemy(gameObject, EnemiesManager.Instance.LittleEnemiesList.ToArray());
        }
        else
            return;

        foreach(GameObject demon in demons) {
            if(demon) {
                //Combat combat = demon.GetComponent<Combat>();

                //GameObject[] enemies = GameObject.FindGameObjectsWithTag( "LittleEnemy" );
                //GameObject boss = GameObject.FindGameObjectWithTag( "Boss" );

                //if ( boss )
                //    target = boss;
                //else if ( enemies != null )
                //    target = CameraManager.FindNearestEnemy( gameObject, enemies );
                //else
                //    return;

                // TODO - Ally keep attacking when out of combat,
                // I added this check to remove errors but need to 
                // be fixed
                //if (combat.enabled)
                if(demon.GetComponent<DemonMovement>().HorizDistFromTargetBorders(target) < 1.5f) {
                    StartCoroutine(ActionAfterRandomDelay(demon, State.MeleeAttack));
                    //combat.SingleAttack(target);
                }

            }
        }
    }

    IEnumerator ActionAfterRandomDelay(GameObject demon, State action) {

        Combat combat = demon.GetComponent<Combat>();

        if(combat.enabled && demon.GetComponent<DemonMovement>().CanAct()) {

            switch(action) {
                case State.MeleeAttack:
                    combat.SingleAttack(target);
                    break;
                case State.RangeAttack:
                    combat.RangedAttack(target);
                    break;
                default:
                    break;

            }

            float randomDelay = Random.Range(0f, 0.5f);
            yield return new WaitForSeconds(randomDelay);
        }

    }

    public void StopAttack() {
        if(!CheckDemons())
            return;
        foreach(GameObject demon in demons) {
            if(demon) {
                Combat combat = demon.GetComponent<Combat>();
                combat.StopAttack();
            }
        }
        StopCoroutine(continuousAttack);
    }

    public void TankStayAction() {
        if(!CheckDemons())
            return;
        foreach(GameObject demon in demons) {
            if(demon) {
                Combat combat = demon.GetComponent<Combat>();

                if(BattleEventsHandler.IsInBossBattle || BattleEventsHandler.IsInRegularBattle) {
                    combat.StartBlock();
                }
                else {
                    combat.StopBlock();
                }
            }
        }
    }

    public void StopTank() {
        if(!CheckDemons())
            return;
        foreach(GameObject demon in demons) {
            if(demon) {
                Combat combat = demon.GetComponent<Combat>();
                combat.StopBlock();
            }
        }
    }

    public void RangeAttack() {
        if(!CheckDemons())
            return;

        //GameObject[] enemies = GameObject.FindGameObjectsWithTag( "LittleEnemy" );
        //GameObject boss = GameObject.FindGameObjectWithTag( "Boss" );

        //if ( boss )
        if(EnemiesManager.Instance.Boss != null) {
            //target = boss;
            target = EnemiesManager.Instance.Boss;
        }
        //else if ( enemies != null )
        else if(EnemiesManager.Instance.LittleEnemiesList.Count != 0) {
            //target = CameraManager.FindNearestEnemy(gameObject, enemies);
            target = CameraManager.FindNearestEnemy(gameObject, EnemiesManager.Instance.LittleEnemiesList.ToArray());
        }
        else
            return;


        foreach(GameObject demon in demons) {
            if(demon) {
                //Combat combat = demon.GetComponent<Combat>();

                // TODO - Ally keep attacking when out of combat,
                // I added this check to remove errors but need to 
                // be fixed
                //if(combat.enabled) {
                StartCoroutine(ActionAfterRandomDelay(demon, State.RangeAttack));
                //combat.RangedAttack(target);
                //}
            }
        }
    }

    public void StopRangeAttack() {
        if(!CheckDemons())
            return;

        foreach(GameObject demon in demons) {
            if(demon) {
                Combat combat = demon.GetComponent<Combat>();
                combat.StopRangedAttack();
            }
        }

        StopCoroutine(continuousAttack);
    }

    public void SupportStayAction() {
        if(!CheckDemons())
            return;
        foreach(GameObject demon in demons) {
            if(demon) {
                Combat combat = demon.GetComponent<Combat>();
                if(BattleEventsHandler.IsInBossBattle || BattleEventsHandler.IsInRegularBattle) {
                    combat.StartSupport();
                }
                else {
                    combat.StopSupport();
                }
            }
        }
    }

    public void StopSupport() {
        if(!CheckDemons())
            return;

        foreach(GameObject demon in demons) {
            if(demon) {
                Combat combat = demon.GetComponent<Combat>();
                combat.StopSupport();
            }
        }
    }

    public void RecruitStayAction() {
        if(!CheckDemons())
            return;
        foreach(GameObject demon in demons) {
            if(demon) {
                Combat combat = demon.GetComponent<Combat>();
                if(BattleEventsHandler.IsInBossBattle || BattleEventsHandler.IsInRegularBattle) {
                    // TODO
                    //combat.StartRecruit();
                }
                else {
                    // TODO
                    //combat.StopRecruit();
                }
            }
        }
    }

    public void StopRecruit() {
        if(!CheckDemons())
            return;

        foreach(GameObject demon in demons) {
            if(demon) {
                Combat combat = demon.GetComponent<Combat>();
                // TODO
                //combat.StopRecruit();
            }
        }
    }

    // TODO - Parametrize this
    public void UpdateSupportAggro() {
        foreach(GameObject demon in demons) {
            if(demon)
                demon.GetComponent<Stats>().Aggro *= 1.08f;
        }
    }

    #endregion

    #endregion

    //TODO To know if all demons found their group (can be improved by just setting a single boolean in a single gameobjact, without checking for all demons)
    public bool CheckDemons() {
        //GameObject[] allDemons = GameObject.FindGameObjectsWithTag( "Demon" );

        //foreach ( GameObject go in allDemons )
        foreach(GameObject go in AlliesManager.Instance.AlliesList) {
            if(!go.GetComponent<DemonBehaviour>().groupFound)
                return false;
        }
        return true;
    }

    public FSMState GetCurrentFSMState(State state) {
        switch(state) {
            case State.MeleeAttack:
                return meleeState;
            case State.Tank:
                return tankState;
            case State.RangeAttack:
                return rangeAttackState;
            case State.Support:
                return supportState;
            case State.Recruit:
                return recruitState;
        }
        return null;
    }

    // The coroutine that cycles through the FSM
    public IEnumerator MoveThroughFSM() {
        while(true) {
            yield return new WaitForSeconds(fsmReactionTime);
            //Debug.Log( groupFSM.current.stateName );
            groupFSM.Update();
        }
    }

    private void Awake() {
        groupHealthBar = healthBar.GetComponent<GroupHealthBar>();
    }

    void Start() {
        currentState = State.MeleeAttack;
        newState = State.MeleeAttack;
        // Just to test
        inCombat = true;

        demons = new GameObject[maxNumDemons];

        #region FSM

        FSMTransition t1 = new FSMTransition(MeleeOrderGiven);
        FSMTransition t2 = new FSMTransition(TankOrderGiven);
        FSMTransition t3 = new FSMTransition(RangeAttackOrderGiven);
        FSMTransition t4 = new FSMTransition(SupportOrderGiven);
        FSMTransition t5 = new FSMTransition(Idle);
        FSMTransition t6 = new FSMTransition(EnterCombat);
        FSMTransition t7 = new FSMTransition(RecruitOrderGiven);

        meleeState = new FSMState(State.MeleeAttack.ToString());
        tankState = new FSMState(State.Tank.ToString());
        rangeAttackState = new FSMState(State.RangeAttack.ToString());
        supportState = new FSMState(State.Support.ToString());
        recruitState = new FSMState(State.Recruit.ToString());
        idleState = new FSMState();

        meleeState.enterActions.Add(GeneralEnterAction);
        //meleeState.stayActions.Add( MeleeAttack );
        meleeState.exitActions.Add(StopAttack);

        rangeAttackState.enterActions.Add(GeneralEnterAction);
        //rangeAttackState.stayActions.Add( RangeAttack );
        rangeAttackState.exitActions.Add(StopRangeAttack);

        tankState.enterActions.Add(GeneralEnterAction);
        tankState.enterActions.Add(TankStayAction);
        tankState.stayActions.Add(TankStayAction);
        tankState.exitActions.Add(StopTank);

        supportState.enterActions.Add(GeneralEnterAction);
        supportState.stayActions.Add(SupportStayAction);
        supportState.stayActions.Add(UpdateSupportAggro);
        supportState.exitActions.Add(StopSupport);

        recruitState.enterActions.Add(GeneralEnterAction);
        recruitState.stayActions.Add(RecruitStayAction);
        recruitState.exitActions.Add(StopRecruit);

        meleeState.AddTransition(t2, tankState);
        meleeState.AddTransition(t3, rangeAttackState);
        meleeState.AddTransition(t4, supportState);
        meleeState.AddTransition(t5, idleState);
        meleeState.AddTransition(t7, recruitState);

        tankState.AddTransition(t1, meleeState);
        tankState.AddTransition(t3, rangeAttackState);
        tankState.AddTransition(t4, supportState);
        tankState.AddTransition(t5, idleState);
        tankState.AddTransition(t7, recruitState);

        rangeAttackState.AddTransition(t1, meleeState);
        rangeAttackState.AddTransition(t2, tankState);
        rangeAttackState.AddTransition(t4, supportState);
        rangeAttackState.AddTransition(t5, idleState);
        rangeAttackState.AddTransition(t7, recruitState);

        supportState.AddTransition(t1, meleeState);
        supportState.AddTransition(t2, tankState);
        supportState.AddTransition(t3, rangeAttackState);
        supportState.AddTransition(t5, idleState);
        supportState.AddTransition(t7, recruitState);

        recruitState.AddTransition(t1, meleeState);
        recruitState.AddTransition(t2, tankState);
        recruitState.AddTransition(t3, rangeAttackState);
        recruitState.AddTransition(t4, supportState);
        recruitState.AddTransition(t5, idleState);

        idleState.AddTransition(t6, GetCurrentFSMState(currentState));

        //groupFSM = new FSM( idleState );
        groupFSM = new FSM(meleeState);
        StartCoroutine(MoveThroughFSM());

        #endregion
    }

    public bool IsEmpty() {
        foreach(GameObject demon in demons) {
            if(demon != null)
                return false;
        }
        return true;
    }

    public int GetDemonsNumber() {
        return DemonsInGroup;
    }

    public void SetDemonsNumber(int i) {
        DemonsInGroup = i;
        groupHealthBar.SetDemonsNumber(i);
    }

    //TODO to be improved
    public GameObject GetRandomDemon() {
        GameObject demon = null;

        bool found = false;
        while(!found) {
            int index = Random.Range(0, demons.Length);
            if(demons[index] != null) {
                demon = demons[index];
                found = true;
            }
        }

        return demon;
    }

    public IEnumerator AttackOneTime(float rateo) {
        while(true) {
            yield return new WaitForSeconds(rateo);
            if(currentState == State.MeleeAttack)
                MeleeAttack();
            else if(currentState == State.RangeAttack)
                RangeAttack();
        }
    }
}
