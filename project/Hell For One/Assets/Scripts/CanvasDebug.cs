using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasDebug : MonoBehaviour
{
    public GroupBehaviour groupAzure, groupPink, groupGreen, groupYellow;
    public GroupAggro aggroA, aggroB, aggroC, aggroD;
    public Text aggroValueA, aggroValueB, aggroValueC, aggroValueD;
    public Text tacticA, tacticB, tacticC, tacticD;
    public Text bossTargetGroup;
    public Text currentGroup;
    public Text currentTactic;
    public Text regenCountdown;
    public Text playerAggro;
    public BossBehavior bossBehaviour;
    public TacticsManager tacticsManager;
    public AllyDemonSpawnerTest allyDemonSpawnerTest;
    public Stats playerStats;

    // Start is called before the first frame update
    void Start()
    {
        groupAzure = GameObject.Find( "GroupAzure" ).GetComponent<GroupBehaviour>();
        groupPink = GameObject.Find( "GroupPink" ).GetComponent<GroupBehaviour>();
        groupGreen = GameObject.Find( "GroupGreen" ).GetComponent<GroupBehaviour>();
        groupYellow = GameObject.Find( "GroupYellow" ).GetComponent<GroupBehaviour>();

        aggroA = GameObject.Find( "GroupAzure" ).GetComponent<GroupAggro>();
        aggroB = GameObject.Find( "GroupPink" ).GetComponent<GroupAggro>();
        aggroC = GameObject.Find( "GroupGreen" ).GetComponent<GroupAggro>();
        aggroD = GameObject.Find( "GroupYellow" ).GetComponent<GroupAggro>();

        aggroValueA = GameObject.Find( "Azure aggro" ).GetComponent<Text>();
        aggroValueB = GameObject.Find( "Pink aggro" ).GetComponent<Text>();
        aggroValueC = GameObject.Find( "Green aggro" ).GetComponent<Text>();
        aggroValueD = GameObject.Find( "Yellow aggro" ).GetComponent<Text>();

        tacticA = GameObject.Find( "Azure tactic" ).GetComponent<Text>();
        tacticB = GameObject.Find( "Pink tactic" ).GetComponent<Text>();
        tacticC = GameObject.Find( "Green tactic" ).GetComponent<Text>();
        tacticD = GameObject.Find( "Yellow tactic" ).GetComponent<Text>();

        bossBehaviour = GameObject.FindGameObjectWithTag( "Boss" ).GetComponent<BossBehavior>();
        bossTargetGroup = GameObject.Find( "Boss target's group" ).GetComponent<Text>();

        tacticsManager = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<TacticsManager>();
        currentGroup = GameObject.Find( "Current Group" ).GetComponent<Text>();
        currentTactic = GameObject.Find( "Current Tactic" ).GetComponent<Text>();

        allyDemonSpawnerTest = GameObject.Find( "AllyDemonSpawner" ).GetComponent<AllyDemonSpawnerTest>();
        regenCountdown = GameObject.Find( "Regen Countdown" ).GetComponent<Text>();

        playerStats = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<Stats>();
        playerAggro = GameObject.Find( "Player Aggro" ).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        tacticA.text = groupAzure.currentState.ToString();
        tacticB.text = groupPink.currentState.ToString();
        tacticC.text = groupGreen.currentState.ToString();
        tacticD.text = groupYellow.currentState.ToString();

        aggroValueA.text = aggroA.GetAggro().ToString();
        aggroValueB.text = aggroB.GetAggro().ToString();
        aggroValueC.text = aggroC.GetAggro().ToString();
        aggroValueD.text = aggroD.GetAggro().ToString();

        currentGroup.text = tacticsManager.CurrentShowedGroup.ToString();
        currentTactic.text = tacticsManager.CurrentShowedState.ToString();

        if (!tacticsManager)
            tacticsManager = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<TacticsManager>();

        regenCountdown.text = "New ally imp in: " + (( int ) (allyDemonSpawnerTest.countdown)).ToString();

        playerAggro.text = "Player's aggro: " + playerStats.Aggro.ToString();

        if ( bossBehaviour.TargetDemon && bossBehaviour.TargetDemon.tag == "Player" )
            bossTargetGroup.text = "Boss target:    " + "Player";
        else
            bossTargetGroup.text = "Boss target:    " + bossBehaviour.TargetGroup.name;
    }
}
