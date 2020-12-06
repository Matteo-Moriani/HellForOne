﻿using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using CRBT;
using UnityEngine;
using UnityEngine.AI;

public abstract class AbstractBoss : MonoBehaviour {

    public enum TimerType {
        stare,
        pursue,
        attack
    }

    // must initialize every value of the list at the top of the this script
    public abstract void InitializeValues();

    #region private fields

    // all these fields must be initialized in "InitializeValues()"
    private float absSpeed;                         // around 5
    private float absRotSpeed;
    private float absStopDist;
    private float absStareTime;
    private float absPursueTime;
    private float absChangeTargetProb;
    private GameObject absArenaCenter;
    private float absMaxDistFromCenter;             // around arena ray - 2.5
    private float absMaxTargetDistFromCenter;       // around arena ray - 1
    
    // while these not
    private GameObject player;
    private float[] aggroValues;
    private float[] probability;
    private float hp;
    private FSM fsm;
    private bool timerStarted1 = false;
    private bool timerStillGoing1 = false;
    private bool pursueTimeout = false;
    private bool targetFarFromCenter = false;
    private bool resetFightingBT = false;
    private CRBT.BehaviorTree fightingBT;
    private Coroutine fightingCR;
    private Coroutine timer1;
    private Coroutine attackCR;
    private bool canWalk = false;
    private bool canFace = true;
    private float fsmReactionTime = 0.5f;
    private float btReactionTime = 0.05f;
    private Stats stats;
    private bool demonsReady = false;
    private AnimationsManager animationsManager;
    private float facingIntervall = 0.5f;
    private NewHUD newHud;
    
    //private Combat bossCombat;
    private NormalCombat normalCombat;
    
    private GameObject targetGroup;
    private GameObject targetDemon;
    private bool isWalking = false;
    private bool isIdle = true;
    private bool isAttacking = false;
    private GameObjectSearcher searcher;
    private Coroutine faceCR;
    private bool faceCRisActive = true;
    private CombatEventsManager combatEventsManager;

    protected StunReceiver stunReceiver;
    protected bool isStunned = false;
    
    #endregion

    #region properties

