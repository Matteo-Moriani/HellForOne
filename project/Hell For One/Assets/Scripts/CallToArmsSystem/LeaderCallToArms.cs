using System;
using System.Collections;
using ActionsBlockSystem;
using AggroSystem;
using ArenaSystem;
using CrownSystem;
using HordeSystem;
using ManaSystem;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CallToArmsSystem
{
    // No rotation
    
    public class LeaderCallToArms : MonoBehaviour, ICrownObserver, IActionsBlockObserver, IActionsBlockSubject, IPlayerAggroSubject
    {
        [SerializeField] private UnitActionsBlockManager.UnitAction[] actionBlocks;
        [SerializeField, Range(0,100)] private float onActionAggro;
        
        [SerializeField, Min(0)] private int manaSegmentsCost;
        [SerializeField, Range(0,1)] private float availableHordePercentage;
        [SerializeField, Min(0f)] private float duration;
        
        private ImpMana _impMana;
        private ArenaManager _currentArena;

        private readonly ActionLock _callToArmsLock = new ActionLock();

        public event Action OnCallToArmsStart;
        public event Action OnCallToArmsStop;

        private Coroutine _callToArmsCr = null;
        
        private void Awake()
        {
            _impMana = GetComponent<ImpMana>();
        }

        private void OnEnable()
        {
            ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        }

        private void OnDisable()
        {
            ArenaManager.OnGlobalStartBattle += OnGlobalStartBattle;
            ArenaManager.OnGlobalEndBattle += OnGlobalEndBattle;
        }

        private void TryCallToArms()
        {
            if(!_impMana.CheckSegments(manaSegmentsCost)) return;
            if(_callToArmsCr != null) return;
            if(_currentArena == null) return;

            _callToArmsCr = StartCoroutine(CallToArms());
            
            OnCallToArmsStart?.Invoke();
            OnBlockEvent?.Invoke(actionBlocks);
            OnAggroActionDone?.Invoke(onActionAggro);
        }

        private void StopCallToArms()
        {
            if(_callToArmsCr == null) return;
            
            StopCoroutine(_callToArmsCr);
            _callToArmsCr = null;
            
            OnCallToArmsStop?.Invoke();
            OnUnblockEvent?.Invoke(actionBlocks);
        }

        private IEnumerator CallToArms()
        {
            yield return new WaitForSeconds(duration);

            int toSpawn = (int) (HordeManager.Instance.AvailableSlots() * availableHordePercentage);
            
            for(int i = 0; i < toSpawn; i++)
                HordeManager.Instance.SpawnImp(_currentArena.NewImpSpawnAnchors[Random.Range(0,_currentArena.NewImpSpawnAnchors.Length)].position,Quaternion.identity);
            
            _impMana.SpendSegments(manaSegmentsCost);
            
            StopCallToArms();
        }
        
        private void OnGlobalStartBattle(ArenaManager obj) => _currentArena = obj;

        private void OnGlobalEndBattle(ArenaManager obj) => _currentArena = null;

        private void OnCallToArmsInputDown() => TryCallToArms();

        private void OnCallToArmsInputUp() => StopCallToArms();

        public void OnCrownCollected()
        {
            PlayerInput.OnCallToArmsInputDown += OnCallToArmsInputDown;
            PlayerInput.OnCallToArmsInputUp += OnCallToArmsInputUp;
        }
        
        public void OnCrownLost()
        {
            PlayerInput.OnCallToArmsInputDown -= OnCallToArmsInputDown;
            PlayerInput.OnCallToArmsInputUp -= OnCallToArmsInputUp;
        }

        public void Block() => _callToArmsLock.AddLock();

        public void Unblock() => _callToArmsLock.RemoveLock();

        public UnitActionsBlockManager.UnitAction GetAction() => UnitActionsBlockManager.UnitAction.CallToArms;
        
        public event Action<UnitActionsBlockManager.UnitAction[]> OnBlockEvent;
        public event Action<UnitActionsBlockManager.UnitAction[]> OnUnblockEvent;
        public event Action<float> OnAggroActionDone;
    }
}