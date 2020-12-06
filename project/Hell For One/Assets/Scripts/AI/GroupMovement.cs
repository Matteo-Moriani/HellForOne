using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;
using UnityEngine.AI;

public class GroupMovement : MonoBehaviour {
    private float distanceAllowed = 2.2f;

    [SerializeField]
    private GameObject player;
    private GameObject target;
    private GameObject bossPositions;
    private GameObject groupsFormation;
    private Transform targetPosition;
    private Transform meleePosition;
    private Transform rangedPosition;
    private Transform outOfCombatPosition;
    private GroupBehaviour gb;
    private bool haveTarget = false;
    private bool vsBoss = false;
    private bool outOfCombat = false;

    public float DistanceAllowed { get => distanceAllowed; set => distanceAllowed = value; }
    public GameObject Target { get => target; set => target = value; }

    private void OnEnable() {
        BattleEventsManager.onBattleExit += SetOutOfCombat;
        BattleEventsManager.onBattleEnter += SetVsBoss;
    }

    private void OnDisable() {
        BattleEventsManager.onBattleExit -= SetOutOfCombat;
        BattleEventsManager.onBattleEnter -= SetVsBoss;
    }

    private void Awake() {
        gb = GetComponent<GroupBehaviour>();
    }

    void Start() {
        targetPosition = gameObject.transform;
        player = GameObject.FindGameObjectWithTag("Player");
        bossPositions = GameObject.FindGameObjectWithTag("BossPositions");
        groupsFormation = GameObject.FindGameObjectWithTag("GroupsFormation");

        // Testing out of combat start
        SetOutOfCombat();
    }

    void FixedUpdate() {
        if(targetPosition)
            transform.position = targetPosition.position;
    }

    void Update() {
        // Adding OR target == null make the ally demons follow the player 
        // when out of combat, idk why.
        if(!haveTarget || Target == null) {
            ChooseTarget();
        }

        // i update the target position only if the group is too far to do its things
        if(vsBoss) {
            ChooseTargetPosition();
            // if the boss is escaped since I positioned
            //if(distanceInPosition < (transform.position - target.transform.position).magnitude) {
            //    inRangedPosition = false;
            //    distanceInPosition = float.MaxValue;
            //}
        }
        else {
            targetPosition = outOfCombatPosition;
        }

    }
    
    public void ChooseTarget() {
        SearchTarget();

        if(vsBoss) {
            bossPositions = GameObject.FindGameObjectWithTag("BossPositions");
            ChooseBossPositions();
        }
        if(outOfCombat) {
            ChooseOutOfCombatPosition();
        }
    }

    public float HorizDistFromTargetBorders(GameObject target) {
        Vector3 closestPoint = target.GetComponent<Collider>().ClosestPoint(transform.position);
        Vector3 targetPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        return (targetPosition - transform.position).magnitude;
    }

    private void PrepareDemonsToBattle(GameObject target) {
        foreach(GameObject imp in GetComponent<GroupManager>().Imps) {
            if(imp != null)
                imp.GetComponent<AllyImpMovement>().PrepareForBattleWith(target);
        }
    }

    private void FacePlayer() {
        Vector3 targetPosition = player.transform.position;
        Vector3 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.y = 0f;
        Quaternion facingDir = Quaternion.LookRotation(vectorToTarget);
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, facingDir, 0.1f);
        transform.rotation = newRotation;
    }

    public void SetVsBoss() {
        vsBoss = true;
        outOfCombat = false;
        haveTarget = false;
    }

    public void SetOutOfCombat() {
        vsBoss = false;
        outOfCombat = true;
        haveTarget = false;
    }

    private void SearchTarget() {
        
        if(EnemiesManager.Instance.Boss != null) {
            // TODO - Testing new logic
            // SetVsBoss();
            Target = EnemiesManager.Instance.Boss;
            PrepareDemonsToBattle(Target);
        }
        else {
            // TODO - Testing new logic
            // SetOutOfCombat();
            if(player == null) {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            if(player != null) {
                Target = player;
                PrepareDemonsToBattle(Target);
            }
            //else { 
            //    Debug.Log(this.gameObject.name + "Cannot find Player");    
            //}
        }
        
    }

    private void ChooseOutOfCombatPosition() {
        foreach(Transform position in groupsFormation.GetComponent<GroupsFormation>().GetPositions()) {
            if(groupsFormation.GetComponent<GroupsFormation>().GetAvailability(position)) {
                outOfCombatPosition = position;
                groupsFormation.GetComponent<GroupsFormation>().SetAvailability(position, false);
                haveTarget = true;
                break;
            }
        }
    }

    private void ChooseBossPositions() {
        foreach(Transform position in bossPositions.GetComponent<BossPositions>().GetMeleePositions()) {
            if(bossPositions.GetComponent<BossPositions>().GetAvailability(position)) {
                meleePosition = position;
                rangedPosition = bossPositions.GetComponent<BossPositions>().GetClosestRanged(position);
                bossPositions.GetComponent<BossPositions>().SetAvailability(position, false);
                haveTarget = true;

                // TODO - sostituire poi con questo

                //switch(outOfCombatPosition.name) {
                //    case "right":
                //        break;
                //    case "left":
                //        break;
                //    case "front":
                //        break;
                //    case "behind":
                //        break;
                //    default:
                //        break;
                //}

                break;
            }
        }
    }

    // TODO - questa cosa andrebbe fatta invece che negli update solo quando viene dato un nuovo ordine
    // Probabilmente non andrebbe proprio fatto...
    public void ChooseTargetPosition() {
        switch(gb.currentState) {
            case GroupBehaviour.State.MeleeAttack:
            case GroupBehaviour.State.Tank:
                targetPosition = meleePosition;
                break;
            case GroupBehaviour.State.RangeAttack:
            case GroupBehaviour.State.Support:
            case GroupBehaviour.State.Recruit:
                targetPosition = rangedPosition;
                break;
            default:
                Debug.Log("NEW ORDER NOT YET ASSIGNED TO A POSITION!");
                break;
        }

        transform.position = targetPosition.position;
    }
}
