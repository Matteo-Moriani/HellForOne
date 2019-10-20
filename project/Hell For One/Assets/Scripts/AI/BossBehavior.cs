using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ResetTimer at the beginning of ChooseTarget but I don't understand why it doesn't stop the second timer

public class BossBehavior : MonoBehaviour
{
    public float speed = 8f;
    [Range(0f,1f)]
    public float rotSpeed = 0.1f;
    public float stopDist = 4.5f;
    public float stare = 2f;
    public float timeout = 5f;
    public float initialHP = 100f;
    [Range(0f, 1f)]
    public float changeTargetProb = 0.3f;

    private GameObject[] demonGroups;
    private GameObject currentTarget;
    private float[] aggroValues;
    private float[] probability;
    private readonly float singleAttackProb = 0.6f;
    private readonly float groupAttackProb = 0.3f;
    private readonly float globalAttackProb = 0.1f;
    private float crisis = 0f;
    private float crisisMax = 50f;
    private float hp;
    private FSM bossFSM;
    private bool timerStarted = false;
    private bool timerStillGoing = false;
    private bool resetFightingBT = false;
    private CRBT.BehaviorTree FightingBT;
    private Coroutine fightingCR;
    private Coroutine timer;
    private bool canWalk = false;
    [SerializeField]
    private float fsmReactionTime = 0.5f;
    [SerializeField]
    private float btReactionTime = 0.05f;

    #region Finite State Machine

    FSMState waitingState, fightingState, stunnedState;

    public bool PlayerApproaching() {
        //if(playerDistance < tot)
        //    return true;
        return true;
    }

    public bool CrisisFull() {
        if(crisis >= crisisMax)
            return true;
        return false;
    }

    public bool LifeIsHalven() {
        if(hp <= initialHP/2)
            return true;
        return false;
    }

    public bool RecoverFromStun() {
        //StartCoroutine(WaitSeconds(5));
        return true;
    }

    // The coroutine that cycles through the FSM
    public IEnumerator MoveThroughFSM() {
        while(true) {
            bossFSM.Update();
            yield return new WaitForSeconds(fsmReactionTime);
        }
    }

    #endregion

    #region Fighting State Behavior Tree

    public bool TimerStarted() {
        return timerStarted;
    }

    public bool TimerStillGoing() {
        if(!timerStillGoing) {
            ResetTimer();
            return false;
        }
        else {
            return true;
        }
    }

    private void ResetTimer() {
        timerStarted = false;
        timerStillGoing = false;
    }

    public bool ChooseTarget() {
        ResetTimer();

        if(GameObject.FindGameObjectsWithTag("Demon").Length == 0)
            return false;

        if(Random.Range(0f, 1f) < changeTargetProb || currentTarget == gameObject) {
            float totalAggro = 0f;
            for(int i = 0; i < demonGroups.Length; i++) {
                aggroValues[i] = demonGroups[i].GetComponent<TargetScript>().GetAggro();
                totalAggro = totalAggro + demonGroups[i].GetComponent<TargetScript>().GetAggro();
                probability[i + 1] = totalAggro;
            }

            float random = Random.Range(0.001f, totalAggro);

            for(int i = 1; i < probability.Length; i++) {
                if(random > probability[i - 1] && random <= probability[i])
                    currentTarget = demonGroups[i - 1];
            }
            //Debug.Log("target: " + currentTarget.name);
        }
        else {
            Debug.Log("target won't change this time");
        }

        return true;
    }

    public bool StareAtTarget() {
        if(timer != null)
            StopCoroutine(timer);
        timer = StartCoroutine(Timer(stare));
        return true;
    }

    public bool WalkToTarget() {
        if(HorizDistFromTarget(currentTarget) > stopDist) {
            canWalk = true;
            return true;
        }
        else {
            canWalk = false;
            return false;
        }
    }

    public bool TimeoutAttack() {
        StopCoroutine(timer);
        timer = StartCoroutine(Timer(timeout));
        return true;
    }

    public bool RandomAttack() {
        float random = Random.Range(0f, singleAttackProb + groupAttackProb + globalAttackProb);
        if(random < singleAttackProb)
            SingleAttack();
        else if(random >= singleAttackProb && random < singleAttackProb + groupAttackProb)
            GroupAttack();
        else
            GlobalAttack();

        return true;
    }

