using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBehavior : MonoBehaviour
{
    public float speed = 8f;
    [Range(0f,1f)]
    public float rotSpeed = 0.1f;
    public float stopDist = 4f;
    public float pause = 3f;
    public float initialHP = 100f;

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
    private bool timerRunning = false;
    private bool resetFightingBT = false;
    private CRBT.BehaviorTree FightingBT;
    private Coroutine fightingCR;
    private bool canWalk = false;


    FSMState waitingState, fightingState, stunnedState;

    [SerializeField]
    private float fsmReactionTime = 1f;
    private float btReactionTime = 0.05f;

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
        StartCoroutine(WaitSeconds(5));
        return true;
    }

    // The coroutine that cycles through the FSM
    public IEnumerator MoveThroughFSM() {
        while(true) {
            bossFSM.Update();
            yield return new WaitForSeconds(fsmReactionTime);
        }
    }

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
            Vector3 targetPosition = currentTarget.transform.position;
            Vector3 vectorToTarget = targetPosition - transform.position;
            vectorToTarget.y = 0f;
            Quaternion facingDir = Quaternion.LookRotation(vectorToTarget);
            Quaternion newRotation = Quaternion.Slerp(transform.rotation, facingDir, rotSpeed);
            transform.rotation = newRotation;

            if(canWalk) {
                transform.position += transform.forward * speed * Time.deltaTime;
            }
        }
    }

    private IEnumerator WaitSeconds(int s) {
        yield return new WaitForSeconds(s);
    }

    private IEnumerator Timer(int s) {
        timerRunning = true;
        yield return new WaitForSeconds(s);
        timerRunning = false;
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

    public bool TimeoutStarted() {
        return timerRunning;
    }

    public bool TimeoutEnded() {
        return !timerRunning;
    }

    public bool ChooseTarget() {

        if(GameObject.FindGameObjectsWithTag("Demon").Length == 0)
            return false;

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

        return true;
    }

    public bool WaitABit() {
        StartCoroutine(WaitSeconds(2));
        return true;
    }

    public bool WalkToTarget() {
        if(HorizDistFromTarget(currentTarget) > stopDist) {
            canWalk = true;
            return true;
        } else {
            canWalk = false;
            return false;
        }
    }

    public bool StartTimer() {
        StartCoroutine(Timer(5));
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

        CRBT.BTCondition c1 = new CRBT.BTCondition(TimeoutStarted);
        CRBT.BTCondition c2 = new CRBT.BTCondition(TimeoutEnded);

        CRBT.BTAction a1 = new CRBT.BTAction(ChooseTarget);
        CRBT.BTAction a2 = new CRBT.BTAction(WaitABit);
        CRBT.BTAction a3 = new CRBT.BTAction(StartTimer);
        CRBT.BTAction a4 = new CRBT.BTAction(WalkToTarget);
        CRBT.BTAction a5 = new CRBT.BTAction(RandomAttack);

        CRBT.BTSelector sel1 = new CRBT.BTSelector(new CRBT.IBTTask[] { c1, a3 });

        CRBT.BTSelector sel2 = new CRBT.BTSelector(new CRBT.IBTTask[] { c2, a4 });

        CRBT.BTSequence seq1 = new CRBT.BTSequence(new CRBT.IBTTask[] { sel1, sel2 });

        CRBT.BTDecoratorUntilFail uf1 = new CRBT.BTDecoratorUntilFail(seq1);

        CRBT.BTSequence seq2 = new CRBT.BTSequence(new CRBT.IBTTask[] { a1, a2, uf1, a5 });

        CRBT.BTDecoratorUntilFail utf2 = new CRBT.BTDecoratorUntilFail(seq2);

        return new CRBT.BehaviorTree(utf2);
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
}
