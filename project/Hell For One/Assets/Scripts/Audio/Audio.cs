using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
    #region Fields

    private AudioSource combatAudioSource;
    private AudioSource walkAudioSource;
    private AudioSource deathAudioSource;
    private AudioSource dashAudioSource;
    private AudioSource roarAudioSource;

    private Stats stats;

    private CombatEventsManager combatEventsManager;
    private NormalCombat normalCombat;
    private Block block;
    private IdleCombat idleCombat;

    private Coroutine walkCR;

    private Coroutine dashCr;

    #endregion

    #region Unity methods

    private void Awake()
    {
        combatEventsManager = GetComponent<CombatEventsManager>();
        stats = GetComponent<Stats>();
        normalCombat = GetComponentInChildren<NormalCombat>();
        block = GetComponentInChildren<Block>();
        idleCombat = GetComponentInChildren<IdleCombat>();
    }

    private void OnEnable()
    {
        block.onBlockFailed += OnBlockFailed;
        block.onBlockSuccess += OnBlockSuccess;

        idleCombat.onAttackBeingHit += OnAttackBeingHit;
        
        combatEventsManager.onStartMoving += PlayFootStep;
        combatEventsManager.onStopMoving += StopFootStep;
        stats.onDeath += OnDeath;
        combatEventsManager.onStartDash += PlayDashClip;
        combatEventsManager.onStartGlobalAttack += PlayRoarClip;
    }
    
    private void OnDisable()
    {
        block.onBlockFailed += OnBlockFailed;
        block.onBlockSuccess += OnBlockSuccess;

        idleCombat.onAttackBeingHit += OnAttackBeingHit;
        
        combatEventsManager.onStartMoving -= PlayFootStep;
        combatEventsManager.onStopMoving -= StopFootStep;
        stats.onDeath -= OnDeath;
        combatEventsManager.onStartDash -= PlayDashClip;
        combatEventsManager.onStartGlobalAttack -= PlayRoarClip;
    }

    private void Start()
    {
        stats = GetComponent<Stats>();

        combatAudioSource = gameObject.AddComponent<AudioSource>();
        AudioManager.Instance.SetAudioAudioSource(combatAudioSource, true, 5f, 500f, false);
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        AudioManager.Instance.SetAudioAudioSource(walkAudioSource, true, 5f, 500f, false);
        deathAudioSource = gameObject.AddComponent<AudioSource>();
        AudioManager.Instance.SetAudioAudioSource(deathAudioSource,true,5f,500f,false);
        dashAudioSource = gameObject.AddComponent<AudioSource>();
        AudioManager.Instance.SetAudioAudioSource(dashAudioSource,true,5f,500f,false);
        
        if(stats.ThisUnitType == Stats.Type.Boss) {
            roarAudioSource = gameObject.AddComponent<AudioSource>();
            AudioManager.Instance.SetAudioAudioSource(roarAudioSource,true,10,500,false);
            roarAudioSource.outputAudioMixerGroup = AudioManager.Instance.RoarMixerGroup;
        }
        
        walkAudioSource.outputAudioMixerGroup = AudioManager.Instance.WalkAudioMixerGroup;
        combatAudioSource.outputAudioMixerGroup = AudioManager.Instance.CombatAudioMixerGroup;
        deathAudioSource.outputAudioMixerGroup = AudioManager.Instance.DeathMixerGroup;
        dashAudioSource.outputAudioMixerGroup = AudioManager.Instance.DashMixerGroup;
    }

    #endregion

    #region Methods

    private void PlayRoarClip() { 
        AudioManager.Instance.PlayRandomRoarClip(roarAudioSource);    
    }

    private void PlayDashClip() { 
        if(dashCr == null) { 
            dashCr = StartCoroutine(dashCoroutine());        
        }    
    }

    private void StopDashClip() { 
        if(dashCr != null){ 
            StopCoroutine(dashCr);
            dashCr = null;
        }    
    }
    
    private void PlayHitClip() { 
        AudioManager.Instance.PlayRandomCombatAudioClip(AudioManager.CombatAudio.Hit,combatAudioSource);    
    }

    private void PlayBlockClip() { 
        AudioManager.Instance.PlayRandomCombatAudioClip(AudioManager.CombatAudio.Block,combatAudioSource);    
    }

    private void PlayDeathSound() { 
        AudioManager.Instance.PlayDeathSound(stats.ThisUnitType,deathAudioSource);    
    }

    private void PlayFootStep() { 
        if(walkCR == null) { 
            walkCR = StartCoroutine(walkCoroutine());    
        }   
    }

    private void StopFootStep() { 
        if(walkCR != null) { 
            StopCoroutine(walkCR);
            walkCR = null;
            walkAudioSource.Stop();
        }    
    }

    #endregion

    #region Event handlers
    
    private void OnAttackBeingHit(IdleCombat sender, GenericAttack genericAttack, NormalCombat attackernormalcombat)
    {
        PlayHitClip();
    }

    private void OnBlockSuccess(Block sender, GenericAttack genericAttack, NormalCombat attackernormalcombat)
    {
        PlayBlockClip();
    }

    private void OnBlockFailed(Block sender, GenericAttack genericAttack, NormalCombat attackernormalcombat)
    {
        PlayHitClip();
    }

    private void OnDeath(Stats sender)
    {
        PlayDeathSound();
    }

    #endregion

    #region Coroutines

    private IEnumerator walkCoroutine() {
        while (true) {
            yield return new WaitForSeconds(AudioManager.Instance.PlayRandomWalkClip(AudioManager.Size.Small, walkAudioSource));// + Random.Range(0,0.01f));
        }    
    }

    private IEnumerator dashCoroutine() { 
        AudioManager.Instance.LockWalkAudio();
        yield return new WaitForSeconds(AudioManager.Instance.PlayRandomDashClip(dashAudioSource));
        AudioManager.Instance.UnlockWalkAudio();
        StopDashClip();
    }

    #endregion
}
