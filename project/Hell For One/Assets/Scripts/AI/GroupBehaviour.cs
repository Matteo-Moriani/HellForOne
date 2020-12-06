using System;
using System.Collections;
using AI.Imp;
using OrdersSystem;
using OrdersSystem.ScriptableObjects;
using UnityEngine;

namespace AI
{
    public class GroupBehaviour : MonoBehaviour
    {
    //     public float rateo = 2f;
    //     private Coroutine continuousAttack;
    //
    //     private bool impsAlreadyChecked = false;
    //     private bool isSupportOrderGiven = false;
    //     private bool isTankOrderGiven = false;
    //     private bool isRecruitOrderGiven = false;
    //
    //     private GroupManager groupManager;
    //
    //     // TODO - Stuff missing
    //     #region Delegates and events
    //
    //     // TODO - Rethink this, it works but it's not clean-------------------------------------------
    //
    //     // TODO - This is used to update imp aggro and give right block chance for orders
    //     public delegate void OnOrderChanged(GroupBehaviour sender, GroupBehaviour.State newState);
    //     public event OnOrderChanged onOrderChanged;
    //
    //     // TODO - This is used only to assign right aggro to Tank state
    //     // TODO - Do we need OrderGiven events for other orders too?
    //     public delegate void OnTankOrderGiven(GroupBehaviour sender);
    //     public event OnTankOrderGiven onTankOrderGiven;
    //
    //     //----------------------------------------------------------------------------------------------
    //
    //     // TODO - Refactor this using an abstract class for orders--------------
    //
    //     // Support
    //     public delegate void OnStartSupportOrderGiven(GroupBehaviour sender);
    //     public event OnStartSupportOrderGiven onStartSupportOrderGiven;
    //
    //     public delegate void OnStopSupportOrderGiven(GroupBehaviour sender);
    //     public event OnStopSupportOrderGiven onStopSupportOrderGiven;
    //
    //     // Tank
    //     public delegate void OnStartTankOrderGiven(GroupBehaviour sender);
    //     public event OnStartTankOrderGiven onStartTankOrderGiven;
    //
    //     public delegate void OnStopTankOrderGiven(GroupBehaviour sender);
    //     public event OnStopTankOrderGiven onStopTankOrderGiven;
    //
    //     // Recruit
    //     public delegate void OnStartRecruitOrderGiven(GroupBehaviour sender);
    //     public event OnStartRecruitOrderGiven onStartRecruitOrderGiven;
    //
    //     public delegate void OnStopRecruitOrderGiven(GroupBehaviour sender);
    //     public event OnStopRecruitOrderGiven onStopRecruitOrderGiven;
    //
    //     //-------------------------------------------------------------------------
    //
    //     public static event Action OnRealOrderChanged;
    //
    //     #region Methods
    //
    //     private void RaiseOnStopTankOrderGiven()
    //     {
    //         isTankOrderGiven = false;
    //         onStopTankOrderGiven?.Invoke(this);
    //     }
    //
    //     private void RaiseOnStartTankOrderGiven()
    //     {
    //         isTankOrderGiven = true;
    //         onStartTankOrderGiven?.Invoke(this);
    //     }
    //
    //     private void RaiseOnStopRecruitOrderGiven()
    //     {
    //         isRecruitOrderGiven = false;
    //         onStopRecruitOrderGiven?.Invoke(this);
    //     }
    //
    //     private void RaiseOnStartRecruitOrderGiven()
    //     {
    //         isRecruitOrderGiven = true;
    //         onStartRecruitOrderGiven?.Invoke(this);
    //     }
    //
    //     private void RaiseOnStopSupportOrderGiven()
    //     {
    //         isSupportOrderGiven = false;
    //         onStopSupportOrderGiven?.Invoke(this);
    //     }
    //
    //     private void RaiseOnStartSupportOrderGiven()
    //     {
    //         isSupportOrderGiven = true;
    //         onStartSupportOrderGiven?.Invoke(this);
    //     }
    //
    //     private void RaiseOnOrderChanged(GroupBehaviour.State newState)
    //     {
    //         onOrderChanged?.Invoke(this, newState);   
    //     }
    //
    //     private void RaiseOnTankOrderGiven()
    //     {
    //         onTankOrderGiven?.Invoke(this);
    //     }
    //
    //     private void RaiseOnRealOrderChanged()
    //     {
    //         OnRealOrderChanged?.Invoke();
    //     }
    //
    //     #endregion
    //
    //     #endregion
    //
    //     #region FSM
    //
    //     // The time after the next update of the FSM
    //     public float fsmReactionTime = 1f;
    //     public State currentState;
    //     public State newState;
    //     public bool orderConfirmed = false;
    //
    //     private FSM groupFSM;
    //     // Used to know if the group is in combat or not (don't want to add a state in State enum cause it's simpler this way)
    //     private bool inCombat = false;
    //     FSMState meleeState, tankState, rangeAttackState, supportState, recruitState, idleState;
    //     private GameObject target;
    //
    //     public GameObject Target { get => target; set => target = value; }
    //     //public int DemonsInGroup { get => demonsInGroup; set => demonsInGroup = value; }
    //
    //     #region Conditions
    //
    //     public bool MeleeOrderGiven() {
    //         if((newState != currentState) && (orderConfirmed) && (newState == State.MeleeAttack)) {
    //             return true;
    //         }
    //         return false;
    //     }
    //
    //     public bool TankOrderGiven() {
    //         if((newState.ToString() != groupFSM.current.stateName) && (orderConfirmed) && (newState == State.Tank)) {
    //             return true;
    //         }
    //         return false;
    //     }
    //
    //     public bool RangeAttackOrderGiven() {
    //         if((newState.ToString() != groupFSM.current.stateName) && (orderConfirmed) && (newState == State.RangeAttack)) {
    //             return true;
    //         }
    //         return false;
    //     }
    //
    //     public bool SupportOrderGiven() {
    //         if((newState != currentState) && (orderConfirmed) && (newState == State.Support)) {
    //             return true;
    //         }
    //         return false;
    //     }
    //
    //     public bool RecruitOrderGiven() {
    //         if((newState != currentState) && (orderConfirmed) && (newState == State.Recruit)) {
    //             return true;
    //         }
    //         return false;
    //     }
    //
    //     public bool Idle() {
    //         if(!inCombat)
    //             return true;
    //         return false;
    //     }
    //
    //     public bool EnterCombat() {
    //         if(!Idle())
    //             return true;
    //         return false;
    //     }
    //
    //     private void ConfirmEffects() {
    //         foreach(GameObject imp in groupManager.Imps) {
    //             if(imp)
    //                 imp.GetComponent<SelectedUnitEffects>().ConfirmOrder();
    //         }
    //     }
    //
    //     #endregion
    //
    //     #region Actions
    //
    //     public void GeneralEnterAction() {
    //         currentState = newState;
    //         ConfirmEffects();
    //         orderConfirmed = false;
    //     
    //         if(currentState == State.Tank)
    //             // Tank aggro was previously calculated and assigned here
    //             RaiseOnTankOrderGiven();
    //         else if(currentState == State.MeleeAttack || currentState == State.RangeAttack)
    //             continuousAttack = StartCoroutine(AttackOneTime(rateo));
    //     }
    //
    //     public void MeleeAttack() {
    //         if(!AllImpsFoundGroup())
    //             return;
    //
    //         if(EnemiesManager.Instance.Boss != null) {
    //             target = EnemiesManager.Instance.Boss;
    //         }
    //         else if(EnemiesManager.Instance.LittleEnemiesList.Count != 0) {
    //             target = CameraManager.FindNearestEnemy(gameObject, EnemiesManager.Instance.LittleEnemiesList.ToArray());
    //         }
    //         else
    //             return;
    //
    //         foreach(GameObject imp in groupManager.Imps) {
    //             if(imp) {
    //                 if(imp.GetComponent<AllyImpMovement>().HorizDistFromTargetBorders(target) < 1.5f) {
    //                     StartCoroutine(ActionAfterRandomDelay(imp, State.MeleeAttack));
    //                 }
    //
    //             }
    //         }
    //     }
    //
    //     // TODO - all demons from the group attack at the same time
    //     IEnumerator ActionAfterRandomDelay(GameObject demon, State action) {
    //         ImpCombatBehaviour impCombatBehaviour = demon.GetComponentInChildren<ImpCombatBehaviour>();
    //     
    //         if(impCombatBehaviour.enabled && demon.GetComponent<AllyImpMovement>().CanAct()) 
    //         {
    //             impCombatBehaviour.Attack();
    //         }
    //
    //         float randomDelay = UnityEngine.Random.Range(0f, 0.5f);
    //         yield return new WaitForSeconds(randomDelay);
    //     }
    //
    //     private void StopAttack() {
    //         if(!AllImpsFoundGroup())
    //             return;
    //     
    //         StopCoroutine(continuousAttack);
    //     }
    //
    //     private void TankStayAction() {
    //         if(!AllImpsFoundGroup())
    //             return;
    //         foreach(GameObject imp in groupManager.Imps) {
    //             if(imp) {
    //                 if(BattleEventsHandler.IsInBattle) {
    //                     if(!isTankOrderGiven)
    //                         RaiseOnStartTankOrderGiven();
    //                 }
    //                 else {
    //                     if(isTankOrderGiven)
    //                         RaiseOnStopTankOrderGiven();
    //                 }
    //             }
    //         }
    //     }
    //
    //     private void StopTank() {
    //         if(!AllImpsFoundGroup())
    //             return;
    //         foreach(GameObject imp in groupManager.Imps) {
    //             if(imp) {
    //                 if(isTankOrderGiven)
    //                     RaiseOnStopTankOrderGiven();
    //             }
    //         }
    //     }
    //
    //     private void RangeAttack() {
    //         if(!AllImpsFoundGroup())
    //             return;
    //     
    //         if(EnemiesManager.Instance.Boss != null) {
    //             target = EnemiesManager.Instance.Boss;
    //         }
    //         else if(EnemiesManager.Instance.LittleEnemiesList.Count != 0) {
    //             target = CameraManager.FindNearestEnemy(gameObject, EnemiesManager.Instance.LittleEnemiesList.ToArray());
    //         }
    //         else
    //             return;
    //
    //
    //         foreach(GameObject imp in groupManager.Imps) {
    //             if(imp) {
    //                 StartCoroutine(ActionAfterRandomDelay(imp, State.RangeAttack));
    //             }
    //         }
    //     }
    //
    //     private void StopRangeAttack() {
    //         if(!AllImpsFoundGroup())
    //             return;
    //
    //         foreach(GameObject imp in groupManager.Imps) {
    //             if(imp) {
    //                 //Combat combat = imp.GetComponent<Combat>();
    //                 //NormalCombat normalCombat = imp.GetComponentInChildren<NormalCombat>();
    //
    //                 //ombat.StopRangedAttack();
    //                 //normalCombat.StopAttack(meleeAttack);
    //             }
    //         }
    //
    //         StopCoroutine(continuousAttack);
    //     }
    //
    //     //private void SupportStayAction() {
    //     //    if(!AllImpsFoundGroup())
    //     //        return;
    //     //    foreach(GameObject imp in groupManager.Imps) {
    //     //        if(imp) {
    //     //            if(BattleEventsHandler.IsInBossBattle || BattleEventsHandler.IsInRegularBattle) {
    //     //                if(!isSupportOrderGiven)
    //     //                    RaiseOnStartSupportOrderGiven();
    //     //            }
    //     //            else {
    //     //                if(isSupportOrderGiven)
    //     //                    RaiseOnStopSupportOrderGiven();
    //     //            }
    //     //        }
    //     //    }
    //     //}
    //
    //     //private void StopSupport() {
    //     //    if(!AllImpsFoundGroup())
    //     //        return;
    //
    //     //    foreach(GameObject imp in groupManager.Imps) {
    //     //        if(imp)
    //     //        {
    //     //            if (isSupportOrderGiven)
    //     //            {
    //     //                RaiseOnStopSupportOrderGiven();
    //     //            }
    //     //        }
    //     //    }
    //     //}
    //
    //     public void RecruitStayAction() {
    //         if(!AllImpsFoundGroup())
    //             return;
    //         foreach(GameObject imp in groupManager.Imps) {
    //             if(imp) {
    //                 //Combat combat = imp.GetComponent<Combat>();
    //                 if(BattleEventsHandler.IsInBattle && imp.GetComponent<AllyImpMovement>().CanAct()) {
    //                     //combat.StartRecruit();
    //                     //if(!isRecruitOrderGiven)
    //                     RaiseOnStartRecruitOrderGiven();
    //                 }
    //                 else {
    //                     //combat.StopRecruit();
    //                     //if(isRecruitOrderGiven)
    //                     RaiseOnStopSupportOrderGiven();
    //                 }
    //             }
    //         }
    //     }
    //
    //     public void StopRecruit() {
    //         if(!AllImpsFoundGroup())
    //             return;
    //
    //         foreach(GameObject imp in groupManager.Imps) {
    //             if(imp) {
    //                 RaiseOnStopRecruitOrderGiven();
    //             }
    //         }
    //     }
    //
    //     #endregion
    //
    //     #endregion
    //
    //     #region Methods
    //
    //     public bool AllImpsFoundGroup()
    //     {
    //         if (!impsAlreadyChecked)
    //         {
    //             foreach (GameObject go in AlliesManager.Instance.AlliesList)
    //             {
    //                 if (go && !go.GetComponent<GroupFinder>().GroupFound)
    //                     return false;
    //             }
    //         
    //             impsAlreadyChecked = true;
    //         }
    //         return true;
    //     }
    //
    //     private void AssignOrder(State newState)
    //     {
    //         if (groupFSM.current.stateName == newState.ToString()) return;
    //     
    //         this.newState = newState;
    //         orderConfirmed = true;
    //     
    //         RaiseOnOrderChanged(newState);
    //     }
    //
    //     #endregion
    //
    //     public FSMState GetCurrentFSMState(State state) {
    //         switch(state) {
    //             case State.MeleeAttack:
    //                 return meleeState;
    //             case State.Tank:
    //                 return tankState;
    //             case State.RangeAttack:
    //                 return rangeAttackState;
    //             //case State.Support:
    //             //    return supportState;
    //             case State.Recruit:
    //                 return recruitState;
    //         }
    //         return null;
    //     }
    //
    //     // The coroutine that cycles through the FSM
    //     public IEnumerator MoveThroughFSM() {
    //         while(true) {
    //             yield return new WaitForSeconds(fsmReactionTime);
    //             //Debug.Log( groupFSM.current.stateName );
    //             groupFSM.Update();
    //         }
    //     }
    //
    //     private void Awake()
    //     {
    //         groupManager = this.gameObject.GetComponent<GroupManager>();
    //         currentState = State.MeleeAttack;
    //         newState = State.MeleeAttack;
    //         // Just to test
    //         inCombat = true;
    //     }
    //
    //     private void OnEnable()
    //     {
    //         TacticsManager.OnTryOrderAssign += OnTryOrderAssign;
    //     
    //         BattleEventsManager.onBattleEnter += OnBattleEnter;
    //     }
    //
    //     private void OnDisable()
    //     {
    //         AlliesManager.Instance.onNewImpSpawned -= OnNewImpSpawned;
    //     
    //         TacticsManager.OnTryOrderAssign -= OnTryOrderAssign;
    //
    //         BattleEventsManager.onBattleEnter -= OnBattleEnter;
    //     }
    //
    //     void Start() {
    //         AlliesManager.Instance.onNewImpSpawned += OnNewImpSpawned;
    //
    //         #region FSM
    //
    //         FSMTransition t1 = new FSMTransition(MeleeOrderGiven);
    //         FSMTransition t2 = new FSMTransition(TankOrderGiven);
    //         FSMTransition t3 = new FSMTransition(RangeAttackOrderGiven);
    //         FSMTransition t5 = new FSMTransition(Idle);
    //         FSMTransition t6 = new FSMTransition(EnterCombat);
    //         FSMTransition t7 = new FSMTransition(RecruitOrderGiven);
    //
    //         meleeState = new FSMState(State.MeleeAttack.ToString());
    //         tankState = new FSMState(State.Tank.ToString());
    //         rangeAttackState = new FSMState(State.RangeAttack.ToString());
    //         recruitState = new FSMState(State.Recruit.ToString());
    //         idleState = new FSMState();
    //
    //         meleeState.enterActions.Add(GeneralEnterAction);
    //         //meleeState.stayActions.Add( MeleeAttack );
    //         meleeState.exitActions.Add(StopAttack);
    //
    //         rangeAttackState.enterActions.Add(GeneralEnterAction);
    //         //rangeAttackState.stayActions.Add( RangeAttack );
    //         rangeAttackState.exitActions.Add(StopRangeAttack);
    //
    //         tankState.enterActions.Add(GeneralEnterAction);
    //         tankState.enterActions.Add(TankStayAction);
    //         tankState.stayActions.Add(TankStayAction);
    //         tankState.exitActions.Add(StopTank);
    //
    //         //supportState.enterActions.Add(GeneralEnterAction);
    //         //supportState.stayActions.Add(SupportStayAction);
    //         // Managed in support
    //         //supportState.stayActions.Add(UpdateSupportAggro);
    //         //supportState.exitActions.Add(StopSupport);
    //
    //         recruitState.enterActions.Add(GeneralEnterAction);
    //         recruitState.stayActions.Add(RecruitStayAction);
    //         recruitState.exitActions.Add(StopRecruit);
    //
    //         meleeState.AddTransition(t2, tankState);
    //         meleeState.AddTransition(t3, rangeAttackState);
    //         //meleeState.AddTransition(t4, supportState);
    //         meleeState.AddTransition(t5, idleState);
    //         meleeState.AddTransition(t7, recruitState);
    //
    //         tankState.AddTransition(t1, meleeState);
    //         tankState.AddTransition(t3, rangeAttackState);
    //         //tankState.AddTransition(t4, supportState);
    //         tankState.AddTransition(t5, idleState);
    //         tankState.AddTransition(t7, recruitState);
    //
    //         rangeAttackState.AddTransition(t1, meleeState);
    //         rangeAttackState.AddTransition(t2, tankState);
    //         //rangeAttackState.AddTransition(t4, supportState);
    //         rangeAttackState.AddTransition(t5, idleState);
    //         rangeAttackState.AddTransition(t7, recruitState);
    //
    //         //supportState.AddTransition(t1, meleeState);
    //         //supportState.AddTransition(t2, tankState);
    //         //supportState.AddTransition(t3, rangeAttackState);
    //         //supportState.AddTransition(t5, idleState);
    //         //supportState.AddTransition(t7, recruitState);
    //
    //         recruitState.AddTransition(t1, meleeState);
    //         recruitState.AddTransition(t2, tankState);
    //         recruitState.AddTransition(t3, rangeAttackState);
    //         //recruitState.AddTransition(t4, supportState);
    //         recruitState.AddTransition(t5, idleState);
    //
    //         idleState.AddTransition(t6, GetCurrentFSMState(currentState));
    //
    //         //groupFSM = new FSM( idleState );
    //         groupFSM = new FSM(meleeState);
    //         StartCoroutine(MoveThroughFSM());
    //
    //         #endregion
    //     }
    //     
    //     private IEnumerator AttackOneTime(float rateo) {
    //         while(true) {
    //             yield return new WaitForSeconds(rateo);
    //             if(currentState == State.MeleeAttack)
    //                 MeleeAttack();
    //             else if(currentState == State.RangeAttack)
    //                 RangeAttack();
    //         }
    //     }
    //
    //     private void OnTryOrderAssign(Order order, GroupManager.Group group)
    //     {
    //         if(groupManager.ThisGroupName == group || group == GroupManager.Group.All )
    //             AssignOrder(state);
    //     }
    //
    //     private void OnBattleEnter()
    //     {
    //         target = GameObject.FindWithTag("Boss");
    //     }
    //
    //     private void OnNewImpSpawned(GameObject newImp)
    //     {
    //         impsAlreadyChecked = false;
    //     }
    }
}