    public CombatEventsManager CombatEventsManager { get => combatEventsManager; set => combatEventsManager = value; }
    public GameObject TargetGroup { get => targetGroup; set => targetGroup = value; }
    public GameObject TargetDemon { get => targetDemon; set => targetDemon = value; }
    public bool IsWalking { get => isWalking; set => isWalking = value; }
    public bool IsInPosition { get => isIdle; set => isIdle = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public GameObject Player { get => player; set => player = value; }
    public float[] AggroValues { get => aggroValues; set => aggroValues = value; }
    public float[] Probability { get => probability; set => probability = value; }
    public float Hp { get => hp; set => hp = value; }
    public FSM Fsm { get => fsm; set => fsm = value; }
    public bool TimerStarted1 { get => timerStarted1; set => timerStarted1 = value; }
    public bool TimerStillGoing1 { get => timerStillGoing1; set => timerStillGoing1 = value; }
    public bool PursueTimeout { get => pursueTimeout; set => pursueTimeout = value; }
    public bool TargetFarFromCenter { get => targetFarFromCenter; set => targetFarFromCenter = value; }
    public bool ResetFightingBT { get => resetFightingBT; set => resetFightingBT = value; }
    public BehaviorTree FightingBT { get => fightingBT; set => fightingBT = value; }
    public Coroutine FightingCR { get => fightingCR; set => fightingCR = value; }
    public Coroutine Timer1 { get => timer1; set => timer1 = value; }
    public Coroutine AttackCR { get => attackCR; set => attackCR = value; }
    public bool CanMove { get => canWalk; set => canWalk = value; }
    public bool CanFace { get => canFace; set => canFace = value; }
    public float FsmReactionTime { get => fsmReactionTime; set => fsmReactionTime = value; }
    public float BtReactionTime { get => btReactionTime; set => btReactionTime = value; }
    public Stats Stats { get => stats; set => stats = value; }
    public bool DemonsReady { get => demonsReady; set => demonsReady = value; }
    public AnimationsManager AnimationsManager { get => animationsManager; set => animationsManager = value; }
    public float FacingIntervall { get => facingIntervall; set => facingIntervall = value; }
    public NewHUD HUD { get => newHud; set => newHud = value; }

    //public Combat BossCombat { get => bossCombat; set => bossCombat = value; }
    public NormalCombat NormalCombat { get => normalCombat; protected set => normalCombat = value; }

    public float Speed { get => absSpeed; set => absSpeed = value; }
    public float RotSpeed { get => absRotSpeed; set => absRotSpeed = value; }
    public float StopDist { get => absStopDist; set => absStopDist = value; }
    public float StareTime { get => absStareTime; set => absStareTime = value; }
    public float PursueTime { get => absPursueTime; set => absPursueTime = value; }
    public float ChangeTargetProb { get => absChangeTargetProb; set => absChangeTargetProb = value; }
    public GameObject ArenaCenter { get => absArenaCenter; set => absArenaCenter = value; }
    public float MaxDistFromCenter { get => absMaxDistFromCenter; set => absMaxDistFromCenter = value; }
    public float MaxTargetDistFromCenter { get => absMaxTargetDistFromCenter; set => absMaxTargetDistFromCenter = value; }
    public GameObjectSearcher Searcher { get => searcher; set => searcher = value; }
    public Coroutine FaceCR { get => faceCR; set => faceCR = value; }
    public bool FaceCRisActive { get => faceCRisActive; set => faceCRisActive = value; }

    #endregion

    #region Delegates and events

    public delegate void OnStartIdle();
    public event OnStartIdle onStopMoving;

    public delegate void OnStartMoving();
    public event OnStartMoving onStartMoving;

    protected void RaiseOnStopMoving()
    {
        onStopMoving?.Invoke();
    }

    protected void RaiseOnStartMoving()
    {
        onStartMoving?.Invoke();
    }
    
    #endregion
    
    #region Finite State Machine

    public FSMState waitingState, fightingState, winState, deathState;

    public bool PlayerApproaching() {
        //TODO - transition between wait and fight event
        //if(playerDistance < tot)
        //    return true;
        return true;
    }

    public bool EnemiesAreDead() {
        int enemies = 0;
        foreach(GameObject demonGroup in GroupsManager.Instance.Groups) {
            enemies += demonGroup.GetComponent<GroupManager>().ImpsInGroupNumber;
        }
        if(GameObject.FindGameObjectWithTag("Player"))
            enemies += 1;
        if(enemies != 0)
            return false;
        else
            return true;

    }

    public bool Death() {
        if(Stats.health <= 0)
            return true;
        else
            return false;
    }

    public IEnumerator MoveThroughFSM() {
        while(true) {
            Fsm.Update();
            yield return new WaitForSeconds(FsmReactionTime);
        }
    }

    #endregion

    #region Fighting State Behavior Tree

    public IEnumerator FightingLauncherCR() {
        while(FightingBT.Step())
            yield return new WaitForSeconds(BtReactionTime);
    }

    public void StartFightingCoroutine() {
        if(ResetFightingBT) {
            FightingBT = FightingBTBuilder();
            ResetFightingBT = false;
        }

        FightingCR = StartCoroutine(FightingLauncherCR());
    }

    public void StopFightingBT() {
        StopCoroutine(FightingCR);
        FightingCR = null;
        ResetFightingBT = true;
    }

    public abstract CRBT.BehaviorTree FightingBTBuilder();

    public abstract bool ChooseTarget();

    public abstract bool ChooseAttack();

    public bool StareAtTarget() {
        if(Timer1 != null)
            StopCoroutine(Timer1);
        Timer1 = StartCoroutine(Timer(StareTime, TimerType.stare));
        return true;
    }

    public bool WalkToTarget() {
        if(HorizDistFromTarget(TargetDemon) > StopDist) {
            CanMove = true;
            return true;
        }
        else {
            // can walk = false verrà settato a fine attacco
            ResetTimer();
            return false;
        }
    }

    public bool TimeoutPursue() {
        StopCoroutine(Timer1);
        Timer1 = StartCoroutine(Timer(PursueTime, TimerType.pursue));
        return true;
    }

    public bool TimerStarted() {
        return TimerStarted1;
    }

    public bool TimerStillGoing() {
        if(!TimerStillGoing1) {
            ResetTimer();
            return false;
        }
        else {
            return true;
        }
    }

    public bool TargetNearArenaCenter() {
        if((ArenaCenter.transform.position - targetDemon.transform.position).magnitude > MaxTargetDistFromCenter && HorizDistFromTarget(ArenaCenter) > MaxDistFromCenter) {
            TargetFarFromCenter = true;
            CanMove = false;
            return false;
        }
        else
            return true;
    }

    #endregion
    
    #region Unity methods

    public void Awake() {
        Searcher = new GameObjectSearcher();
        AnimationsManager = GetComponent<AnimationsManager>();
        Stats = GetComponent<Stats>();
        combatEventsManager = GetComponent<CombatEventsManager>();
        HUD = GameObject.FindGameObjectWithTag("HUD").GetComponent<NewHUD>();
        ArenaCenter = GameObject.FindWithTag("ArenaCenter");

        Player = GameObject.FindGameObjectWithTag("Player");
        Hp = Stats.health;
        AggroValues = new float[GroupsManager.Instance.Groups.Length + 1];
        Probability = new float[GroupsManager.Instance.Groups.Length + 2];
        Probability[0] = 0f;
        FaceCR = StartCoroutine(FaceEvery(FacingIntervall));

        FightingBT = FightingBTBuilder();

        stunReceiver = GetComponentInChildren<StunReceiver>();
    }

    public void FixedUpdate() {
        if (isStunned)
            return;

        if(!Player)
            Player = GameObject.FindGameObjectWithTag("Player");
        if(!ArenaCenter)
            ArenaCenter = GameObject.FindGameObjectWithTag("ArenaCenter");

        if(!DemonsReady && GroupsManager.Instance.Groups[0].GetComponent<GroupBehaviour>().AllImpsFoundGroup()) {
            DemonsReady = true;
            StartCoroutine(MoveThroughFSM());
        }

        if(TargetDemon && !IsAttacking && Stats.health > 0) {
            if(!IsInPosition)
                Face(TargetDemon);
            else
                if(CanFace)
                Face(TargetDemon);

            if(CanMove) {
                if(!IsWalking) {
                    IsWalking = true;
                    IsInPosition = false;
                    RaiseOnStartMoving();
                }

                if(HorizDistFromTarget(TargetDemon) > StopDist)
                    transform.position += transform.forward * Speed * Time.deltaTime;
            }
            else {
                if(!IsInPosition) {
                    IsInPosition = true;
                    IsWalking = false;
                    RaiseOnStopMoving();
                }
            }

        }
        else if(!EnemiesAreDead()) {
            ChooseTarget();
        }
    }

    public void OnEnable() {
        stats.onDeath += OnDeath;
        stunReceiver.onStartStun += OnStartStun;
        stunReceiver.onStopStun += OnStopStun;
    }

    public void OnDisable() {
        stats.onDeath -= OnDeath;
        stunReceiver.onStartStun -= OnStartStun;
        stunReceiver.onStopStun -= OnStopStun;

    }

    #endregion

    protected virtual void StopMoving()
    {
        CanMove = false;
    }

    protected virtual void OnStartStun()
    {
        isStunned = true;
    }

    protected virtual void OnStopStun()
    {
        isStunned = false;
    }

    private void OnDeath(Stats sender) {
        StopAllCoroutines();
        HUD.DeactivateAggroIcon();
    }

    public IEnumerator Timer(float s, TimerType type) {
        TimerStarted1 = true;
        TimerStillGoing1 = true;
        yield return new WaitForSeconds(s);
        TimerStillGoing1 = false;
        if(type == TimerType.pursue) {
            PursueTimeout = true;
            CanMove = false;
        }
        else if(type == TimerType.attack) {
            //CombatEventsManager.RaiseOnStartIdle();
            // TODO - don't know why it doesn't work here
            //isAttacking = false;
            RaiseOnStopMoving();
        }

    }

    public void ResetTimer() {
        TimerStarted1 = false;
        TimerStillGoing1 = false;
        if(Timer1 != null)
            StopCoroutine(Timer1);
    }

    public IEnumerator FaceEvery(float f) {
        while(true) {
            if(FaceCRisActive) CanFace = true;
            yield return new WaitForSeconds(f);
            if(FaceCRisActive) CanFace = false;
            yield return new WaitForSeconds(f);
        }
    }

    public void Face(GameObject target) {
        Vector3 targetPosition = target.transform.position;
        Vector3 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.y = 0f;
        Quaternion facingDir = Quaternion.LookRotation(vectorToTarget);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, facingDir, RotSpeed);
        transform.rotation = newRotation;
    }

