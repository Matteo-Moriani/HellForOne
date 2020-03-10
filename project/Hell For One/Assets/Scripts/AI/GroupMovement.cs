using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroupMovement : MonoBehaviour {
    public float rangedDist = 6f;
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
    private bool inRangedPosition = false;
    private float distanceInPosition = float.MaxValue;
    private bool vsLittleEnemies = false;
    private bool vsBoss = false;
    private bool outOfCombat = false;

    public float DistanceAllowed { get => distanceAllowed; set => distanceAllowed = value; }

    private void OnEnable() {
        BattleEventsManager.onBattleExit += SetOutOfCombat;
        BattleEventsManager.onBossBattleExit += SetOutOfCombat;

        BattleEventsManager.onBossBattleEnter += SetVsBoss;

        BattleEventsManager.onBattleEnter += SetVsLittleEnemies;
    }

    private void OnDisable() {
        BattleEventsManager.onBattleExit -= SetOutOfCombat;
        BattleEventsManager.onBossBattleExit -= SetOutOfCombat;

        BattleEventsManager.onBossBattleEnter -= SetVsBoss;

        BattleEventsManager.onBattleEnter -= SetVsLittleEnemies;
    }

    void Start() {
        targetPosition = gameObject.transform;
        player = GameObject.FindGameObjectWithTag("Player");
        bossPositions = GameObject.FindGameObjectWithTag("BossPositions");
        groupsFormation = GameObject.FindGameObjectWithTag("GroupsFormation");

        // Testing out of combat start
        SetOutOfCombat();
    }

    void Update() {
        // Adding OR target == null make the ally demons follow the player 
        // when out of combat, idk why.
        if(!haveTarget || target == null) {
            ChooseTarget();
        }

        // i update the target position only if the group is too far to do its things
        if(vsBoss) {
            switch(gb.currentState) {
                case GroupBehaviour.State.MeleeAttack:
                case GroupBehaviour.State.Tank:
                    targetPosition = meleePosition;
                    inRangedPosition = false;
                    break;
                case GroupBehaviour.State.RangeAttack:
                case GroupBehaviour.State.Support:
                case GroupBehaviour.State.Recruit:
                    if(!inRangedPosition) {
                        targetPosition = rangedPosition;
                        inRangedPosition = true;
                        distanceInPosition = (transform.position - target.transform.position).magnitude;
                    }
                    break;
                default:
                    Debug.Log("NEW ORDER NOT YET ASSIGNED TO A POSITION!");
                    break;
            }
            // if the boss is escaped since I positioned
            if(distanceInPosition < (transform.position - target.transform.position).magnitude) {
                inRangedPosition = false;
                distanceInPosition = float.MaxValue;
            }
        }
        //else if(vsLittleEnemies) {
        //    if(!target)
        //        SearchTarget();

        //    switch(gb.currentState) {
        //        case GroupBehaviour.State.MeleeAttack:
        //            targetPosition = target.transform;
        //            break;
        //        case GroupBehaviour.State.Tank:
        //            targetPosition = target.transform;
        //            break;
        //        case GroupBehaviour.State.RangeAttack:
        //            if(HorizDistFromTargetBorders(target) > rangedDist)
        //                targetPosition = target.transform;
        //            break;
        //        case GroupBehaviour.State.Support:
        //            if(HorizDistFromTargetBorders(target) > rangedDist)
        //                targetPosition = target.transform;
        //            break;
        //        default:
        //            break;
        //    }
        //}
        else {
            FacePlayer();
            targetPosition = outOfCombatPosition;
        }

    }

    void FixedUpdate() {
        if(targetPosition)
            transform.position = targetPosition.position;
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

    private void Awake() {
        gb = GetComponent<GroupBehaviour>();
    }

    private void SetDemonsTarget(GameObject target) {
        foreach(GameObject demon in GetComponent<GroupBehaviour>().demons) {
            if(demon != null)
                demon.GetComponent<DemonMovement>().SetTarget(target);
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

    public void SetVsLittleEnemies() {
        vsLittleEnemies = true;
        vsBoss = false;
        outOfCombat = false;
        haveTarget = false;
    }

    public void SetVsBoss() {
        vsLittleEnemies = false;
        vsBoss = true;
        outOfCombat = false;
        haveTarget = false;
    }

    public void SetOutOfCombat() {
        vsLittleEnemies = false;
        vsBoss = false;
        outOfCombat = true;
        haveTarget = false;
    }

    private void SearchTarget() {
        if(EnemiesManager.Instance.LittleEnemiesList.Count != 0) {
            // TODO - Testing new logic
            // SetVsLittleEnemies();
            target = EnemiesManager.Instance.LittleEnemiesList[Random.Range(0, EnemiesManager.Instance.LittleEnemiesList.Count)];
            SetDemonsTarget(target);
            haveTarget = true;
            //Debug.Log("new target is " + target.name);
        }
        else {
            if(EnemiesManager.Instance.Boss != null) {
                // TODO - Testing new logic
                // SetVsBoss();
                target = EnemiesManager.Instance.Boss;
                SetDemonsTarget(target);
            }
            else {
                // TODO - Testing new logic
                // SetOutOfCombat();
                if(player == null) {
                    player = GameObject.FindGameObjectWithTag("Player");
                }
                if(player != null) {
                    target = player;
                    SetDemonsTarget(target);
                }
                //else { 
                //    Debug.Log(this.gameObject.name + "Cannot find Player");    
                //}
            }
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

                // sostituire poi con questo

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

    public GameObject GetTarget() {
        return target;
    }
}
