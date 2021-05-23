using ArenaSystem;
using ManaSystem;
using ReincarnationSystem;
using System;
using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private float shortMessageDuration;
    [SerializeField] private float mediumMessageDuration;
    [SerializeField] private float longMessageDuration;
    [SerializeField] private float distanceBetweenTutorials;
    [SerializeField] private float coroutinesWaitTime;
    [SerializeField] private ArenaManager firstArena;
    [SerializeField] private ArenaManager secondArena;

    // if we don't want to see the tutorial while debugging
    public bool tutorialsEnabled = true;
    public float tutorialClosingDelay = 2f;

    private bool specialAbilityTutorialDone = false;
    private bool callToArmsTutorialDone = false;
    private bool inSecondBattle = false;
    private bool tacticsTutorialsDone = false;
    private bool reincarnationTutorialDone = false;

    private TutorialScreensBehaviour _tutorialScreens;

    private static TutorialManager _instance;

    public static TutorialManager Instance
    {
        get => _instance;
        private set => _instance = value;
    }

    private void Awake()
    {
        if(_instance == null && tutorialsEnabled)
            _instance = this;
        else
            Destroy(this);

        _tutorialScreens = GetComponent<TutorialScreensBehaviour>();
    }

    public void StartTutorials()
    {
        StartCoroutine(FirstTutorial());
    }

    private void OnEnable()
    {
        firstArena.OnSetupBattle += OnFirstArenaEnter;
        firstArena.OnStartBattle += OnFirstBattleStart;
        ImpMana.OnSegmentCharged += OnSegmentCharged;
        secondArena.OnSetupBattle += OnSecondArenaEnter;
        secondArena.OnStartBattle += OnSecondBattleStart;
        ReincarnationManager.OnLeaderDeath += OnLeaderDeath;
        ReincarnationManager.OnLeaderReincarnated += OnLeaderReincarnated;
    }

    private void OnDisable()
    {
        
        if(tutorialsEnabled)
        {
            // mi deregistro solo se non mi sono già autodistrutto
            // mi devo deregistrare anche dove non mi serve più un certo tutorial

            firstArena.OnSetupBattle -= OnFirstArenaEnter;
            firstArena.OnStartBattle -= OnFirstBattleStart;
            ImpMana.OnSegmentCharged -= OnSegmentCharged;
            secondArena.OnSetupBattle -= OnSecondArenaEnter;
            secondArena.OnStartBattle -= OnSecondBattleStart;
            ReincarnationManager.OnLeaderDeath -= OnLeaderDeath;
            ReincarnationManager.OnLeaderReincarnated -= OnLeaderReincarnated;
        }
    }

    private IEnumerator CloseTutorial(string tutorial)
    {
        yield return new WaitForSeconds(tutorialClosingDelay);
        _tutorialScreens.HideScreen(tutorial);
    }

    // se il tutorial dopo un tot va chiuso comunque
    private IEnumerator ForceCloseTutorial(string tutorial)
    {
        yield return new WaitForSeconds(coroutinesWaitTime);
        StartCoroutine(CloseTutorial(tutorial));
    }

    private IEnumerator FirstTutorial()
    {
        yield return new WaitForSeconds(2f);
        _tutorialScreens.ShowScreenWithTimeout("Intro", longMessageDuration);
    }

    private IEnumerator FirstBattleTutorials()
    {
        yield return new WaitForSeconds(distanceBetweenTutorials);
        _tutorialScreens.ShowScreenWithTimeout("RangedTutorial", mediumMessageDuration);
        yield return new WaitForSeconds(mediumMessageDuration);

        while(_tutorialScreens.ShowingScreen()) yield return new WaitForSeconds(coroutinesWaitTime);

        yield return new WaitForSeconds(distanceBetweenTutorials);
        _tutorialScreens.ShowScreenWithTimeout("CounterTutorial", mediumMessageDuration);
        yield return new WaitForSeconds(mediumMessageDuration);

        while(_tutorialScreens.ShowingScreen()) yield return new WaitForSeconds(coroutinesWaitTime);

        yield return new WaitForSeconds(distanceBetweenTutorials);
        _tutorialScreens.ShowScreenWithTimeout("RecruitTutorial", mediumMessageDuration);
        yield return new WaitForSeconds(mediumMessageDuration);

        while(_tutorialScreens.ShowingScreen()) yield return new WaitForSeconds(coroutinesWaitTime);

        yield return new WaitForSeconds(distanceBetweenTutorials);
        if(ImpMana.CurrentChargedSegments > 0)
        {
            StartCoroutine(SpecialAbilityTutorial());
            specialAbilityTutorialDone = true;
        }
        tacticsTutorialsDone = true;
    }

    private IEnumerator SecondBattleTutorials() 
    {
        yield return new WaitForSeconds(distanceBetweenTutorials);
        _tutorialScreens.ShowScreenWithTimeout("MassOrderTutorial", mediumMessageDuration);
        yield return new WaitForSeconds(mediumMessageDuration);

        while(_tutorialScreens.ShowingScreen()) yield return new WaitForSeconds(coroutinesWaitTime);

        yield return new WaitForSeconds(distanceBetweenTutorials);
        _tutorialScreens.ShowScreenWithTimeout("MassOrderTutorial2", mediumMessageDuration);
    }

    private IEnumerator SpecialAbilityTutorial()
    {
        while(_tutorialScreens.ShowingScreen())
        {
            yield return new WaitForSeconds(1f);
        }
        _tutorialScreens.ShowScreenWithTimeout("SpecialAbilityTutorial", longMessageDuration); 
    }

    private IEnumerator ReincarnationTutorial()
    {
        while(_tutorialScreens.ShowingScreen())
        {
            yield return new WaitForSeconds(1f);
        }
        if(!reincarnationTutorialDone)
        {
            _tutorialScreens.ShowScreenWithTimeout("ReincarnationTutorial", longMessageDuration);
            yield return new WaitForSeconds(longMessageDuration);
            reincarnationTutorialDone = true;
            _tutorialScreens.ShowScreenWithTimeout("DashTutorial", longMessageDuration);
        }
        
    }

    private void OnFirstArenaEnter()
    {
        _tutorialScreens.ShowScreenWithTimeout("TacticsCrossTutorial", longMessageDuration);
    }

    private void OnSecondArenaEnter()
    {
        inSecondBattle = true;
    }

    private void OnFirstBattleStart()
    {
        StartCoroutine(FirstBattleTutorials());
    }

    private void OnSecondBattleStart()
    {
        StartCoroutine(SecondBattleTutorials());
    }

    private void OnSegmentCharged(int segments)
    {
        if(tacticsTutorialsDone && segments == 1 && !specialAbilityTutorialDone)
        {
            StartCoroutine(SpecialAbilityTutorial());
            specialAbilityTutorialDone = true;
        }
        else if (inSecondBattle && segments == 2 && !callToArmsTutorialDone)
        {
            _tutorialScreens.ShowScreenWithTimeout("CallToArmsTutorial", longMessageDuration);
            callToArmsTutorialDone = true;
        }
    }

    private void OnLeaderDeath(ReincarnableBehaviour r)
    {
        StartCoroutine(ReincarnationTutorial());
    }

    private void OnLeaderReincarnated(ReincarnableBehaviour r)
    {
        if(_tutorialScreens.ShowingScreen())
        {
            reincarnationTutorialDone = true;
            _tutorialScreens.HideScreen("ReincarnationTutorial");
        }
    }

}