    public CRBT.BehaviorTree FightingBTBuilder() {

        CRBT.BTCondition started = new CRBT.BTCondition(TimerStarted);
        CRBT.BTCondition going = new CRBT.BTCondition(TimerStillGoing);

        CRBT.BTAction target = new CRBT.BTAction(ChooseTarget);
        CRBT.BTAction stare = new CRBT.BTAction(StareAtTarget);
        CRBT.BTAction timeout = new CRBT.BTAction(TimeoutAttack);
        CRBT.BTAction walk = new CRBT.BTAction(WalkToTarget);
        CRBT.BTAction attack = new CRBT.BTAction(RandomAttack);

        CRBT.BTSelector sel1 = new CRBT.BTSelector(new CRBT.IBTTask[] { started, timeout });
        CRBT.BTSelector sel3 = new CRBT.BTSelector(new CRBT.IBTTask[] { started, stare });
        CRBT.BTSequence seq4 = new CRBT.BTSequence(new CRBT.IBTTask[] { going, walk });

        CRBT.BTSequence seq1 = new CRBT.BTSequence(new CRBT.IBTTask[] { sel1, seq4 });
        CRBT.BTDecoratorUntilFail uf1 = new CRBT.BTDecoratorUntilFail(seq1);

        CRBT.BTSequence seq3 = new CRBT.BTSequence(new CRBT.IBTTask[] { sel3, going });
        CRBT.BTDecoratorUntilFail uf2 = new CRBT.BTDecoratorUntilFail(seq3);

        CRBT.BTSequence seq2 = new CRBT.BTSequence(new CRBT.IBTTask[] { target, uf2, uf1, attack });

        CRBT.BTDecoratorUntilFail root = new CRBT.BTDecoratorUntilFail(seq2);

        return new CRBT.BehaviorTree(root);
    }

    public IEnumerator FightingLauncherCR() {
        while(FightingBT.Step())
            yield return new WaitForSeconds(btReactionTime);
    }

    public void StartFightingCoroutine() {
        if(resetFightingBT) {
            FightingBT = FightingBTBuilder();
            resetFightingBT = false;
        }

        fightingCR = StartCoroutine(FightingLauncherCR());
    }

    public void StopFightingBT() {
        StopCoroutine(fightingCR);
        fightingCR = null;
        resetFightingBT = true;
    }

    #endregion

    void Start()
    {
        FSMTransition t0 = new FSMTransition(PlayerApproaching);
        FSMTransition t1 = new FSMTransition(CrisisFull);
        FSMTransition t2 = new FSMTransition(LifeIsHalven);
        FSMTransition t3 = new FSMTransition(RecoverFromStun);

        FightingBT = FightingBTBuilder();

        waitingState = new FSMState();

        fightingState = new FSMState();
        fightingState.enterActions.Add(StartFightingCoroutine);
        fightingState.exitActions.Add(StopFightingBT);

        stunnedState = new FSMState();
        

        waitingState.AddTransition(t0, fightingState);
        fightingState.AddTransition(t1, stunnedState);
        fightingState.AddTransition(t2, stunnedState);
        stunnedState.AddTransition(t3, fightingState);

        bossFSM = new FSM(waitingState);

        // the initial target is himself to stay on his place for the first seconds
        hp = initialHP;
        currentTarget = gameObject;
        demonGroups = GameObject.FindGameObjectsWithTag("Demon");
        aggroValues = new float[demonGroups.Length];
        probability = new float[demonGroups.Length + 1];
        probability[0] = 0f;

        StartCoroutine(MoveThroughFSM());
    }
    
    void Update()
    {
       
    }

    void FixedUpdate() {

        // in case I don't have a target anymore for some reason
        if(currentTarget.transform) {

            // I'm always facing my last target
            FaceTarget();

            if(canWalk) {
                transform.position += transform.forward * speed * Time.deltaTime;
            }
        }
    }

    private IEnumerator Timer(float s) {
        timerStarted = true;
        timerStillGoing = true;
        yield return new WaitForSeconds(s);
        timerStillGoing = false;
    }

    private void SingleAttack() {
        Debug.Log("single attack!");
    }

    private void GroupAttack() {
        Debug.Log("group attack!");
    }

    private void GlobalAttack() {
        Debug.Log("global attack!");
    }

    private float HorizDistFromTarget(GameObject target) {
        Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        return (targetPosition - transform.position).magnitude;
    }

    private void FaceTarget() {
        Vector3 targetPosition = currentTarget.transform.position;
        Vector3 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.y = 0f;
        Quaternion facingDir = Quaternion.LookRotation(vectorToTarget);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, facingDir, rotSpeed);
        transform.rotation = newRotation;
    }

}
