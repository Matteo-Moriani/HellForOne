using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // if we don't want to see the tutorial while debugging
    public bool tutorialsEnabled = true;
    public float tutorialClosingDelay = 2f;

    private TutorialScreensBehaviour _tutorialScreens;
    private Transform _player;

    // FLAG UTILI PER TENERE CONTO DI CERTE COSE NELLE SCHERMATE
    //private bool _rightAnalogUsed = false;
    //private bool _playerHasShot = false;
    //private bool _toolsUsed = false;
    //private bool _dashUsed = false;
    //private bool _sonarVisionUsed = false;
    //private bool _healingUsed = false;
    //private int _battlesEntered = 0;

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

        _tutorialScreens = GameObject.FindGameObjectWithTag("TutorialScreens").GetComponent<TutorialScreensBehaviour>();
    }

    private void Start()
    {
        _tutorialScreens.ShowScreen("Intro");
    }

    private void OnEnable()
    {
        // mi registro ai vari eventi a giro
    }

    private void OnDisable()
    {
        
        if(tutorialsEnabled)
        {
            // mi deregistro solo se non mi sono già autodistrutto
            // mi devo deregistrare anche dove non mi serve più un certo tutorial
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
        yield return new WaitForSeconds(3f);
        StartCoroutine(CloseTutorial(tutorial));
    }

    // ESEMPI DI METODI DA REGISTRARE AGLI EVENTI

    //private void LeftAnalogUsed(Vector2 moveInput)
    //{
    //    if(moveInput.magnitude >= 0.1f)
    //    {
    //        PlayerInput.OnMoveInput -= LeftAnalogUsed;
    //        StartCoroutine(CloseMovementTutorial());
    //    }
    //}

    //private void RightAnalogUsed(Vector2 rotateInput)
    //{
    //    if(rotateInput.magnitude >= 0.1f)
    //    {
    //        _rightAnalogUsed = true;
    //        PlayerInput.OnRotateInput -= RightAnalogUsed;
    //    }

    //    if(_playerHasShot)
    //        StartCoroutine(CloseTutorial("AimShootTutorial"));
    //}

    //private void PlayerHasShot()
    //{
    //    _playerHasShot = true;
    //    PlayerInput.OnStartFireInput -= PlayerHasShot;

    //    if(_rightAnalogUsed)
    //        StartCoroutine(CloseTutorial("AimShootTutorial"));
    //}

    //private void DashUsed()
    //{
    //    _dashUsed = true;
    //    PlayerInput.OnDashInputDown -= DashUsed;
    //    StartCoroutine(CloseTutorial("DashTutorial"));
    //}

    //private void ToolsUsed()
    //{
    //    _toolsUsed = true;
    //    PlayerInput.OnLeftToolInput -= ToolsUsed;
    //    StartCoroutine(CloseTutorial("ToolsTutorial"));
    //}

    //private void SonarVisionUsed()
    //{
    //    _sonarVisionUsed = true;
    //    PlayerInput.OnStartSonarVisionInput -= SonarVisionUsed;
    //    StartCoroutine(CloseTutorial("SonarVisionTutorial"));
    //}

    //private void OnHealingSuccess()
    //{
    //    _healingUsed = true;
    //    _player.GetComponent<PlayerHealing>().OnHealingSuccess -= OnHealingSuccess;
    //    _player.GetComponent<HitPoints>().OnDamageReceived -= OnDamageReceived;
    //    StartCoroutine(CloseTutorial("HealTutorial"));
    //}

    //private void OnDamageReceived(float amount)
    //{
    //    if(!_healingUsed)
    //    {
    //        _tutorialScreens.ShowScreen("HealTutorial");
    //    }
    //}






    // ESEMPI DI METODI PER LA GESTIONE DELLE SCHERMATE.

    //private void StartMovementTutorial()
    //{
    //    StartCoroutine(MovementTutorial());
    //}

    //private IEnumerator MovementTutorial()
    //{
    //    yield return new WaitForSeconds(2f);
    //    _tutorialScreens.ShowScreen("MovementTutorial");
    //}

    //private IEnumerator CloseMovementTutorial()
    //{
    //    yield return new WaitForSeconds(1f);
    //    _tutorialScreens.HideScreen("MovementTutorial");

    //    yield return new WaitForSeconds(2f);
    //    AimShootTutorial();

    //    PlayerInput.OnRotateInput += RightAnalogUsed;
    //    PlayerInput.OnStartFireInput += PlayerHasShot;
    //    UIManager.Instance.OnMainGameUi -= StartMovementTutorial;
    //}

    //private void AimShootTutorial()
    //{
    //    if(!_rightAnalogUsed || !_playerHasShot)
    //        _tutorialScreens.ShowScreen("AimShootTutorial");
    //}

    //private void OnBattleEnter()
    //{
    //    _battlesEntered++;

    //    if(_battlesEntered == 2)
    //    {
    //        PlayerInput.OnLeftToolInput += ToolsUsed;
    //        PlayerInput.OnRightToolInput += ToolsUsed;
    //    }

    //    if(_battlesEntered >= 2 && !_toolsUsed)
    //    {
    //        _tutorialScreens.ShowScreen("ToolsTutorial");
    //    }
    //}

    //private void OnBattleExit()
    //{
    //    if(_battlesEntered == 1)
    //    {
    //        PlayerInput.OnDashInputDown += DashUsed;
    //    }
    //    else if (_battlesEntered == 2)
    //    {
    //        PlayerInput.OnStartSonarVisionInput += SonarVisionUsed;
    //    }

    //    if(_battlesEntered >= 1 && !_dashUsed)
    //    {
    //        _tutorialScreens.ShowScreen("DashTutorial");
    //    }
    //    else if(_battlesEntered >= 2 && !_sonarVisionUsed)
    //    {
    //        _tutorialScreens.ShowScreen("SonarVisionTutorial");
    //    }
    //}

    //private void OnRangerUnconscious(Transform t)
    //{
    //    _tutorialScreens.ShowScreen("ReviveTutorial");
    //    StartCoroutine(ForceCloseTutorial("ReviveTutorial"));
    //    RangerManager.Instance.OnRangerUnconscious -= OnRangerUnconscious;
    //}

    //private void OnEmptyMagazine()
    //{
    //    if(_player.GetComponent<PlayerEquipment>().Firearm.Name != "GrenadeLauncher")
    //    {
    //        _tutorialScreens.ShowScreen("ReloadTutorial");
    //        StartCoroutine(ForceCloseTutorial("ReloadTutorial"));
    //        _player.GetComponent<PlayerReloading>().OnEmptyMagazine -= OnEmptyMagazine;
    //    }
    //}

}