    //localScale.x/2f should be the "ray" of the boss (half of his his width)
    public float HorizDistFromTarget(GameObject target) {
        Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

        return (targetPosition - transform.position).magnitude - gameObject.transform.localScale.x/2f;
    }

    public GameObject ClosestGroupTo(Vector3 position) {

        GameObject closest = GroupsManager.Instance.Groups[0];
        float minDist = float.MaxValue;

        foreach(GameObject group in GroupsManager.Instance.Groups) {
            if(group.GetComponent<GroupManager>().IsEmpty() == false) {
                if((group.transform.position - position).magnitude < minDist) {
                    minDist = (group.transform.position - position).magnitude;
                    closest = group;
                }
            }
        }

        return closest;
    }

    public GameObject ChooseCentralTarget() {
        TargetGroup = ClosestGroupTo(ArenaCenter.transform.position);
        foreach(GameObject imp in TargetGroup.GetComponent<GroupManager>().Imps) {
            if(imp != null) {
                return imp;
            }
        }

        // returns player if every demon of the group is dead in the meanwhile
        return Player;
    }

    public GameObject[] GetDemonGroups() {
        return GroupsManager.Instance.Groups;
    }

    public void ChangeTarget(GameObject newTarget) {
        if(Stats.health > 0) {
            if(TargetDemon != null) {
                Searcher.FindObjectWithTag(TargetDemon.transform, "AggroIcon");
                Searcher.GetFirstChildWithTag().GetComponent<BossFaceRotations>().BossFaceOFF();
            }

            TargetDemon = newTarget;

            Searcher.FindObjectWithTag(TargetDemon.transform, "AggroIcon");
            Searcher.GetFirstChildWithTag().GetComponent<BossFaceRotations>().BossFaceON();
        }
    }

