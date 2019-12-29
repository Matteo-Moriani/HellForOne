using System.Collections;
using System.Collections.Generic;
using CRBT;
using UnityEngine;
using UnityEngine.AI;

public abstract class AbstractBoss : MonoBehaviour {

    public enum TimerType {
        stare,
        pursue,
        attack
    }
    
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
    private GameObject[] demonGroups;
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
    private CombatEventsManager combatEventsManager;
    private AnimationsManager animationsManager;
    private float facingIntervall = 0.5f;
    private HUD hud;
    private Combat bossCombat;
    private GameObject targetGroup;
    private GameObject targetDemon;
    private bool isWalking = false;
    private bool isIdle = true;
    private bool isAttacking = false;

    #endregion

    #region properties

    public GameObject TargetGroup { get => targetGroup; set => targetGroup = value; }
    public GameObject TargetDemon { get => targetDemon; set => targetDemon = value; }
    public bool IsWalking { get => isWalking; set => isWalking = value; }
    public bool IsIdle { get => isIdle; set => isIdle = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public GameObject[] DemonGroups { get => demonGroups; set => demonGroups = value; }
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
    public bool CanWalk { get => canWalk; set => canWalk = value; }
    public bool CanFace { get => canFace; set => canFace = value; }
    public float FsmReactionTime { get => fsmReactionTime; set => fsmReactionTime = value; }
    public float BtReactionTime { get => btReactionTime; set => btReactionTime = value; }
    public Stats Stats { get => stats; set => stats = value; }
    public bool DemonsReady { get => demonsReady; set => demonsReady = value; }
    public CombatEventsManager CombatEventsManager { get => combatEventsManager; set => combatEventsManager = value; }
    public AnimationsManager AnimationsManager { get => animationsManager; set => animationsManager = value; }
    public float FacingIntervall { get => facingIntervall; set => facingIntervall = value; }
    public HUD HUD { get => hud; set => hud = value; }
    public Combat BossCombat { get => bossCombat; set => bossCombat = value; }
    public float Speed { get => absSpeed; set => absSpeed = value; }
    public float RotSpeed { get => absRotSpeed; set => absRotSpeed = value; }
    public float StopDist { get => absStopDist; set => absStopDist = value; }
    public float StareTime { get => absStareTime; set => absStareTime = value; }
    public float PursueTime { get => absPursueTime; set => absPursueTime = value; }
    public float ChangeTargetProb { get => absChangeTargetProb; set => absChangeTargetProb = value; }
    public GameObject ArenaCenter { get => absArenaCenter; set => absArenaCenter = value; }
    public float MaxDistFromCenter { get => absMaxDistFromCenter; set => absMaxDistFromCenter = value; }
    public float MaxTargetDistFromCenter { get => absMaxTargetDistFromCenter; set => absMaxTargetDistFromCenter = value; }

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
        foreach(GameObject demonGroup in DemonGroups) {
            enemies += demonGroup.GetComponent<GroupBehaviour>().GetDemonsNumber();
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
            CanWalk = true;
            return true;
        }
        else {
            CanWalk = false;
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
            CanWalk = false;
            return false;
        }
        else
            return true;
    }

    #endregion

    // must initialize every value of the list at the top of the this script
    public abstract void InitializeValues();

    public void Awake() {
        CombatEventsManager = GetComponent<CombatEventsManager>();
        AnimationsManager = GetComponent<AnimationsManager>();
        Stats = GetComponent<Stats>();
        HUD = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();
        ArenaCenter = GameObject.FindWithTag("ArenaCenter");

        // TODO - find a better way to do this
        DemonGroups = new GameObject[GameObject.FindGameObjectsWithTag("Group").Length];
        DemonGroups[0] = GameObject.Find("GroupAzure");
        DemonGroups[1] = GameObject.Find("GroupPink");
        DemonGroups[2] = GameObject.Find("GroupGreen");
        DemonGroups[3] = GameObject.Find("GroupYellow");

        Player = GameObject.FindGameObjectWithTag("Player");
        Hp = Stats.health;
        AggroValues = new float[DemonGroups.Length + 1];
        Probability = new float[DemonGroups.Length + 2];
        Probability[0] = 0f;
        StartCoroutine(FaceEvery(FacingIntervall));

        FightingBT = FightingBTBuilder();
    }

    public void FixedUpdate() {
        if(!Player)
            Player = GameObject.FindGameObjectWithTag("Player");
        if(!ArenaCenter)
            ArenaCenter = GameObject.FindGameObjectWithTag("ArenaCenter");

        if(!DemonsReady && DemonGroups[0].GetComponent<GroupBehaviour>().CheckDemons()) {
            DemonsReady = true;
            StartCoroutine(MoveThroughFSM());
        }

        if(TargetDemon && !isAttacking && Stats.health > 0) {
            if(!isIdle)
                Face(TargetDemon);
            else
                if(CanFace)
                Face(TargetDemon);

            if(CanWalk) {
                if(!isWalking) {
                    IsWalking = true;
                    IsIdle = false;
                    CombatEventsManager.RaiseOnStartMoving();

                }

                transform.position += transform.forward * Speed * Time.deltaTime;

            }
            else {
                if(!isIdle) {
                    IsIdle = true;
                    IsWalking = false;
                    CombatEventsManager.RaiseOnStartIdle();
                }
            }

        }
        else if(!EnemiesAreDead()) {
            ChooseTarget();
        }
    }

    public void OnEnable() {
        CombatEventsManager.onDeath += OnDeath;
    }

    public void OnDisable() {
        CombatEventsManager.onDeath -= OnDeath;
    }

    public void OnDeath() {
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
            CanWalk = false;
        }
        else if(type == TimerType.attack) {
            CombatEventsManager.RaiseOnStartIdle();
            // TODO - don't know why it doesn't work here
            //isAttacking = false;
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
            CanFace = false;
            yield return new WaitForSeconds(f);
            CanFace = true;
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

    public float HorizDistFromTarget(GameObject target) {
        Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        return (targetPosition - transform.position).magnitude;
    }

    public GameObject ClosestGroupTo(Vector3 position) {

        GameObject closest = DemonGroups[0];
        float minDist = float.MaxValue;

        foreach(GameObject group in DemonGroups) {
            if(group.GetComponent<GroupBehaviour>().IsEmpty() == false) {
                if((group.transform.position - position).magnitude < minDist) {
                    minDist = (group.transform.position - position).magnitude;
                    closest = group;
                }
            }
        }

        return closest;
    }

    public void ChooseCentralTarget() {
        TargetGroup = ClosestGroupTo(ArenaCenter.transform.position);
        foreach(GameObject demon in TargetGroup.GetComponent<GroupBehaviour>().demons) {
            if(demon != null) {
                TargetDemon = demon;
                break;
            }
        }
    }

    public GameObject[] GetDemonGroups() {
        return DemonGroups;
    }

    public void SwitchAggroIcon(int index) {
        if(Stats.health > 0) {
            switch(index) {
                case 0:
                    HUD.ActivateAggroIcon(TacticsManager.Group.GroupAzure);
                    break;
                case 1:
                    HUD.ActivateAggroIcon(TacticsManager.Group.GroupPink);
                    break;
                case 2:
                    HUD.ActivateAggroIcon(TacticsManager.Group.GroupGreen);
                    break;
                case 3:
                    HUD.ActivateAggroIcon(TacticsManager.Group.GroupYellow);
                    break;
                default:
                    break;
            }
        }
    }

}
