using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsBlockSystem
{
    // TODO - Manage interrupts
    public class UnitActionsBlockManager : MonoBehaviour
    {
        public enum UnitAction
        {
            BeingHit,
            Dash,
            Move,
            GiveOrders,
            UseAbilities,
            ChargeMana,
            Attack
        }
        
        private readonly Dictionary<UnitAction, List<IActionsBlockObserver>> _observers = new Dictionary<UnitAction, List<IActionsBlockObserver>>();
        private IActionsBlockSubject[] _subjects;

        private void Awake()
        {
            _subjects = GetComponentsInChildren<IActionsBlockSubject>();
            
            foreach (UnitAction action in Enum.GetValues(typeof(UnitAction)))
                _observers.Add(action,new List<IActionsBlockObserver>());
            
            foreach (IActionsBlockObserver observer in GetComponentsInChildren<IActionsBlockObserver>())
                _observers[observer.GetAction()].Add(observer);
        }

        private void OnEnable()
        {
            foreach (IActionsBlockSubject subject in _subjects)
            {
                subject.OnBlockEvent += OnBlockEvent;
                subject.OnUnblockEvent += OnUnblockEvent;
            }
        }

        private void OnDisable()
        {
            foreach (IActionsBlockSubject subject in _subjects)
            {
                subject.OnBlockEvent -= OnBlockEvent;
                subject.OnUnblockEvent -= OnUnblockEvent;
            }
        }

        private void OnBlockEvent(UnitAction[] toBlock)
        {
            foreach (UnitAction playerAction in toBlock)
            {
                if(!_observers.ContainsKey(playerAction)) continue;

                _observers[playerAction].ForEach(observer => observer.Block());
            }      
        }

        private void OnUnblockEvent(UnitAction[] toUnblock)
        {
            foreach (UnitAction playerAction in toUnblock)
            {
                if(!_observers.ContainsKey(playerAction)) continue;

                _observers[playerAction].ForEach(observer => observer.Unblock());
            }     
        }
    }
}