    // TODO - Optimize
    public void ChooseByAggro() {
        float totalAggro = 0f;

        for(int i = 0; i < GroupsManager.Instance.Groups.Length; i++) {
            float groupAggro = 0f;
            // if the group is empty, I give to the group a temporary value of zero
            if (!GroupsManager.Instance.Groups[i].GetComponent<GroupManager>().IsEmpty())
                groupAggro = GroupsManager.Instance.Groups[i].GetComponent<GroupAggro>().GroupAggroValue;
            AggroValues[i] = groupAggro;
            totalAggro = totalAggro + groupAggro;
            Probability[i + 1] = totalAggro;
        }

        float playerAggro = Player.GetComponent<ImpAggro>().Aggro;
        
        AggroValues[GroupsManager.Instance.Groups.Length] = playerAggro;
        totalAggro = totalAggro + playerAggro;
        Probability[GroupsManager.Instance.Groups.Length + 1] = totalAggro;

        float random = UnityEngine.Random.Range(0f, totalAggro);

        // if I was pursuing the player, I won't choose him again
        if(PursueTimeout)
            random = UnityEngine.Random.Range(0f, totalAggro - playerAggro);

        for(int i = 1; i < Probability.Length; i++) {
            if(random > Probability[i - 1] && random <= Probability[i]) {
                // if i'm talking about a group (player probability is in the last slot of the array)
                if(i < Probability.Length - 1) {
                    TargetGroup = GroupsManager.Instance.Groups[i - 1];
                    GameObject tempDemon = TargetGroup.GetComponent<GroupManager>().GetRandomImp();

                    //check if the chosen demon is too far from center
                    if((tempDemon.transform.position - ArenaCenter.transform.position).magnitude > MaxDistFromCenter || TargetFarFromCenter)
                        tempDemon = ChooseCentralTarget();

                    ChangeTarget(tempDemon);
                }
                else {
                    ChangeTarget(Player);
                }

                break;
            }
        }
    }

}
