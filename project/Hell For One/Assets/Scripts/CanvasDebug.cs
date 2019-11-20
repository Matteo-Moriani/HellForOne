using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasDebug : MonoBehaviour
{
    public GroupBehaviour groupA, groupB, groupC, groupD;
    public GroupAggro aggroA, aggroB, aggroC, aggroD;
    public Text aggroValueA, aggroValueB, aggroValueC, aggroValueD;
    public Text tacticA, tacticB, tacticC, tacticD;
    public Text bossTargetGroup;
    public Text currentGroup;
    public Text currentTactic;
    public BossBehavior bossBehaviour;
    public TacticsManager tacticsManager;

    // Start is called before the first frame update
    void Start()
    {
        groupA = GameObject.Find( "GroupA" ).GetComponent<GroupBehaviour>();
        groupB = GameObject.Find( "GroupB" ).GetComponent<GroupBehaviour>();
        groupC = GameObject.Find( "GroupC" ).GetComponent<GroupBehaviour>();
        groupD = GameObject.Find( "GroupD" ).GetComponent<GroupBehaviour>();

        aggroA = GameObject.Find( "GroupA" ).GetComponent<GroupAggro>();
        aggroB = GameObject.Find( "GroupB" ).GetComponent<GroupAggro>();
        aggroC = GameObject.Find( "GroupC" ).GetComponent<GroupAggro>();
        aggroD = GameObject.Find( "GroupD" ).GetComponent<GroupAggro>();

        aggroValueA = GameObject.Find( "A aggro" ).GetComponent<Text>();
        aggroValueB = GameObject.Find( "B aggro" ).GetComponent<Text>();
        aggroValueC = GameObject.Find( "C aggro" ).GetComponent<Text>();
        aggroValueD = GameObject.Find( "D aggro" ).GetComponent<Text>();

        tacticA = GameObject.Find( "A tactic" ).GetComponent<Text>();
        tacticB = GameObject.Find( "B tactic" ).GetComponent<Text>();
        tacticC = GameObject.Find( "C tactic" ).GetComponent<Text>();
        tacticD = GameObject.Find( "D tactic" ).GetComponent<Text>();

        bossBehaviour = GameObject.FindGameObjectWithTag( "Boss" ).GetComponent<BossBehavior>();
        bossTargetGroup = GameObject.Find( "Boss target's group" ).GetComponent<Text>();

        tacticsManager = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<TacticsManager>();
        currentGroup = GameObject.Find( "Current Group" ).GetComponent<Text>();
        currentTactic = GameObject.Find( "Current Tactic" ).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        tacticA.text = groupA.currentState.ToString();
        tacticB.text = groupB.currentState.ToString();
        tacticC.text = groupC.currentState.ToString();
        tacticD.text = groupD.currentState.ToString();

        aggroValueA.text = aggroA.GetAggro().ToString();
        aggroValueB.text = aggroB.GetAggro().ToString();
        aggroValueC.text = aggroC.GetAggro().ToString();
        aggroValueD.text = aggroD.GetAggro().ToString();

        currentGroup.text = tacticsManager.CurrentShowedGroup.ToString();
        currentTactic.text = tacticsManager.CurrentShowedState.ToString();

        if ( bossBehaviour.TargetDemon.tag == "Player" )
            bossTargetGroup.text = "Boss target:    " + "Player";
        else
            bossTargetGroup.text = "Boss target:    " + bossBehaviour.TargetGroup.name;
    }
}
